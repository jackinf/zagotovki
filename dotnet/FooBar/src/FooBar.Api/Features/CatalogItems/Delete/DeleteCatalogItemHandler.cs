using System.Threading;
using System.Threading.Tasks;
using FooBar.Domain.Interfaces;
using MediatR;

namespace FooBar.Api.Features.CatalogItems.Delete
{
    public class DeleteCatalogItemHandler : IRequestHandler<DeleteCatalogItem>
    {
        private readonly ICatalogItemRepository catalogItemRepository;

        public DeleteCatalogItemHandler(ICatalogItemRepository catalogItemRepository)
        {
            this.catalogItemRepository = catalogItemRepository;
        }
        
        public async Task<Unit> Handle(DeleteCatalogItem request, CancellationToken cancellationToken)
        {
            var catalogItem = await catalogItemRepository.GetByIdAsync(request.Id);
            await catalogItemRepository.DeleteAsync(catalogItem);

            return Unit.Value;
        }
    }
}