using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace Zeus.Middleware
{
    public class LogRequestResponseMiddleware
    {
        private const int BufferLength = 4096;

        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _memoryStreamManager;

        public LogRequestResponseMiddleware(RequestDelegate next)
        {
            _next = next;
            _memoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Zeus.Http.RequestResponse");
            if (!logger.IsEnabled(LogLevel.Debug))
            {
                await _next(context);
                return;
            }

            var requestText = await FormatRequestAsync(context, _memoryStreamManager);

            logger.LogDebug($"HTTP Request {Environment.NewLine}{requestText}");

            var originalBody = context.Response.Body;

            await using var bodyCopy = _memoryStreamManager.GetStream();
            context.Response.Body = bodyCopy;

            await _next(context);

            bodyCopy.Seek(0, SeekOrigin.Begin);
            await bodyCopy.CopyToAsync(originalBody);

            bodyCopy.Seek(0, SeekOrigin.Begin);
            var responseText = await FormatResponseAsync(context, bodyCopy);

            logger.LogDebug($"HTTP Response {Environment.NewLine}{responseText}");
        }

        private static async Task<string> FormatResponseAsync(HttpContext context, Stream responseStream)
        {
            var request = context.Request;
            var response = context.Response;

            var bodyText = await ChunkReadAsync(responseStream);

            return  $"Schema: {request.Scheme} {Environment.NewLine}" +
                    $"Host: {request.Host} {Environment.NewLine}" +
                    $"Path: {request.Path} {Environment.NewLine}" +
                    $"QueryString: {request.QueryString} {Environment.NewLine}" +
                    $"StatusCode: {response.StatusCode} {Environment.NewLine}" +
                    $"Body: {bodyText}";
        }

        private static async Task<string> FormatRequestAsync(HttpContext context, RecyclableMemoryStreamManager manager)
        {
            var request = context.Request;
            var bodyText = await GetRequestBodyAsync(request, manager);

            return $"Schema: {request.Scheme} {Environment.NewLine}" +
                   $"Host: {request.Host} {Environment.NewLine}" +
                   $"Path: {request.Path} {Environment.NewLine}" +
                   $"QueryString: {request.QueryString} {Environment.NewLine}" +
                   $"Body: {bodyText}";
        }

        private static async Task<string> GetRequestBodyAsync(HttpRequest request, RecyclableMemoryStreamManager manager)
        {
            request.EnableBuffering();

            await using var requestStream = manager.GetStream();
            await request.Body.CopyToAsync(requestStream);

            request.Body.Seek(0, SeekOrigin.Begin);
            requestStream.Seek(0, SeekOrigin.Begin);

            return await ChunkReadAsync(requestStream);
        }

        private static async Task<string> ChunkReadAsync(Stream stream)
        {
            await using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var chunk = new char[BufferLength];
            int length;

            do
            {
                length = await reader.ReadBlockAsync(chunk, 0, BufferLength);
                await textWriter.WriteAsync(chunk, 0, length);

            } while (length > 0);

            var result = textWriter.ToString();

            return result;
        }
    }
}
