using Microsoft.EntityFrameworkCore;
using WeningerDemoProject.Models;

namespace WeningerDemoProject.Helpers
{
    public static class OrderExtensions
    {
        public static IQueryable<Order> ApplySearch(this IQueryable<Order> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.ToLower();

            return query.Where(o =>
                    EF.Functions.Like(o.Customer.CustomerNumber.ToString(), $"%{search}%") ||
                    EF.Functions.Like(o.Customer.FirstName.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(o.Customer.SecondName.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(o.Title.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(o.Description.ToLower(), $"%{search}%")
            );
        }

        public static IQueryable<Order> ApplySorting(this IQueryable<Order> query, string? sortBy = "date", bool isDescending = false)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            switch (sortBy.ToLower())
            {
                case "firstname":
                    query = isDescending
                        ? query.OrderByDescending(o => o.Customer.FirstName)
                        : query.OrderBy(o => o.Customer.FirstName);
                    break;

                case "secondname" or "name":
                    query = isDescending
                        ? query.OrderByDescending(o => o.Customer.SecondName)
                        : query.OrderBy(o => o.Customer.SecondName);
                    break;

                case "customernumber" or "number":
                    query = isDescending
                        ? query.OrderByDescending(o => o.Customer.CustomerNumber)
                        : query.OrderBy(o => o.Customer.CustomerNumber);
                    break;

                case "date" or "time" or "day":
                    query = isDescending
                        ? query.OrderByDescending(o => o.CreatedOn)
                        : query.OrderBy(o => o.CreatedOn);
                    break;

                default:
                    query = isDescending
                        ? query.OrderByDescending(o => o.CreatedOn)
                        : query.OrderBy(o => o.CreatedOn);
                    break;
            }
            return query;
        }
    }
}
