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

        public async Task<Order?> UpdateAsync(int id, UpdateOrderDto orderDto)
        {
            var orderInDb = await _context.Orders.Include(o => o.Customer).FirstOrDefaultAsync(o => o.Id == id);

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

        public async Task<List<Order>> GetAllAsync(QueryObject query)
        {
            var orders = _context.Orders.Include(o => o.Customer).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CustomerNumber))
                orders = orders.Where(o => 
                    EF.Functions.Like(o.Customer.CustomerNumber.ToString(), $"%{query.CustomerNumber}%"));

            if (!string.IsNullOrWhiteSpace(query.FirstName))
                orders = orders.Where(o => EF.Functions.Like(o.Customer.FirstName, $"%{query.FirstName}%"));

            if (!string.IsNullOrWhiteSpace(query.SecondName))
                orders = orders.Where(o => EF.Functions.Like(o.Customer.SecondName, $"%{query.SecondName}%"));

            if (!string.IsNullOrWhiteSpace(query.Title))
                orders = orders.Where(o => EF.Functions.Like(o.Title, $"%{query.Title}%"));

            if (!string.IsNullOrWhiteSpace(query.Description))
                orders = orders.Where(o => EF.Functions.Like(o.Description, $"%{query.Description}%"));

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "firstname":
                        orders = query.IsDescending
                            ? orders.OrderByDescending(o => o.Customer.FirstName)
                            : orders.OrderBy(o => o.Customer.FirstName);
                        break;

                    case "secondname" or "name":
                        orders = query.IsDescending
                            ? orders.OrderByDescending(o => o.Customer.SecondName)
                            : orders.OrderBy(o => o.Customer.SecondName);
                        break;

                    case "customernumber" or "number":
                        orders = query.IsDescending
                            ? orders.OrderByDescending(o => o.Customer.CustomerNumber)
                            : orders.OrderBy(o => o.Customer.CustomerNumber);
                        break;

                    case "date" or "time":
                        orders = query.IsDescending
                            ? orders.OrderByDescending(o => o.CreatedOn)
                            : orders.OrderBy(o => o.CreatedOn);
                        break;

                    default:
                        orders = query.IsDescending
                            ? orders.OrderByDescending(o => o.CreatedOn)
                            : orders.OrderBy(o => o.CreatedOn);
                        break;
                }
            }
            
            return await orders.ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.Customer).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetByCustomerNumberAsync(int customerNumber)
        {
            return await _context.Orders.Include(o => o.Customer).Where(o => o.Customer.CustomerNumber == customerNumber).ToListAsync();
        }
    }
}
