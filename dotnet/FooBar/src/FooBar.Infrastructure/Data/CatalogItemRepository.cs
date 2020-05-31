using FooBar.Domain.Entities;
using FooBar.Domain.Interfaces;

namespace FooBar.Infrastructure.Data
{
    public class CatalogItemRepository : EfRepository<CatalogItem>, ICatalogItemRepository
    {
        public CatalogItemRepository(CatalogContext dbContext) : base(dbContext)
        {
        }
    }
}