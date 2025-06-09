using WeningerDemoProject.Dtos.Order;
using WeningerDemoProject.Helpers;
using WeningerDemoProject.Models;

namespace WeningerDemoProject.Interfaces
{
    public interface IOrderRepository
    {
        Task SaveChangesAsync();
        Task<Order> CreateAsync(Order order);
        Task<Order?> UpdateAsync(int id, UpdateOrderDto orderDto);
        Task<Order?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto orderDto);
        Task<Order?> DeleteAsync(int id);
        Task<bool> DeleteMultipleAsync(List<int> ids);
        Task<List<Order>> GetAllAsync(QueryObject query);
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetByCustomerNumberAsync(int customerNumber);
    }
}
