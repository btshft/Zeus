using Microsoft.Extensions.Localization;

namespace Zeus.Localization
{
    public interface IMessageLocalizer<TResource> : IStringLocalizer<TResource>
    {

    }
}
