namespace WeningerDemoProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public int CustomerId { get; set; }
        public int CustomerNumber { get; set; }
        public Customer Customer { get; set; } = null!;
        public string? TakenByUserId { get; set; } = null;
        public AppUser? TakenByUser { get; set; } = null;
        
        public void Take(string userId)
        {
            TakenByUserId = userId;
            Status = OrderStatus.InProgress;
        }

        public void Release()
        {
            TakenByUserId = null;
            Status = OrderStatus.Pending;
        }

        public void Complete()
        {
            TakenByUserId = null;
            Status = OrderStatus.Completed;
        }

        public void Cancle()
        {
            TakenByUserId = null;
            Status = OrderStatus.Canceled;
        }
    }
}
