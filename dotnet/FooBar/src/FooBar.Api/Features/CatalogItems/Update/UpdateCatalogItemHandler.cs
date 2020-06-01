using System.Threading;
using System.Threading.Tasks;
using FooBar.Domain.Interfaces;
using MediatR;

namespace FooBar.Api.Features.CatalogItems.Update
{
    public class UpdateCatalogItemHandler : IRequestHandler<UpdateCatalogItem>
    {
        private readonly ICatalogItemRepository catalogItemRepository;

        public UpdateCatalogItemHandler(ICatalogItemRepository catalogItemRepository)
        {
            this.catalogItemRepository = catalogItemRepository;
        }
        
        public async Task<Unit> Handle(UpdateCatalogItem request, CancellationToken cancellationToken)
        {
            var catalogItem = await catalogItemRepository.GetByIdAsync(request.Id);
            catalogItem.Update(request.Name, request.Price);
            
            await catalogItemRepository.UpdateAsync(catalogItem);
            
            return Unit.Value;
        }
    }
}