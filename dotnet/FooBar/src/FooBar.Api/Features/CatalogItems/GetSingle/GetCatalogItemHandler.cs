using System.Threading;
using System.Threading.Tasks;
using FooBar.Api.ViewModels;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Interfaces;
using MediatR;

namespace FooBar.Api.Features.CatalogItems.GetSingle
{
    public class GetCatalogItemHandler : IRequestHandler<GetCatalogItem, CatalogItemViewModel>
    {
        private readonly ICatalogItemRepository catalogItemRepository;

        public GetCatalogItemHandler(ICatalogItemRepository catalogItemRepository)
        {
            this.catalogItemRepository = catalogItemRepository;
        }
        
        public async Task<CatalogItemViewModel> Handle(GetCatalogItem request, CancellationToken cancellationToken)
        {
            var catalogItem = await catalogItemRepository.GetByIdAsync(request.Id);
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