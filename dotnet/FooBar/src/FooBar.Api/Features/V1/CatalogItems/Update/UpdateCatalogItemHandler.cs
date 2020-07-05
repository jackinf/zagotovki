using System;
using System.Threading;
using System.Threading.Tasks;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Interfaces;
using MediatR;

namespace FooBar.Api.Features.V1.CatalogItems.Update
{
    public class UpdateCatalogItemHandler : IRequestHandler<UpdateCatalogItem>
    {
        private readonly ICatalogItemRepository _catalogItemRepository;

        public UpdateCatalogItemHandler(ICatalogItemRepository catalogItemRepository)
        {
            _catalogItemRepository = catalogItemRepository;
        }
        
        public async Task<Unit> Handle(UpdateCatalogItem request, CancellationToken cancellationToken)
        {
            var catalogItem = await _catalogItemRepository.GetByIdAsync(request.Id);
            if (catalogItem == null)
            {
                throw new ItemNotFoundException($"Catalog item with {request.Id} was not found");
            }
            
            catalogItem.Update(request.Name, request.Price);

            try
            {
                await _catalogItemRepository.UpdateAsync(catalogItem);
                return Unit.Value;
            }
            catch (Exception)
            {
                throw new GeneralException($"Failed to update an item");
            }
        }
    }
}