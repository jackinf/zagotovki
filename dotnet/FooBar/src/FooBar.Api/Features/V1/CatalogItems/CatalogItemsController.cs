using System.Threading.Tasks;
using FluentValidation;
using FooBar.Api.Features.V1.CatalogItems.Add;
using FooBar.Api.Features.V1.CatalogItems.Delete;
using FooBar.Api.Features.V1.CatalogItems.GetList;
using FooBar.Api.Features.V1.CatalogItems.GetSingle;
using FooBar.Api.Features.V1.CatalogItems.Update;
using FooBar.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FooBar.Api.Features.V1.CatalogItems
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("v{version:apiVersion}/catalog-items")]
    public class CatalogItemsController : Controller
    {
        private readonly ILogger<CatalogItemsController> _logger;
        private readonly IMediator _mediator;
        
        public CatalogItemsController(ILogger<CatalogItemsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<IActionResult> Search(int page = 0)
        {
            var catalogItems = await _mediator.Send(new GetCatalogItems(10, page));
            return Ok(catalogItems);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _mediator.Send(new GetCatalogItem(id)));
            }
            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
            catch (ItemNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddCatalogItemViewModel viewModel, ApiVersion apiVersion)
        {
            try
            {
                var id = await _mediator.Send(new AddCatalogItem(
                    viewModel.Name,
                    viewModel.Description,
                    viewModel.Price,
                    viewModel.PictureUri,
                    viewModel.CatalogTypeId,
                    viewModel.CatalogBrandId));
            
                return CreatedAtRoute(nameof(Get), new { version = apiVersion.ToString(), id });
            }
            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCatalogItemViewModel viewModel)
        {
            try
            {
                await _mediator.Send(new UpdateCatalogItem(id, viewModel.Name, viewModel.Price));
                return Ok("An item was successfully updated!");
            }
            catch (ItemNotFoundException e)
            {
                return BadRequest(e.Message);
            }
            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _mediator.Send(new DeleteCatalogItem(id));
                return Ok("An item was successfully deleted!");
            }
            catch (ItemNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}