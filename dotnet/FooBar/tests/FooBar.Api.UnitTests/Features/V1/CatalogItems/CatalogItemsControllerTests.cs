using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FooBar.Api.Features.V1.CatalogItems;
using FooBar.Api.Features.V1.CatalogItems.GetList;
using FooBar.Api.UnitTests.Abstract;
using FooBar.Api.UnitTests.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Stashbox.Mocking.Moq;
using Xunit;

namespace FooBar.Api.UnitTests.Features.V1.CatalogItems
{
    public class CatalogItemsControllerTestsFixture : ServiceFixture
    {
        public CatalogItemsControllerTestsFixture()
        {
            var singleItem = new CatalogItemViewModel
            {
                Id = 1,
                Name = "Item1",
                Price = 123.45m,
                PictureUri = "url123"
            };
            ExpectedResponseItem1 = singleItem;
            ExpectedResponse = new List<CatalogItemViewModel> { singleItem };
        }
        
        public CatalogItemViewModel ExpectedResponseItem1 { get; }
        
        public IEnumerable<CatalogItemViewModel> ExpectedResponse { get; }
    }
    
    public class CatalogItemsControllerTests: TestBase<CatalogItemsControllerTestsFixture>
    {
        public CatalogItemsControllerTests(CatalogItemsControllerTestsFixture serviceFixture) : base(serviceFixture)
        {
        }
        
        [Fact]
        public async Task WhenSearching_ShouldDispatchMessage()
        {
            // Arrange
            using var stash = StashMoq.Create();
            var mockMediator = stash.Mock<IMediator>();
            mockMediator
                .Setup(x => x
                    .Send(It.IsAny<GetCatalogItems>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceFixture.ExpectedResponse);

            var controller = stash
                .GetWithParamOverrides<CatalogItemsController>(mockMediator)
                .BypassStub(stash);

            // Act
            var result = await controller.Search();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var responseValue = okResult!.Value as List<CatalogItemViewModel>;
            responseValue.Should().NotBeNull();
            var responseValueItem = responseValue!.Single();
            responseValueItem.Id.Should().Be(ServiceFixture.ExpectedResponseItem1.Id);
            responseValueItem.Name.Should().Be(ServiceFixture.ExpectedResponseItem1.Name);
            responseValueItem.Price.Should().Be(ServiceFixture.ExpectedResponseItem1.Price);
            responseValueItem.PictureUri.Should().Be(ServiceFixture.ExpectedResponseItem1.PictureUri);
        }
    }
}