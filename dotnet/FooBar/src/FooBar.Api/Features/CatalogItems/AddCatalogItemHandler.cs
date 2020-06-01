using System.Threading;
using System.Threading.Tasks;
using FooBar.Domain.Entities;
using FooBar.Domain.Interfaces;
using MediatR;

namespace FooBar.Api.Features.CatalogItems
{
    public class AddCatalogItemHandler : IRequestHandler<AddCatalogItem>
    {
        private readonly ICatalogItemRepository catalogItemRepository;

        public AddCatalogItemHandler(ICatalogItemRepository catalogItemRepository)
        {
            this.catalogItemRepository = catalogItemRepository;
        }
        
        public async Task<Unit> Handle(AddCatalogItem request, CancellationToken cancellationToken)
        {
            var catalogItem = new CatalogItem(
                request.CatalogTypeId, 
                request.CatalogBrandId, 
                request.Description, 
                request.Name, 
                request.Price,
                request.PictureUri);
            
            await catalogItemRepository.AddAsync(catalogItem);

            return Unit.Value;
        }
    }
}