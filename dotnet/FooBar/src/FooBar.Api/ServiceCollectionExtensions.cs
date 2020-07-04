using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using FooBar.Api.Behaviors;
using FooBar.Api.Infrastructure.Swagger;
using FooBar.Domain.Entities;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Interfaces;
using FooBar.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FooBar.Api
{
    // TODO: move out of here
    public class IsMemberOfOrganizationRequirement : IAuthorizationRequirement
    {
    }
    
    // TODO: move out of here
    public class IsMemberOfOrganizationHandler : AuthorizationHandler<IsMemberOfOrganizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsMemberOfOrganizationRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)  return Task.CompletedTask;
            
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
    
    public class ApiOptions
    {
        public string CorsPolicy { get; set; } = "CorsAllowAll";
    }
    
    public static class ServiceCollectionExtensions
    {
        public static void AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions(); // TODO: check this
            services.AddHttpContextAccessor(); // TODO: check this
            services.AddControllers(); // TODO: check this
            AddAuth0(services, configuration);
            
            // TODO: check this
            services.AddAuthorization(options => options.AddPolicy("IsMemberOfOrganization", 
                policy => policy.Requirements.Add(new IsMemberOfOrganizationRequirement())));
            services.AddSingleton<IAuthorizationHandler, IsMemberOfOrganizationHandler>();
            
            services.AddHealthChecks(); // TODO: check this
            AddDistributedCache(services);
            services.AddRouting(); // TODO: check this
            services.AddCors(options => options.AddPolicy(new ApiOptions().CorsPolicy, policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            services.AddApiVersioning(
                options =>
                {
                    // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });
            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    // options.GroupNameFormat = "'v'VVV";
                    options.GroupNameFormat = $"'v'{options.SubstitutionFormat}";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
        }

        public static void UseApi(this IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseCors(new ApiOptions().CorsPolicy);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
            
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.RoutePrefix = string.Empty;
                    // build a swagger endpoint for each discovered API version
                    foreach ( var description in apiVersionProvider.ApiVersionDescriptions )
                    {
                        options.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant() );
                    }
                });
        }
        
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            // services.AddTransient<ICachePolicy<ReadRssFeed, ReadRssFeedResponse>, ReadRssFeedCachePolicy>();
            services.AddValidatorsFromAssembly(typeof(Startup).GetTypeInfo().Assembly);
            services.AddDistributedRedisCache(option => option.Configuration = configuration["Redis:Configuration"]);

            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddScoped<ICatalogItemRepository, CatalogItemRepository>();
            services.AddScoped<IAsyncRepository<CatalogType>, EfRepository<CatalogType>>();
            services.AddScoped<IAsyncRepository<CatalogBrand>, EfRepository<CatalogBrand>>();
            
            services.AddDbContext<CatalogContext>(option 
                => option
                    .UseSqlite(configuration
                    .GetConnectionString("CatalogConnectionSqlite")
                    .Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory)));
        }
        
        // TODO: move out of here
        private static IServiceCollection AddAuth0(IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof (services));
            if (configuration == null) throw new ArgumentNullException(nameof (configuration));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Bearer";
                    options.DefaultChallengeScheme = "Bearer";
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration.GetSection("Auth0:Authority").Value;
                    options.Audience = configuration.GetSection("Auth0:Identifier").Value;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });
            return services;
        }
        
        // TODO: move out of here
        private static IServiceCollection AddDistributedCache(IServiceCollection services, Action<DistributedCacheOptions> setupAction = null)
        {
            if (!(services.BuildServiceProvider().GetService(typeof (IConfiguration)) is IConfiguration service))
                throw new Exception("Could not resolve service: IConfiguration");
            
            var cacheOptions = service.GetSection("DistributedCacheOptions").Get<DistributedCacheOptions>();
            if (cacheOptions == null)
            {
                throw new MissingConfigurationException("DistributedCacheOptions is not defined in appsettings.json");
            }
            
            setupAction?.Invoke(cacheOptions);
            
            if (cacheOptions.UseMemoryCache)
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cacheOptions.Configuration)) throw new InvalidOperationException("Configuration must be set!");
                if (string.IsNullOrWhiteSpace(cacheOptions.InstanceName)) throw new InvalidOperationException("InstanceName must be set!");
                services.AddOptions();
                services.Configure<RedisCacheOptions>((options =>
                {
                    options.Configuration = cacheOptions.Configuration;
                    options.InstanceName = cacheOptions.InstanceName;
                }));
                services.AddSingleton<IDistributedCache, RedisCache>();
            }
            
            return services;
        }
    }
    
    // TODO: move out of here
    public class DistributedCacheOptions : IOptions<DistributedCacheOptions>
    {
        public bool UseMemoryCache { get; set; }

        public string Configuration { get; set; }

        public string InstanceName { get; set; }

        DistributedCacheOptions IOptions<DistributedCacheOptions>.Value => this;
    }
}