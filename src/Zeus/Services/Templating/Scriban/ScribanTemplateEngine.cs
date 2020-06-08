using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Scriban;
using Zeus.Services.Templating.Exceptions;

namespace Zeus.Services.Templating.Scriban
{
    public class ScribanTemplateEngine : ITemplateEngine
    {
        private readonly IOptions<ScribanOptions> _optionsFactory;

        public ScribanTemplateEngine(IOptions<ScribanOptions> optionsFactory)
        {
            _optionsFactory = optionsFactory;
        }

        /// <inheritdoc />
        public Task<string> RenderAsync(string template, object model, CancellationToken cancellation = default)
        {
            var options = _optionsFactory.Value;
            var typedTemplate = Template.Parse(template, sourceFilePath: null, options.Parser, options.Lexer);
            if (typedTemplate.HasErrors)
            {
                var errors = string.Join(Environment.NewLine, typedTemplate.Messages);
                throw new TemplateRenderException($"Template has errors and cant be rendered: {Environment.NewLine}{errors}");
            }

            return typedTemplate.RenderAsync(model, options.MemberRenamer, options.MemberFilter).AsTask();
        }

        /// <inheritdoc />
        public async Task<string> RenderFileAsync(string filePath, object model, CancellationToken cancellation = default)
        {
            var options = _optionsFactory.Value;

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"{nameof(filePath)}'s value cannot be null or whitespace.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new ArgumentException($"File '{filePath}' not found");

            var fileContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellation);

            var typedTemplate = Template.Parse(fileContent, sourceFilePath: filePath, options.Parser, options.Lexer);
            if (typedTemplate.HasErrors)
            {
                var errors = string.Join(Environment.NewLine, typedTemplate.Messages);
                throw new TemplateRenderException($"Template '{filePath}' has errors and cant be rendered: {Environment.NewLine}{errors}");
            }

            return await typedTemplate.RenderAsync(model, options.MemberRenamer, options.MemberFilter).AsTask();
        }
    }
}