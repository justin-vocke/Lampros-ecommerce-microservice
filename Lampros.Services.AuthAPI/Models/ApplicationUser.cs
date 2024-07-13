using Microsoft.AspNetCore.Identity;

namespace Lampros.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
