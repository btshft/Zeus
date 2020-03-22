using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Templating.Abstraction
{
    public interface ITemplateEngine
    {
        Task<string> RenderAsync(string template, object model, CancellationToken cancellation = default);
        Task<string> RenderFileAsync(string path, object model, CancellationToken cancellation = default);
    }
}
