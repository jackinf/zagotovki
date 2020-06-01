using FooBar.Api.ViewModels;
using MediatR;

namespace FooBar.Api.Features.CatalogItems.GetSingle
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