using Microsoft.AspNetCore.Identity;

namespace Shipments.Api.Models
{
    public class AppUser : IdentityUser
    {
        public bool MustChangePassword { get; set; } = false;
    }
}
