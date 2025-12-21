using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Authorization;
using TicTacToe.Database;

namespace TicTacToe.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddAppIdentity(this IServiceCollection services)
        {
            services
                .AddIdentityCore<AppUser>(options =>
                {
                    options.User.RequireUniqueEmail = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}