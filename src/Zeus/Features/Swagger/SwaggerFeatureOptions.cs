using Zeus.Shared.Features.Optional;

namespace Zeus.Features.Swagger
{
    public class SwaggerFeatureOptions : OptionalFeatureOptions
    {
        public SwaggerFeatureOptions()
        {
            Enabled = true;
        }
    }
}