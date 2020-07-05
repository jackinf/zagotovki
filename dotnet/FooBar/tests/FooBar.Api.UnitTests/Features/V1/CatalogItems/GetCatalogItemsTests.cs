using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FooBar.Api.Features.V1.CatalogItems.GetList;
using FooBar.Domain.Entities;
using FooBar.Domain.Interfaces;
using FooBar.Domain.Specifications;
using Moq;
using Stashbox.Mocking.Moq;
using Xunit;

namespace FooBar.Api.UnitTests.Features.V1.CatalogItems
{
    public class GetCatalogItemsTests
    {
        private readonly GetCatalogItemsHandler _sut;
        private readonly Mock<ICatalogItemRepository> _mockRepository;

        public GetCatalogItemsTests()
        {
            var stash = StashMoq.Create();
            _mockRepository = stash.Mock<ICatalogItemRepository>();

            _sut = stash.GetWithParamOverrides<GetCatalogItemsHandler>(_mockRepository);
        }

        [Fact]
        public async Task WhenSearching_ShouldGetItems()
        {
            // Arrange
            var catalogItem = new CatalogItem(1, 2, "t", "d", 1, "p");
            _mockRepository
                .Setup(x => x.ListAsync(It.IsAny<CatalogItemsSpecification>()))
                .ReturnsAsync(new List<CatalogItem> { catalogItem });
            
            // Act
            var result = (await _sut.Handle(new GetCatalogItems(3, 1), CancellationToken.None)).ToList();
            
            // Assert
            result.Count.Should().Be(1);
            var item = result.Single();
            item.Name.Should().Be(catalogItem.Name);
            item.Price.Should().Be(catalogItem.Price);
            item.PictureUri.Should().Be(catalogItem.PictureUri);
        }
    }
}