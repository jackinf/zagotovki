using MediatR;

namespace FooBar.Api.Features.V1.CatalogItems.Delete
{
    public class DeleteCatalogItem : IRequest
    {
        public int Id { get; }

        public DeleteCatalogItem(int id)
        {
            Id = id;
        }
    }
}