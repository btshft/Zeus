﻿using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.Shared.Extensions;

namespace Zeus.Shared.AppFeature
{
    public abstract class AppFeature : IAppFeature
    {
        private readonly MethodHolder _methodHolder;

        protected IConfiguration Configuration { get; }

        protected IHostEnvironment Environment { get; }

        protected AppFeature(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
            _methodHolder = new MethodHolder(GetType());
        }

        /// <inheritdoc />
        public bool Use(IApplicationBuilder builder)
        {
            UseFeature(builder);
            return _methodHolder.UseFeature.IsOverriden();
        }

        protected virtual void UseFeature(IApplicationBuilder builder) { }

        /// <inheritdoc />
        public bool Map(IEndpointRouteBuilder endpoints)
        {
            MapFeature(endpoints);
            return _methodHolder.MapFeature.IsOverriden();
        }

        protected virtual void MapFeature(IEndpointRouteBuilder endpoints) { }

        /// <inheritdoc />
        public bool Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            ConfigureFeature(services, features);
            return _methodHolder.ConfigureFeature.IsOverriden();
        }

        protected virtual void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features) { }

        private class MethodHolder
        {
            public MethodHolder(Type sourceType)
            {
                var typeInfo = sourceType.GetTypeInfo();

                UseFeature = typeInfo.GetDeclaredMethod(nameof(AppFeature.UseFeature));
                MapFeature = typeInfo.GetDeclaredMethod(nameof(AppFeature.MapFeature));
                ConfigureFeature = typeInfo.GetDeclaredMethod(nameof(AppFeature.ConfigureFeature));
            }

            public MethodInfo UseFeature { get; }

            public MethodInfo MapFeature { get; }

            public MethodInfo ConfigureFeature { get; }
        }
    }

    public abstract class AppFeature<TOptions> : AppFeature, IAppFeature<TOptions>
        where TOptions : class, new()
    {
        protected IOptions<TOptions> Options { get; }

        protected AppFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<TOptions> options)
            : base(configuration, environment)
        {
            Options = options;
        }
    }
}