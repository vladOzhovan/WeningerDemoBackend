using Microsoft.AspNetCore.Identity;

namespace WeningerDemoProject.Models
{
    public class AppUser : IdentityUser
    {
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
