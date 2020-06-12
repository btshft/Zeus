using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.Shared.AppFeature;
using Zeus.Shared.Extensions;

namespace Zeus.Shared.Features.Optional
{
    public abstract class OptionalFeature<TOptions> : IAppFeature<TOptions> 
        where TOptions : OptionalFeatureOptions, new()
    {
        private readonly MethodHolder _methodHolder;

        protected IConfiguration Configuration { get; }

        protected IHostEnvironment Environment { get; }

        protected IOptions<TOptions> Options { get; }

        protected OptionalFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<TOptions> options)
        {
            Configuration = configuration;
            Environment = environment;
            Options = options;
            _methodHolder = new MethodHolder(GetType());
        }

        /// <inheritdoc />
        public bool Use(IApplicationBuilder builder)
        {
            if (Options.Value.Enabled)
                UseFeature(builder);

            var isOverriden = _methodHolder.UseFeature.IsOverriden();
            return Options.Value.Enabled && isOverriden;
        }

        protected virtual void UseFeature(IApplicationBuilder builder) { }

        /// <inheritdoc />
        public bool Map(IEndpointRouteBuilder endpoints)
        {
            if (Options.Value.Enabled)
                MapFeature(endpoints);

            var isOverriden = _methodHolder.MapFeature.IsOverriden();
            return Options.Value.Enabled && isOverriden;
        }

        protected virtual void MapFeature(IEndpointRouteBuilder endpoints) { }

        /// <inheritdoc />
        public bool Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            if (Options.Value.Enabled)
                ConfigureFeature(services, features);

            var isOverriden = _methodHolder.ConfigureFeature.IsOverriden();
            return Options.Value.Enabled && isOverriden;
        }

        protected virtual void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features) { }

        private class MethodHolder
        {
            public MethodHolder(Type sourceType)
            {
                var typeInfo = sourceType.GetTypeInfo();

                UseFeature = typeInfo.GetDeclaredMethod(nameof(OptionalFeature<TOptions>.UseFeature));
                MapFeature = typeInfo.GetDeclaredMethod(nameof(OptionalFeature<TOptions>.MapFeature));
                ConfigureFeature = typeInfo.GetDeclaredMethod(nameof(OptionalFeature<TOptions>.ConfigureFeature));
            }

            public MethodInfo UseFeature { get; }

            public MethodInfo MapFeature { get; }

            public MethodInfo ConfigureFeature { get; }
        }
    }
}
