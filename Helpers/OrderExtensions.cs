using WeningerDemoProject.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WeningerDemoProject.Helpers
{
    public static class OrderExtensions
    {
        public static IQueryable<Order> ApplySearch(this IQueryable<Order> orders, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return orders;

            search = search.ToLower();

            return orders.Where(o =>
                    o.Customer.CustomerNumber.ToString().Contains(search) ||
                    o.Customer.FirstName.ToLower().Contains(search) ||
                    o.Customer.SecondName.ToLower().Contains(search) ||
                    o.Title.ToLower().Contains(search) ||
                    o.Description.ToLower().Contains(search)
            );
        }

        public static IQueryable<Order> ApplySorting(this IQueryable<Order> orders, bool isDescending, string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return orders;

            switch (sortBy.ToLower())
            {
                case "firstname":
                    orders = isDescending
                        ? orders.OrderByDescending(o => o.Customer.FirstName)
                        : orders.OrderBy(o => o.Customer.FirstName);
                    break;

                case "secondname" or "name":
                    orders = isDescending
                        ? orders.OrderByDescending(o => o.Customer.SecondName)
                        : orders.OrderBy(o => o.Customer.SecondName);
                    break;

                case "customernumber" or "number":
                    orders = isDescending
                        ? orders.OrderByDescending(o => o.Customer.CustomerNumber)
                        : orders.OrderBy(o => o.Customer.CustomerNumber);
                    break;

                case "date" or "time":
                    orders = isDescending
                        ? orders.OrderByDescending(o => o.CreatedOn)
                        : orders.OrderBy(o => o.CreatedOn);
                    break;

                default:
                    orders = isDescending
                        ? orders.OrderByDescending(o => o.CreatedOn)
                        : orders.OrderBy(o => o.CreatedOn);
                    break;
            }

            return orders;
        }
    }
}
