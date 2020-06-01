using MediatR;

namespace FooBar.Api.Features.CatalogItems.Update
{
    public class UpdateCatalogItem : IRequest
    {
        public int Id { get; }
        public string Name { get; }
        public decimal Price { get; }

        public UpdateCatalogItem(int id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }
}