using System;
using FluentValidation;
using FooBar.Api.Features;
using FooBar.Domain.Entities;
using FooBar.Domain.Interfaces;
using FooBar.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FooBar.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            ConfigurePersistence(services);
            services.AddServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FooBar API V1");
                c.RoutePrefix = string.Empty;
            });
        }
        
        private void ConfigurePersistence(IServiceCollection services)
        {
            services.AddDbContext<CatalogContext>(c =>
            {
                var connectionString = Configuration.GetConnectionString("CatalogConnectionSqlite")
                    .Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory);
                c.UseSqlite(connectionString);
            });
        }
    }
}