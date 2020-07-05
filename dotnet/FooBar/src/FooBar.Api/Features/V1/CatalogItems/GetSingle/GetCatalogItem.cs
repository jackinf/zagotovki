using MediatR;

namespace FooBar.Api.Features.V1.CatalogItems.GetSingle
{
    /// <summary>
    /// Handled by <see cref="GetCatalogItemHandler"/>
    /// </summary>
    public class GetCatalogItem : IRequest<CatalogItemViewModel>
    {
        public int Id { get; }

        public GetCatalogItem(int id)
        {
            Id = id;
        }
    }
}