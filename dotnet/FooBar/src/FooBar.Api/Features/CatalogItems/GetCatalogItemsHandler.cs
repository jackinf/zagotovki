using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FooBar.Api.ViewModels;
using FooBar.Domain.Interfaces;
using FooBar.Domain.Specifications;
using MediatR;

namespace FooBar.Api.Features.CatalogItems
{
    public class GetCatalogItemsHandler : IRequestHandler<GetCatalogItems, IEnumerable<CatalogItemViewModel>>
    {
        private readonly ICatalogItemRepository catalogItemRepository;

        public GetCatalogItemsHandler(ICatalogItemRepository catalogItemRepository)
        {
            this.catalogItemRepository = catalogItemRepository;
        }
        
        public async Task<IEnumerable<CatalogItemViewModel>> Handle(GetCatalogItems request, CancellationToken cancellationToken)
        {
            var specification = new CatalogItemsSpecification(request.ItemsPage * request.PageIndex, request.ItemsPage);
            var catalogItems = await catalogItemRepository.ListAsync(specification);
            return catalogItems.Select(x => new CatalogItemViewModel
            {
                // TODO: define mapping
            });
        }
    }
}