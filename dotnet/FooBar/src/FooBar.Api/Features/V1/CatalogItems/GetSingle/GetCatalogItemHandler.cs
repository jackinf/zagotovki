using System.Threading;
using System.Threading.Tasks;
using FooBar.Api.ViewModels;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Interfaces;
using MediatR;

namespace FooBar.Api.Features.V1.CatalogItems.GetSingle
{
    public class GetCatalogItemHandler : IRequestHandler<GetCatalogItem, CatalogItemViewModel>
    {
        private readonly ICatalogItemRepository _catalogItemRepository;

        public GetCatalogItemHandler(ICatalogItemRepository catalogItemRepository)
        {
            _catalogItemRepository = catalogItemRepository;
        }
        
        public async Task<CatalogItemViewModel> Handle(GetCatalogItem request, CancellationToken cancellationToken)
        {
            var catalogItem = await _catalogItemRepository.GetByIdAsync(request.Id);
            if (catalogItem == null)
            {
                throw new ItemNotFoundException($"Catalog item with {request.Id} was not found");
            }
            
            return new CatalogItemViewModel
            {
                Id = catalogItem.Id,
                Name = catalogItem.Name,
                Price = catalogItem.Price,
                PictureUri = catalogItem.PictureUri
            };
        }
    }
}