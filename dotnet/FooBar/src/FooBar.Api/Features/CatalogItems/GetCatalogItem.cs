using FooBar.Api.ViewModels;
using MediatR;

namespace FooBar.Api.Features.CatalogItems
{
    public class GetCatalogItem : IRequest<CatalogItemViewModel>
    {
        public int Id { get; }

        public GetCatalogItem(int id)
        {
            Id = id;
        }
    }
}