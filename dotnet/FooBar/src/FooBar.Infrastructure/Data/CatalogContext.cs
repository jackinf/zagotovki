using FooBar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FooBar.Infrastructure.Data
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options)
            : base(options)
        {
        }

        public DbSet<CatalogItem> CatalogItems { get; set; }
        
        // protected override void OnConfiguring(DbContextOptionsBuilder options)
        //     => options.UseSqlite("DataSource=foobar.db");
    }
}
