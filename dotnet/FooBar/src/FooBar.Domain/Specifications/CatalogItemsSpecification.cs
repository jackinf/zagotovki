using System.Linq;
using FooBar.Domain.Entities;

namespace FooBar.Domain.Specifications
{
    public sealed class CatalogItemsSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogItemsSpecification(int skip, int take) : base(c => c != null) // TODO: Do I need criteria all the times?
        {
            ApplyPaging(skip, take);
        }
    }
}
