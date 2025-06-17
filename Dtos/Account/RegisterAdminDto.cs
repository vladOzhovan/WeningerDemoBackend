using System.ComponentModel.DataAnnotations;

namespace WeningerDemoProject.Dtos.Account
{
    public class RegisterAdminDto
    {
        [Required, StringLength(30, MinimumLength = 3)]
        public string? UserName { get; set; }

        [Required, MinLength(7)]
        public string? Password { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
