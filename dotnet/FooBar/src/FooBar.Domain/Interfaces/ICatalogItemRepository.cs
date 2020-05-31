using FooBar.Domain.Entities;

namespace FooBar.Domain.Interfaces
{
    public interface ICatalogItemRepository: IAsyncRepository<CatalogItem>
    {
        
    }
}