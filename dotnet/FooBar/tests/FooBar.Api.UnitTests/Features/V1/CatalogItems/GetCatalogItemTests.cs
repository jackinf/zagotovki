using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FooBar.Api.Features.V1.CatalogItems;
using FooBar.Api.Features.V1.CatalogItems.GetSingle;
using FooBar.Domain.Entities;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Interfaces;
using Moq;
using Stashbox.Mocking.Moq;
using Xunit;

namespace FooBar.Api.UnitTests.Features.V1.CatalogItems
{
    public class GetCatalogItemTests
    {
        private readonly GetCatalogItemHandler _sut;
        private readonly Mock<ICatalogItemRepository> _mockRepository;

        public GetCatalogItemTests()
        {
            var stash = StashMoq.Create();
            _mockRepository = stash.Mock<ICatalogItemRepository>();

            _sut = stash.GetWithParamOverrides<GetCatalogItemHandler>(_mockRepository);
        }
        
        [Fact]
        public async Task WhenCannotFindItem_ShouldThrow()
        {
            // Arrange
            const int catalogItemId = 1;
            _mockRepository
                .Setup(x => x.GetByIdAsync(catalogItemId))
                .ReturnsAsync((CatalogItem) null);
            
            // Act
            var task = new Func<Task<CatalogItemViewModel>>(() => _sut.Handle(new GetCatalogItem(1), CancellationToken.None));
            
            // Assert
            var exceptionAssertions = await task.Should().ThrowAsync<ItemNotFoundException>();
            var errors = exceptionAssertions.Subject.Select(x => x.Message);
            errors.Should().ContainSingle($"Catalog item with {catalogItemId} was not found");
        }
        
        [Fact]
        public async Task WhenSearching_ShouldGetItem()
        {
            // Arrange
            const int catalogItemId = 1;
            var catalogItem = new CatalogItem(1, 2, "t", "d", 1, "p");
            _mockRepository
                .Setup(x => x.GetByIdAsync(catalogItemId))
                .ReturnsAsync(catalogItem);
            
            // Act
            var item = await _sut.Handle(new GetCatalogItem(catalogItemId), CancellationToken.None);
            
            // Assert
            item.Name.Should().Be(catalogItem.Name);
            item.Price.Should().Be(catalogItem.Price);
            item.PictureUri.Should().Be(catalogItem.PictureUri);
        }
    }
}