using System;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using FooBar.Api.Behaviors;
using FooBar.Domain.Entities;
using FooBar.Domain.Interfaces;
using FooBar.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
            services.AddRouting(options => options.LowercaseUrls = true); // TODO: check this
            services.AddCors(options => options.AddPolicy(new ApiOptions().CorsPolicy, policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
            
            // Required for the Swagger to recognize API versions
            services.AddVersionedApiExplorer(options =>
            {
                options.SubstituteApiVersionInUrl = true;
                options.GroupNameFormat = options.SubstitutionFormat;
            });
            
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "FooBar API", Version = "v1" }));
        }

        public static void UseApi(this IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FooBar API V1");
                c.RoutePrefix = string.Empty;
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