using CloudObjects.App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using CloudObjects.App.Data;

namespace CloudObjects.App.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTokenGenerator(this IServiceCollection services, string jwtSecret)
        {
            services.AddScoped((sp) =>
            {
                var dbContext = sp.GetRequiredService<CloudObjectsDbContext>();
                return new TokenGenerator(jwtSecret, dbContext);
            });
        }

        public static void AddHttpContext(this IServiceCollection services)
        {
            services.AddScoped((sp) =>
            {
                var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                return contextAccessor.HttpContext;
            });
        }

        /// <summary>
        /// help from https://www.thecodebuzz.com/jwt-authorization-token-swagger-open-api-asp-net-core-3-0/
        /// </summary>
        public static void AddCloudObjectsAuthentication(this IServiceCollection services, string jwtSecret)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    [new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }] = new string[] { }
                });

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CloudObjects API", Version = "v1" });
            });
        }
    }
}
