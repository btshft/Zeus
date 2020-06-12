using Zeus.Shared.Features.Optional;

namespace Zeus.Features.Api.Swagger
{
    public class SwaggerFeatureOptions : OptionalFeatureOptions
    {
        public SwaggerFeatureOptions()
        {
            Enabled = true;
        }
    }
}