using System.ComponentModel.DataAnnotations;

namespace WeningerDemoProject.Dtos.Invitation
{
    public class CreateInvitationDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        [Range(1, 365, ErrorMessage = "ValidDays must be between 1 and 365")]
        public int ValidDays { get; set; } = 1;
    }
}
