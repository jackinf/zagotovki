using MediatR;

namespace FooBar.Api.Features.CatalogItems.Delete
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