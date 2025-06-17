using Microsoft.EntityFrameworkCore;
using WeningerDemoProject.Data;
using WeningerDemoProject.Dtos.Order;
using WeningerDemoProject.Helpers;
using WeningerDemoProject.Interfaces;
using WeningerDemoProject.Models;

namespace WeningerDemoProject.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync(QueryObject query)
        {
            var orders = _context.Orders.Include(o => o.Customer).ThenInclude(c => c.Address).AsQueryable();
            
            orders = orders.ApplySearch(query.Search);
            orders = orders.ApplySorting(query.SortBy, query.IsDescending);
            return await orders.ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.Customer).ThenInclude(c => c.Address).
                FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetByCustomerNumberAsync(int customerNumber)
        {
            return await _context.Orders.Include(o => o.Customer).
                Where(o => o.Customer.CustomerNumber == customerNumber).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Order> CreateAsync(Order orderModel)
        {
            await _context.Orders.AddAsync(orderModel);
            await _context.SaveChangesAsync();
            return orderModel;
        }

        public async Task<Order?> DeleteAsync(int id)
        {
            var orderInDb = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (orderInDb == null)
                return null;

            _context.Orders.Remove(orderInDb);
            await _context.SaveChangesAsync();
            return orderInDb;
        }

        public async Task<bool> DeleteMultipleAsync(List<int> ids)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var id in ids)
                {
                    var order = await _context.Orders.FindAsync(id);

                    if (order == null)
                        throw new KeyNotFoundException($"Order with id {id} not found");
                    
                    _context.Orders.Remove(order);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<Order?> UpdateAsync(int id, UpdateOrderDto orderDto)
        {
            var orderInDb = await _context.Orders.Include(o => o.Customer).ThenInclude(c => c.Address).
                FirstOrDefaultAsync(o => o.Id == id);

            if (orderInDb == null)
                return null;

            orderInDb.Title = orderDto.Title;
            orderInDb.Description = orderDto.Description;

            await _context.SaveChangesAsync();
            return orderInDb;
        }

        public async Task<Order?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto orderStatusDto)
        {
            var orderInDb = await _context.Orders.FindAsync(id);

            if (orderInDb == null)
                return null;

            orderInDb.Status = orderStatusDto.Status;
            await _context.SaveChangesAsync();
            return orderInDb;
        }
    }
}
