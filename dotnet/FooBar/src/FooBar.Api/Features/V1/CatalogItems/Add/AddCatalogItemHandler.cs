using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FooBar.Domain.Entities;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Interfaces;
using MediatR;

namespace FooBar.Api.Features.V1.CatalogItems.Add
{
    public class AddCatalogItemHandler : IRequestHandler<AddCatalogItem>
    {
        private readonly ICatalogItemRepository catalogItemRepository;
        private readonly IAsyncRepository<CatalogBrand> catalogBrandRepository;
        private readonly IAsyncRepository<CatalogType> catalogTypeRepository;

        public AddCatalogItemHandler(
            ICatalogItemRepository catalogItemRepository,
            IAsyncRepository<CatalogBrand> catalogBrandRepository,
            IAsyncRepository<CatalogType> catalogTypeRepository)
        {
            this.catalogItemRepository = catalogItemRepository;
            this.catalogBrandRepository = catalogBrandRepository;
            this.catalogTypeRepository = catalogTypeRepository;
        }
        
        public async Task<Unit> Handle(AddCatalogItem request, CancellationToken cancellationToken)
        {
            await Validate(request);

            var catalogItem = new CatalogItem(
                request.CatalogTypeId, 
                request.CatalogBrandId, 
                request.Description, 
                request.Name, 
                request.Price,
                request.PictureUri);

            try
            {
                await catalogItemRepository.AddAsync(catalogItem);
                return Unit.Value;
            }
            catch (Exception)
            {
                throw new GeneralException($"Failed to add an item");
            }
        }

        private async Task Validate(AddCatalogItem request)
        {
            var validationFailures = new List<ValidationFailure>();
            var catalogBrand = await catalogBrandRepository.GetByIdAsync(request.CatalogBrandId);
            if (catalogBrand == null)
            {
                validationFailures.Add(new ValidationFailure(nameof(request.CatalogBrandId), $"Item with id {request.CatalogBrandId} does not exist in DB"));
            }

            var catalogType = await catalogTypeRepository.GetByIdAsync(request.CatalogTypeId);
            if (catalogType == null)
            {
                validationFailures.Add(new ValidationFailure(nameof(request.CatalogTypeId), $"Item with id {request.CatalogTypeId} does not exist in DB"));
            }

            if (validationFailures.Any())
            {
                throw new ValidationException(validationFailures);
            }
        }
    }
}