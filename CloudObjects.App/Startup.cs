using CloudObjects.App.Extensions;
using Dapper.CX.SqlServer.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            services.AddControllers();
            services.AddMvc();
            services.AddHttpContextAccessor();
            services.AddHttpContext();

            // app-specific
            var connectionString = Configuration.GetConnectionString("Default");
            var jwtSecret = Configuration["Jwt:Secret"];

            services.AddDapperCX(connectionString, (id) => Convert.ToInt64(id));
            services.AddTokenGenerator(jwtSecret);
            services.AddCloudObjectsAuthentication(jwtSecret);
            services.AddSwagger();
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
            app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                /*
                endpoints.MapGet("/config", async (context) => await OutputConfigAsync(context, new Dictionary<Func<KeyValuePair<string, string>, bool>, Func<string, string>>
                {
                    [(kp) => ConnectionString.IsSensitive(kp.Value, out _)] = (value) => ConnectionString.Redact(value),
                    [(kp) => kp.Key.Contains("Secret")] = (value) => "&lt;redacted&gt;"
                }));
                */
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }        

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
