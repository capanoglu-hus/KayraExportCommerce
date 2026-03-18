using MediatR;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductionService.Persistence;
using ProductionService.Persistence.Context;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static ProductionService.Persistence.LogDataMessage;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class UpdateProductCommandHandler :IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly ProductionServiceContext _context;
        private readonly ICacheService _cacheService;

        public UpdateProductCommandHandler(ICacheService cacheService,ProductionServiceContext context)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var value = await _context.Products.FindAsync(command.ProductId);
                /*veritabanında ilgili productı buluyor ve değişiklikleri kaydediyor*/
                if(value != null)
                {
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
                    return true;

                }
                await _cacheService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Production",
                    Level = LogDataMessage.LogLevel.Critical,
                    Message = "Update ürün bulunamadı. ",
                });
                return false;
            }
            catch (Exception error )
            {
                await _cacheService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Production",
                    Level = LogDataMessage.LogLevel.Error,
                    Exception = error.Message,
                    Message = "Product UpdateProductCommand ",
                });
                return false;
            }
           
        } 
    }
}
