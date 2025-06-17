using System.ComponentModel.DataAnnotations;

namespace WeningerDemoProject.Dtos.Invitation
{
    public class CreateInvitationDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public TimeSpan ValidTime { get; set; } = TimeSpan.FromDays(1);
    }
}
