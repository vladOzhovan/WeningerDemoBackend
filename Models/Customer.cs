using Microsoft.EntityFrameworkCore;

namespace WeningerDemoProject.Models
{
    [Index(nameof(CustomerNumber), IsUnique = true)]
    public class Customer
    {
        public int Id { get; set; }
        public int CustomerNumber { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public OrderStatus OverallStatus
        {
            get
            {
                if (Orders == null || Orders.Count == 0)
                    return OrderStatus.NoOrders;

                if (Orders.All(o => o.Status == OrderStatus.Completed))
                    return OrderStatus.Completed;

                if (Orders.All(o => o.Status == OrderStatus.Canceled))
                    return OrderStatus.Canceled;

                if (Orders.Any(o => o.Status == OrderStatus.InProgress))
                    return OrderStatus.InProgress;

                if (Orders.All(o => o.Status == OrderStatus.Pending)) 
                    return OrderStatus.Pending;

                return OrderStatus.Pending;
            }
        }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
