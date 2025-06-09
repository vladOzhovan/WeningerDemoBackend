using WeningerDemoProject.Dtos.Order;

namespace WeningerDemoProject.Interfaces
{
    public interface IOrderActionService
    {
        Task<OrderDto> TakeOrderAsync(int orderId, string userId);
        Task<OrderDto> ReleaseOrderAsync(int orderId, string userId);
        Task<OrderDto> CompleteOrderAsync(int orderId, string userId);
        Task<OrderDto> CancelOrderAsync(int orderId, string userId);
    }
}
