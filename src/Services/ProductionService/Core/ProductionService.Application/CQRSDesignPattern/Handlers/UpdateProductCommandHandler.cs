using MediatR;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductionService.Persistence;
using ProductionService.Persistence.Context;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class UpdateProductCommandHandler :IRequestHandler<UpdateProductCommand>
    {
        private readonly ProductionServiceContext _context;
        private readonly ICacheService _cacheService;

        public UpdateProductCommandHandler(ICacheService cacheService,ProductionServiceContext context)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var value = await _context.Products.FindAsync(command.ProductId);
            /*veritabanında ilgili productı buluyor ve değişiklikleri kaydediyor*/
            value.Name = command.Name;
            value.Description = command.Description;
            value.Price = command.Price;
            value.StockQuantity = command.StockQuantity;
            value.Status = command.Status;
            value.UpdateddAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // güncellemeden sonra cachete tutulan veriler silinir
            await _cacheService.RemoveAsync("all_prodcuts_list");
            await _cacheService.RemoveAsync($"product_{value.ProductId}");
            await _cacheService.PublishEventAsync("event_message", new
            {
                Service = "ProductionService/UpdateProduct",
                Action = $"UpdateProduct işlemi yapıldı {value.ProductId}",
                Timestamp = DateTime.UtcNow
            });
        } 
    }
}
