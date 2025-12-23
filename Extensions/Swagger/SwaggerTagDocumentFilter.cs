using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TicTacToe.Extensions.Swagger
{
    public class SwaggerTagDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc.Tags == null)
                swaggerDoc.Tags = new List<OpenApiTag>();

            swaggerDoc.Tags.Add(new OpenApiTag
            {
                Name = "Auth",
                Description = "Authentication endpoints: user registration and login (returns JWT token)."
            });

            swaggerDoc.Tags.Add(new OpenApiTag
            {
                Name = "Games",
                Description = "Game endpoints: create, list, join and make moves."
            });

            swaggerDoc.Tags.Add(new OpenApiTag
            {
                Name = "Users",
                Description = "User profile endpoints: retrieve player stats and profile information."
            });
        }
    }
}