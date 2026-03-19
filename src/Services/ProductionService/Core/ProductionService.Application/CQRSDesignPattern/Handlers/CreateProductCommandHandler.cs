using MediatR;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductionService.Application.CQRSDesignPattern.Results;
using ProductionService.Domain.Entities;
using ProductionService.Persistence;
using ProductionService.Persistence.Context;
using static ProductionService.Persistence.LogDataMessage;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, bool>
    {
        /* Irequest'i nereye eklediysek onu çağırmalı */
        private readonly ProductionServiceContext _context;
        private readonly ICacheService _cacheService;

        public CreateProductCommandHandler(ICacheService cacheService, ProductionServiceContext context)
        {
            _cacheService = cacheService;
            _context = context;
        }

        public async Task<bool> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                /*veritabanına ekleme işlemi*/
                _context.Products.Add(new Product
                {
                    Name = command.Name,
                    Price = command.Price,
                    Description = command.Description,
                    StockQuantity = command.StockQuantity,
                    Status = command.Status,
                    CreatedAt = command.CreatedAt,
                    UpdateddAt = command.UpdateddAt
                });
                await _context.SaveChangesAsync();

                /* yeni production eklnediği için prodcution list cache silinmesi */
                await _cacheService.RemoveAsync("all_prodcuts_list");

                /* yeni production eklnediği için diğer mikroservislerin redis üzerinden bilgilendirilmesi */
                await _cacheService.PublishEventAsync("event_message", new
                {
                    Service = "Production Service / Create Product",
                    Action = "Create Product işlemi yapıldı",
                    Timestamp = DateTime.UtcNow
                });
                return true;
            }
            catch (Exception error)
            {
                /* redis üzerinden log service gönderme */
                await _cacheService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Production Service / Create Product",
                    Level = LogDataMessage.LogLevel.Error,
                    Exception = error.Message,
                    Message = "Create Product oluşturalamadı",
                });
                return false;
            }
            
        }
    }
}
