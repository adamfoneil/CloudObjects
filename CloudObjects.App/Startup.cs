using CloudObjects.App.Extensions;
using CloudObjects.App.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CloudObjects.App.Data;
using CloudObjects.App.Interfaces;
using CloudObjects.App.Services;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CloudObjects.App
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
            // built-in services
            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddMvc(options =>
                {
                    options.Filters.Add(new ExceptionFilter());
                })
                .AddFluentValidation(config =>
                {
                    config.RegisterValidatorsFromAssemblyContaining<Startup>(filter =>
                        filter.InterfaceType.FindInterfaces((type, criteria) => type == typeof(IDtoValidator), null)
                            .Any());
                });

            services.AddHttpContextAccessor();
            services.AddHttpContext();

            // app-specific
            var connectionString = Configuration.GetConnectionString("Default");
            var jwtSecret = Configuration["Jwt:Secret"];

            services.AddDbContextPool<CloudObjectsDbContext>(builder => builder.UseSqlServer(connectionString));
            services.AddTokenGenerator(jwtSecret);
            services.AddCloudObjectsAuthentication(jwtSecret);
            services.AddSwagger();

            services
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IActivityService, ActivityService>()
                .AddScoped<IStoredObjectService, StoredObjectService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapGet("/routes", (context) => ListRoutesAsync(context, endpoints));
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        /// <summary>
        /// for troubleshooting 404 errors that made no sense, idea from
        /// https://github.com/kobake/AspNetCore.RouteAnalyzer/issues/28
        /// </summary>
        private async Task ListRoutesAsync(HttpContext context, IEndpointRouteBuilder endpoints)
        {
            context.Response.ContentType = "application/json";

            var routes = endpoints.DataSources.SelectMany(ds => ds.Endpoints.OfType<RouteEndpoint>().Select(ep => new
            {
                ep.DisplayName,
                ep.RoutePattern.RawText,
                ep.RoutePattern.PathSegments
            }));

            var json = JsonSerializer.Serialize(routes, options: new JsonSerializerOptions()
            {
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);
        }

        /*
        removed from app.UseEndpoints
        endpoints.MapGet("/config", async (context) => await OutputConfigAsync(context, new Dictionary<Func<KeyValuePair<string, string>, bool>, Func<string, string>>
        {
            [(kp) => ConnectionString.IsSensitive(kp.Value, out _)] = (value) => ConnectionString.Redact(value),
            [(kp) => kp.Key.Contains("Secret")] = (value) => "&lt;redacted&gt;"
        }));
        */

        /// <summary>
        /// this was for some Azure troubleshooting. Not needed now, but left in for study
        /// </summary>
        private async Task OutputConfigAsync(HttpContext context, Dictionary<Func<KeyValuePair<string, string>, bool> , Func<string, string>> redactions = null)
        {                        
            await context.Response.WriteAsync(
                @"<html><head>
                    <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css""/>
                </head><body class=""container"">");

            await context.Response.WriteAsync("<ul>");

            foreach (var item in Configuration.AsEnumerable().OrderBy(item => item.Key))
            {
                var output = (IsRedacted(item, out Func<string, string> transform)) ?
                    transform.Invoke(item.Value) :
                    item.Value;

                await context.Response.WriteAsync($"<li>{item.Key} = {output}</li>\r\n");
            }

            await context.Response.WriteAsync("</ul></body></html>");            

            bool IsRedacted(KeyValuePair<string, string> setting, out Func<string, string> transform)
            {
                foreach (var rule in redactions)
                {
                    if (rule.Key.Invoke(setting))
                    {
                        transform = redactions[rule.Key];
                        return true;
                    }    
                }

                transform = null;
                return false;
            }
        }
    }    
}
