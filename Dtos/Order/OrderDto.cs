using WeningerDemoProject.Models;

namespace WeningerDemoProject.Dtos.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? CustomerFullName { get; set; } = string.Empty;
        public string Status { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public int CustomerId { get; set; }
        public int CustomerNumber { get; set; }
        public string? TakenByUserId { get; set; } = null;
        public bool IsTaken { get; set; } = false;
    }
}
