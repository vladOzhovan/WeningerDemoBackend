using System.ComponentModel.DataAnnotations;

namespace WeningerDemoProject.Dtos.Account
{
    public class RegisterDto
    {
        [Required]
        public Guid Token { get; set; }

        [Required, StringLength(30, MinimumLength = 3)]
        public string? UserName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required, MinLength(7)]
        public string? Password { get; set; }
    }
}
