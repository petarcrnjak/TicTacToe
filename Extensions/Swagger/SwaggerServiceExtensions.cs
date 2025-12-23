using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TicTacToe.Enums;

namespace TicTacToe.Extensions.Swagger
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TicTacToe API",
                    Version = "v1",
                    Description = "API for playing TicTacToe: register and authenticate users, create and manage games.",
                    Contact = new OpenApiContact
                    {
                        Name = "TicTacToe Maintainer",
                        Url = new Uri("https://github.com/petarcrnjak")
                    }
                });

                // include XML comments (enable XML documentation file generation in the project file)
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

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

                // Add tag descriptions for controllers (e.g. Auth)
                c.DocumentFilter<SwaggerTagDocumentFilter>();
            });

            return services;
        }
    }
}