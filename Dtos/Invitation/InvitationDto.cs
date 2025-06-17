﻿namespace WeningerDemoProject.Dtos.Invitation
{
    public class InvitationDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
