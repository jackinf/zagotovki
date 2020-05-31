using System.Collections.Generic;
using FooBar.Api.ViewModels;
using MediatR;

namespace FooBar.Api.Features.CatalogItems
{
    public class GetCatalogItems : IRequest<IEnumerable<CatalogItemViewModel>>
    {
        public int ItemsPage { get; }
        public int PageIndex { get; }

        public GetCatalogItems(int itemsPage, int pageIndex)
        {
            ItemsPage = itemsPage;
            PageIndex = pageIndex;
        }
    }
}