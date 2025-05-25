using Microsoft.EntityFrameworkCore;
using WeningerDemoProject.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WeningerDemoProject.Helpers
{
    public static class CustomerExtensions
    {
        public static IQueryable<Customer> ApplySearch(this IQueryable<Customer> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.ToLower();

            return query.Where(c => 
                EF.Functions.Like(c.CustomerNumber.ToString(), $"%{search}%") ||
                EF.Functions.Like(c.FirstName.ToLower(), $"%{search}%") ||
                EF.Functions.Like(c.SecondName.ToLower(), $"%{search}%") ||
                EF.Functions.Like(c.SecondName.ToLower(), $"%{search}%")
            );
        }

        public static IQueryable<Customer> ApplySorting(this IQueryable<Customer> query, bool isDescending, string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            switch (sortBy.ToLower())
            {
                case "firstname":
                    query = isDescending
                        ? query.OrderByDescending(c => c.FirstName)
                        : query.OrderBy(c => c.FirstName);
                    break;

                case "secondname" or "name":
                    query = isDescending
                        ? query.OrderByDescending(c => c.SecondName)
                        : query.OrderBy(c => c.SecondName);
                    break;

                case "customernumber" or "number":
                    query = isDescending
                        ? query.OrderByDescending(c => c.CustomerNumber)
                        : query.OrderBy(c => c.CustomerNumber);
                    break;

                case "date" or "time":
                    query = isDescending
                        ? query.OrderByDescending(c => c.CreatedOn)
                        : query.OrderBy(c => c.CreatedOn);
                    break;

                default:
                    query = isDescending
                        ? query.OrderByDescending(c => c.CreatedOn)
                        : query.OrderBy(c => c.CreatedOn);
                    break;
            }

            return query;
        }
    }
}
