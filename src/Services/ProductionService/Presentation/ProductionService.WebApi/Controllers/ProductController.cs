using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductionService.Application.CQRSDesignPattern.Queries;

namespace ProductionService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            var value =  await _mediator.Send(new GetProductQuery());
            /* istek nereye yapılıyorsa send metodu içinde orası olmalı*/
            return Ok(value);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            /* CreateProductCommand -> zaman Irequesti<bool> var */
            var value = await _mediator.Send(command);
            return value ? Ok("Ürün ekleme başarılı") : BadRequest("Ürün eklenirken bir hata oluştu");
        }

        [HttpDelete]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var value = await _mediator.Send(new RemoveProductCommand(id));
            /* constructor olduğu için new dedik*/
            return value ? Ok("Ürün silme başarılı") : BadRequest("Ürün silinirken bir hata oluştu");
        }

        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProduct(UpdateProductCommand command)
        {
            var value =  await _mediator.Send(command);
            return value ? Ok("Ürün güncelleme başarılı") : BadRequest("Ürün güncellenirken bir hata oluştu");
        }
        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var value = await _mediator.Send(new GetProductByIdQuery(id));
            return Ok(value);
        }
    }
}
