using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

public class NWContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public void AddProduct(Product product)
    {
        this.Products.Add(product);
        this.SaveChanges();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration =  new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");

        var config = configuration.Build();
        optionsBuilder.UseSqlServer(@config["NWConsole:ConnectionString"]);
    }
}