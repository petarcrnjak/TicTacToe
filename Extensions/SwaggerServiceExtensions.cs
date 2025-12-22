using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using TicTacToe.Enums;

namespace TicTacToe.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TicTacToe API", Version = "v1" });

                // Map GameStatus enum to string values in Swagger (UI will show "Open", "InProgress", "Finished")
                c.MapType<GameStatus>(() => new OpenApiSchema
                {
                    Type = "string",
                    Enum = Enum.GetNames<GameStatus>()
                        .Select(n => new OpenApiString(n) as IOpenApiAny)
                        .ToList()
                });

                // JWT Bearer auth
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}