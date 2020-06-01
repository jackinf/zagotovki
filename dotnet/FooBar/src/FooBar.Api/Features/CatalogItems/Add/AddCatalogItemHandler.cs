using System;
using System.Threading;
using System.Threading.Tasks;
using FooBar.Domain.Entities;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Interfaces;
using MediatR;

namespace FooBar.Api.Features.CatalogItems.Add
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
            // TODO: validate if catalog type and catalog brand exist
            
            var catalogItem = new CatalogItem(
                request.CatalogTypeId, 
                request.CatalogBrandId, 
                request.Description, 
                request.Name, 
                request.Price,
                request.PictureUri);

            try
            {
                await catalogItemRepository.AddAsync(catalogItem);

                return Unit.Value;
            }
            catch (Exception e)
            {
                throw new GeneralValidationException($"Failed to add an item");
            }
        }
    }
}