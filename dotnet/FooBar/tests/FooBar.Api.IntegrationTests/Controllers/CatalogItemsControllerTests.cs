using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FooBar.Api.Features.V1.CatalogItems;
using FooBar.Api.IntegrationTests.Abstract;
using Newtonsoft.Json;
using Xunit;

namespace FooBar.Api.IntegrationTests.Controllers
{
    public class CatalogItemsControllerTests: IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public CatalogItemsControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }
        
        [Fact]
        public async Task WhenSearchingCatalogItems_ShouldGetSuccessfully()
        {
            // Act
            using var client = _factory.CreateClient();
            var httpResponse = await client.GetAsync("/v1/catalog-items");
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<List<CatalogItemViewModel>>(stringResponse);
            response.Should().NotBeNull();

            // Assert
            var testData = Utilities.GetPreconfiguredItems();
            response.Count.Should().Be(Math.Min(10, testData.Count()));
            var item = response!.Single(x => x.Name == Utilities.CatalogItemNames.RoslynRedSheet);
            item.Price.Should().Be(item.Price);
            item.PictureUri.Should().Be(item.PictureUri);
        }
    }
}