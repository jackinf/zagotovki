using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FooBar.Domain.Interfaces;
using FooBar.Domain.Specifications;
using MediatR;

namespace FooBar.Api.Features.V1.CatalogItems.GetList
{
    public class GetCatalogItemsHandler : IRequestHandler<GetCatalogItems, IEnumerable<CatalogItemViewModel>>
    {
        private readonly ICatalogItemRepository _catalogItemRepository;

        public GetCatalogItemsHandler(ICatalogItemRepository catalogItemRepository)
        {
            _catalogItemRepository = catalogItemRepository;
        }
        
        public async Task<IEnumerable<CatalogItemViewModel>> Handle(GetCatalogItems request, CancellationToken cancellationToken)
        {
            var specification = new CatalogItemsSpecification(request.ItemsPage * request.PageIndex, request.ItemsPage);
            var catalogItems = await _catalogItemRepository.ListAsync(specification);
            
            return catalogItems.Select(model => new CatalogItemViewModel
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                PictureUri = model.PictureUri
            });
        }
    }
}