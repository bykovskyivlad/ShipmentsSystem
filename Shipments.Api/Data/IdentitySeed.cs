using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Shipments.Api.Models;

namespace Shipments.Api.Data
{
    public class IdentitySeed
    {
        private static readonly string[] Roles =
        {
            "Client",
            "Courier",
            "Admin"
        };

        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            var email = config["AdminSeed:Email"];
            var password = config["AdminSeed:Password"];

            
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var admin = await userManager.FindByEmailAsync(email);
            if (admin != null)
                return;

            admin = new AppUser
            {
                UserName = email,
                Email = email,
                MustChangePassword = true 
            };

            await userManager.CreateAsync(admin, password);
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
