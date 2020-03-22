using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Scriban;
using Zeus.Templating.Abstraction;
using Zeus.Templating.Exceptions;

namespace Zeus.Templating.Scriban
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

            return typedTemplate.RenderAsync(model, options.GetRenamer(), options.MemberFilter).AsTask();
        }

        /// <inheritdoc />
        public async Task<string> RenderFileAsync(string path, object model, CancellationToken cancellation = default)
        {
            var options = _optionsFactory.Value;

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException($"{nameof(path)}'s value cannot be null or whitespace.", nameof(path));

            if (!File.Exists(path))
                throw new ArgumentException($"File '{path}' not found");

            var fileContent = await File.ReadAllTextAsync(path, Encoding.UTF8, cancellation);

            var typedTemplate = Template.Parse(fileContent, sourceFilePath: path, options.Parser, options.Lexer);
            if (typedTemplate.HasErrors)
            {
                var errors = string.Join(Environment.NewLine, typedTemplate.Messages);
                throw new TemplateRenderException($"Template '{path}' has errors and cant be rendered: {Environment.NewLine}{errors}");
            }

            return await typedTemplate.RenderAsync(model, options.GetRenamer(), options.MemberFilter).AsTask();
        }
    }
}