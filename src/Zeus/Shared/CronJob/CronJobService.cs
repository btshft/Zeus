using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zeus.Shared.Threading;
using Timer = System.Timers.Timer;

namespace Zeus.Shared.CronJob
{
    internal class CronJobService<TJob> : BackgroundService 
        where TJob : class, ICronJob
    {
        private readonly string _jobName;
        private readonly string _jobFullName;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CronJobService<TJob>> _logger;

        private readonly TimeZoneInfo _timeZone;
        private readonly CronExpression _cronExpression;
        private readonly string _schedule;
        private readonly bool _useLocalTimeZone;

        private Timer _timer;

        public CronJobService(IOptionsMonitor<CronJobOptions> optionsMonitor, ILogger<CronJobService<TJob>> logger, IServiceProvider serviceProvider)
        {
            _jobName = typeof(TJob).Name;
            _jobFullName = typeof(TJob).FullName;

            var options = optionsMonitor.Get(_jobFullName);

            _schedule = options.Schedule;
            _cronExpression = CronExpression.Parse(options.Schedule);
            _logger = logger;
            _serviceProvider = serviceProvider;
            _useLocalTimeZone = options.UseLocalTimeZone;
            _timeZone = options.UseLocalTimeZone 
                ? TimeZoneInfo.Local 
                : TimeZoneInfo.Utc;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ScheduleJobAsync(stoppingToken);
        }

        /// <inheritdoc />
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            return base.StopAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();
            _timer?.Dispose();
        }

        protected Task ScheduleJobAsync(CancellationToken cancellation = default)
        {
            _logger.LogInformation($"Scheduling job '{_jobName}' with schedule '{_schedule}'");

            var from = _useLocalTimeZone
                ? DateTimeOffset.Now 
                : DateTimeOffset.UtcNow;

            var next = _cronExpression.GetNextOccurrence(from, _timeZone);

            if (next.HasValue)
            {
                _logger.LogInformation($"Job '{_jobName}' estimated next execution time is '{next.Value}'");

                var delay = next.Value - DateTimeOffset.Now;

                _timer = new Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    // Clean-up
                    _timer.Dispose();  
                    _timer = null;

                    if (cancellation.IsCancellationRequested)
                        return;

                    try
                    {
                        _logger.LogInformation($"Starting execution of job '{_jobName}'");

                        using (await ResourceLocker.LockAsync(_jobFullName, cancellation))
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var job = scope.ServiceProvider.GetRequiredService<TJob>();
                            await job.ExecuteAsync(cancellation);
                        }

                        _logger.LogInformation($"Job '{_jobName}' executed successfully");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Job '{_jobName}' execution failed with exception");
                    }
                    finally
                    {
                        if (!cancellation.IsCancellationRequested)
                            await ScheduleJobAsync(cancellation);
                    }
                };

                _timer.Start();
            }

            return Task.CompletedTask;
        }
    }
}