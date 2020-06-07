using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Zeus.v2.Shared.Json
{
    public class NewtonsoftJsonModelBinder : IModelBinder
    {
        private readonly BodyModelBinder _bodyBinder;

        public NewtonsoftJsonModelBinder(
            IHttpRequestStreamReaderFactory readerFactory, 
            ILoggerFactory loggerFactory,
            IOptions<MvcOptions> optionsProvider,
            IOptions<MvcNewtonsoftJsonOptions> newtonsoftOptionsProvider,
            ArrayPool<char> charPool,
            ObjectPoolProvider objectPoolProvider)
        {
            var options = optionsProvider.Value;
            var formatters = options.InputFormatters.ToList();

            var jsonFormatter = formatters.OfType<SystemTextJsonInputFormatter>().FirstOrDefault();
            var newtonsoftJsonFormatter = formatters.OfType<NewtonsoftJsonInputFormatter>().FirstOrDefault();

            if (jsonFormatter != null && newtonsoftJsonFormatter == null)
            {
                var jsonFormatterIndex = formatters.IndexOf(jsonFormatter);
                var logger = loggerFactory.CreateLogger<NewtonsoftJsonInputFormatter>();
                var settings = JsonSerializerSettingsProvider.CreateSerializerSettings();

                formatters[jsonFormatterIndex] = new NewtonsoftJsonInputFormatter(
                    logger, settings, charPool, objectPoolProvider, options, newtonsoftOptionsProvider.Value);
            }

            _bodyBinder = new BodyModelBinder(formatters, readerFactory, loggerFactory, options);
        }

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            return _bodyBinder.BindModelAsync(bindingContext);
        }
    }
}