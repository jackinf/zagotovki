using System;
using System.Threading;
using System.Threading.Tasks;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Interfaces;
using MediatR;

namespace FooBar.Api.Features.V1.CatalogItems.Delete
{
    public class DeleteCatalogItemHandler : IRequestHandler<DeleteCatalogItem>
    {
        private readonly ICatalogItemRepository catalogItemRepository;

        public DeleteCatalogItemHandler(ICatalogItemRepository catalogItemRepository)
        {
            this.catalogItemRepository = catalogItemRepository;
        }
        
        public async Task<Unit> Handle(DeleteCatalogItem request, CancellationToken cancellationToken)
        {
            var catalogItem = await catalogItemRepository.GetByIdAsync(request.Id);
            if (catalogItem == null)
            {
                throw new ItemNotFoundException($"Catalog item with {request.Id} was not found");
            }
            
            try
            {
                await catalogItemRepository.DeleteAsync(catalogItem);
                return Unit.Value;
            }
            catch (Exception)
            {
                throw new GeneralException($"Failed to delete an item");
            }
        }
    }
}