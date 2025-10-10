using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data;

public class DesignTimeFactory : IDesignTimeDbContextFactory<TPIContext>
{
    public TPIContext CreateDbContext(string[] args)
    {
        var cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true).Build();
        var opts = new DbContextOptionsBuilder<TPIContext>()
            .UseSqlServer(cfg.GetConnectionString("DefaultConnection")).Options;
        return new TPIContext(opts);
    }
}
