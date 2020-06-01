using FooBar.Api.Features.CatalogItems;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FooBar.Api.Controllers
{
    [ApiController]
    [Route("catalog-items")]
    public class CatalogItemsController : ControllerBase
    {
        private readonly ILogger<CatalogItemsController> logger;
        private readonly IMediator mediator;

        public CatalogItemsController(ILogger<CatalogItemsController> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }
        
        [HttpGet]
        public IActionResult Search(int page = 0)
        {
            var catalogItems = mediator.Send(new GetCatalogItems(10, page));
            return Ok(catalogItems);
        }
        
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            var catalogItem = mediator.Send(new GetCatalogItem(id));
            return Ok(catalogItem);
        }
        
        [HttpPost]
        public IActionResult Add(AddCatalogItemViewModel viewModel)
        {
            mediator.Send(new AddCatalogItem(
                viewModel.Name,
                viewModel.Description,
                viewModel.Price,
                viewModel.PictureUri,
                viewModel.CatalogTypeId,
                viewModel.CatalogBrandId));
            
            return Ok("An item was successfully added!");
        }
        
        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(int id, UpdateCatalogItemViewModel viewModel)
        {
            mediator.Send(new UpdateCatalogItem(id, viewModel.Name, viewModel.Price));
            
            return Ok("An item was successfully updated!");
        }
        
        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            mediator.Send(new DeleteCatalogItem(id));
            
            return Ok("An item was successfully deleted!");
        }
    }
}