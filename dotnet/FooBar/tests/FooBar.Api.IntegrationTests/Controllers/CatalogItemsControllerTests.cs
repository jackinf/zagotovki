using System.Threading.Tasks;
using FooBar.Api.IntegrationTests.Abstract;
using Newtonsoft.Json;
using Xunit;

namespace FooBar.Api.IntegrationTests.Controllers
{
    public class CatalogItemsControllerTests: IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;

        public CatalogItemsControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }
        
        [Fact]
        public async Task WhenSearchingCatalogItems_ShouldGetSuccessfully()
        {
            using var client = factory.CreateClient();
            var httpResponse = await client.GetAsync("/v1/catalog-items");
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            // var response = JsonConvert.DeserializeObject<GetSportNewsResponse>(stringResponse);
        }
    }
}