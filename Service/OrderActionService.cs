using WeningerDemoProject.Dtos.Order;
using WeningerDemoProject.Interfaces;
using WeningerDemoProject.Mappers;
using WeningerDemoProject.Models;

namespace WeningerDemoProject.Service
{
    public class OrderActionService : IOrderActionService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderActionService(IOrderRepository orderRepo)
        {
            _orderRepository = orderRepo;
        }

        private async Task<Order> GetOrderOrThrow(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");
            return order;
        }

        public async Task<OrderDto> TakeOrderAsync(int orderId, string userId)
        {
            var order = await GetOrderOrThrow(orderId);

            if (order.TakenByUserId != null)
                throw new InvalidOperationException("Order is already taken");

            order.Take(userId);
            await _orderRepository.SaveChangesAsync();
            return order.ToOrderDto();
        }

        public async Task<OrderDto> ReleaseOrderAsync(int orderId, string userId)
        {
            var order = await GetOrderOrThrow(orderId);

            if (order.TakenByUserId != userId)
                throw new UnauthorizedAccessException("You can only release your own orders");

            order.Release();
            await _orderRepository.SaveChangesAsync();
            return order.ToOrderDto();
        }

        public async Task<OrderDto> CompleteOrderAsync(int orderId, string userId)
        {
            var order = await GetOrderOrThrow(orderId);

            if (order.TakenByUserId != userId)
                throw new UnauthorizedAccessException("You can only complete your own orders");

            if (order.Status == OrderStatus.Completed)
                throw new InvalidOperationException("Order is already completed");

            order.Complete();
            await _orderRepository.SaveChangesAsync();
            return order.ToOrderDto();
        }

        public async Task<OrderDto> CancelOrderAsync(int orderId, string userId)
        {
            var order = await GetOrderOrThrow(orderId);

            if (order.TakenByUserId != userId)
                throw new UnauthorizedAccessException("You can only cancel your own orders");

            if (order.Status == OrderStatus.Canceled)
                throw new InvalidOperationException($"Order {userId} is already canceled");

            order.Cancel();
            await _orderRepository.SaveChangesAsync();
            return order.ToOrderDto();
        }
    }
}
