using MediatR;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductService.Domain.Entities;
using ProductService.Persistence.Context;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
    {
        /* Irequest'i nereye eklediysek onu çağırmalı */
        private readonly ProductionServiceContext _context;

        public CreateProductCommandHandler(ProductionServiceContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            _context.Products.Add(new Product
            {
                Name = command.Name,
                Price = command.Price,
                Description = command.Description,
                StockQuantity = command.StockQuantity,
                Status =command.Status,
                CreatedAt = command.CreatedAt,
                UpdateddAt = command.UpdateddAt
            });
            await _context.SaveChangesAsync();
            /*veritabanına ekleme işlemi*/
        }

       
    }
}
