using System.Threading;
using System.Threading.Tasks;

namespace Zeus.v2.Services.Templating
{
    public interface ITemplateEngine
    {
        Task<string> RenderAsync(string template, object model, CancellationToken cancellation = default);
        Task<string> RenderFileAsync(string filePath, object model, CancellationToken cancellation = default);
    }
}
