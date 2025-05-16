using System.ComponentModel.DataAnnotations;

namespace WeningerDemoProject.Dtos.Account
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;
        [Required]
        [MinLength(5)]
        public string NewPassword { get; set; } = null!;
    }
}
