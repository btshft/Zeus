using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Stores.Default
{
    public class FileSystemTemplatesStore : ITemplatesStore
    {
        private static readonly IDictionary<string, AlertsTemplate.TemplateSyntax> SyntaxMap =
            new Dictionary<string, AlertsTemplate.TemplateSyntax>
            {
                {".tmpl", AlertsTemplate.TemplateSyntax.Default},
                {".md", AlertsTemplate.TemplateSyntax.Markdown},
                {".html", AlertsTemplate.TemplateSyntax.Html}
            };

        private readonly string _templatesPath;
        private readonly ConcurrentDictionary<string, Task<AlertsTemplate>> _templatesCache;

        public FileSystemTemplatesStore(string templatesPath)
        {
            if (string.IsNullOrWhiteSpace(templatesPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(templatesPath));

            if (!Directory.Exists(templatesPath))
                throw new ArgumentException($"Directory '{templatesPath}' not exists.", nameof(templatesPath));

            _templatesPath = templatesPath;
            _templatesCache = new ConcurrentDictionary<string, Task<AlertsTemplate>>(
                StringComparer.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public async Task<AlertsTemplate> GetAsync(string channel, CancellationToken cancellation = default)
        {
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));

            var cultureName = CultureInfo.CurrentCulture.Name;
            var template = await _templatesCache.GetOrAdd(channel,  ch =>
            {
                var filePrefixes = new[]
                {
                    $"{channel}.{cultureName}",
                    $"{channel}",
                    $"_default.{cultureName}",
                    "_default"
                };

                var fileName = filePrefixes.Select(GetMatchedFile).FirstOrDefault(f => f != null);
                if (fileName == null)
                    throw new FileNotFoundException($"Unable to resolve template file name for channel '{channel}'. " +
                                                    $"Searched for: '{channel}.{cultureName}.*', '{channel}.*', '_default.{cultureName}.*', '_default.*'");

                return CreateTemplateAsync(fileName);
            });

            return template;
        }

        private string GetMatchedFile(string namePrefix)
        {
            if (namePrefix == null) 
                throw new ArgumentNullException(nameof(namePrefix));

            var validFileNames = SyntaxMap.Keys
                .Select(ext => Path.Combine(_templatesPath, $"{namePrefix}{ext}"))
                .ToArray();

            return Directory.EnumerateFiles(_templatesPath, searchPattern: "*", SearchOption.TopDirectoryOnly)
                .Select(fn => fn.ToLowerInvariant())
                .FirstOrDefault(fn => validFileNames.Any(fn.ToLowerInvariant().Equals));
        }

        private static async Task<AlertsTemplate> CreateTemplateAsync(string filePath)
        {
            var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            var key = SyntaxMap.Keys.First(filePath.EndsWith);

            return new AlertsTemplate
            {
                Syntax = SyntaxMap[key],
                Content = content
            };
        }
    }
}