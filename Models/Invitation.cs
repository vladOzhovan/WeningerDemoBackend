namespace WeningerDemoProject.Models
{
    public class Invitation
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed => UsedAt.HasValue;
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}
