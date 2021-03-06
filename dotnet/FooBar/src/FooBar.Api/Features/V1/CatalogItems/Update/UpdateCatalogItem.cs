using MediatR;

namespace FooBar.Api.Features.V1.CatalogItems.Update
{
    /// <summary>
    /// Handled by <see cref="UpdateCatalogItemHandler"/>
    /// </summary>
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