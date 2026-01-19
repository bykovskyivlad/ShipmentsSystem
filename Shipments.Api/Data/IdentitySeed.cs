using Microsoft.AspNetCore.Identity;

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
    }
}
