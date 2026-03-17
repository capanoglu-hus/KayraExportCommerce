using MediatR;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductionService.Persistence;
using ProductionService.Domain.Entities;
using ProductionService.Persistence.Context;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
    {
        /* Irequest'i nereye eklediysek onu çağırmalı */
        private readonly ProductionServiceContext _context;
        private readonly ICacheService _cacheService;

        public CreateProductCommandHandler(ICacheService cacheService, ProductionServiceContext context)
        {
            _cacheService = cacheService;
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

            await _cacheService.RemoveAsync("all_prodcuts_list");

            await _cacheService.PublishEventAsync("event_message", new
            {
                Service = "ProductionService/CreateProductCommandHandler",
                Action = "CreateProduct işlemi yapıldı",
                Timestamp = DateTime.UtcNow
            });
        }

       
    }
}
