using WeningerDemoProject.Dtos.Order;
using WeningerDemoProject.Models;

namespace WeningerDemoProject.Mappers
{
    public static class OrderMapper
    {
        public static OrderDto ToOrderDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                Title = order.Title,
                Description = order.Description,
                CustomerId = order.CustomerId,
                CustomerNumber = order.CustomerNumber,
                Status = order.Status.ToString(),
                CreatedOn = order.CreatedOn,
                TakenByUserId = order.TakenByUserId,
                IsTaken = !string.IsNullOrEmpty(order.TakenByUserId)
            };
        }

        public static Order ToOrderFromCreateDto(this CreateOrderDto dto, int customerNumber, int customerId)
        {
            return new Order
            {
                CustomerId = customerId,
                Title = dto.Title,
                Description = dto.Description,
                CustomerNumber = customerNumber,
                Status = dto.Status,
                CreatedOn = DateTime.Now
            };
        }
    }
}
