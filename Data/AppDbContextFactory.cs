using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WeningerDemoProject.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Local connection string
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=weninger_local;Username=postgres;Password=your_password");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
