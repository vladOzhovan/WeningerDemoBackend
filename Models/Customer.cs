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
        public OrderStatus OverallStatus
        {
            get
            {
                if (Orders.All(o => o.Status == OrderStatus.Completed))
                    return OrderStatus.Completed;

                if (Orders.All(o => o.Status == OrderStatus.Canceled))
                    return OrderStatus.Canceled;

                else if (Orders.Any(o => o.Status == OrderStatus.InProgress))
                    return OrderStatus.InProgress;

                else if (Orders.All(o => o.Status == OrderStatus.Pending)) 
                    return OrderStatus.Pending;

                else return OrderStatus.Pending;
            }
        }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
