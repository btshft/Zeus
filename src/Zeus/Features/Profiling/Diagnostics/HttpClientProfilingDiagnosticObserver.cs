using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Extensions.DiagnosticAdapter;
using StackExchange.Profiling;
using Zeus.Shared.Diagnostics;

namespace Zeus.Features.Profiling.Diagnostics
{
    /// <summary>
    /// https://github.com/dotnet/corefx/blob/master/src/System.Net.Http/src/HttpDiagnosticsGuide.md
    /// </summary>
    public class HttpClientProfilingDiagnosticObserver : DiagnosticObserver
    {
        private static readonly ConcurrentDictionary<string, IDisposable> Timers 
            = new ConcurrentDictionary<string, IDisposable>();

        /// <inheritdoc />
        protected override bool IsMatch(string component)
        {
            return component == "HttpHandlerDiagnosticListener";
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Start")]
        public void RequestStart(HttpRequestMessage request)
        {
            var profiler = MiniProfiler.Current;
            if (profiler != null)
            {
                Timers.TryAdd(Activity.Current.Id, profiler.Step($"HttpClient: '{request.RequestUri}'"));
            }
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Stop")]
        public void RequestStop()
        {
            if (Timers.TryRemove(Activity.Current.Id, out var timer))
            {
                timer.Dispose();
            }
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut")]
        public void Precondition() { }
    } 
}
