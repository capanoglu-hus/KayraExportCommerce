using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductionService.Application.CQRSDesignPattern.Handlers;
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
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            await _mediator.Send(command);
            /* CreateProductCommand -> zaman Irequesti var */
            return Ok("Ürün ekleme başarılı");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _mediator.Send(new RemoveProductCommand(id));
            /* constructor olduğu için new dedik*/
            return Ok("Silme işlemi başarılı");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(UpdateProductCommand command)
        {
            await _mediator.Send(command);
            return Ok("Güncelleme işlemi başarılı");
        }
        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var value = await _mediator.Send(new GetProductByIdQuery(id));
            return Ok(value);
        }
    }
}
