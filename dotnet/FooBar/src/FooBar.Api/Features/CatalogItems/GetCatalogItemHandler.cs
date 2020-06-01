using System.Threading;
using System.Threading.Tasks;
using FooBar.Api.ViewModels;
using FooBar.Domain.Interfaces;
using MediatR;

namespace FooBar.Api.Features.CatalogItems
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