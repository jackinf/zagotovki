using MediatR;

namespace FooBar.Api.Features.CatalogItems
{
    public class AddCatalogItem : IRequest
    {
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public string PictureUri { get; }
        public int CatalogTypeId { get; }
        public int CatalogBrandId { get; }

        public AddCatalogItem(string name, string description, decimal price, string pictureUri, int catalogTypeId, int catalogBrandId)
        {
            Name = name;
            Description = description;
            Price = price;
            PictureUri = pictureUri;
            CatalogTypeId = catalogTypeId;
            CatalogBrandId = catalogBrandId;
        }
    }
}