using MediatR;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductionService.Persistence;
using ProductionService.Persistence.Context;
using static ProductionService.Persistence.LogDataMessage;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand , bool>
    {
        private readonly ProductionServiceContext _context;
        private readonly ICacheService _cacheService;

        public RemoveProductCommandHandler(ICacheService cacheService, ProductionServiceContext context)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(RemoveProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var values = await _context.Products.FindAsync(command.ProductId);
                /*veritabanında ilgili productı buluyor*/
                if(values != null)
                {
                    _context.Products.Remove(values);
                    /*bulunan ilgili productı siliyor*/
                    await _context.SaveChangesAsync();

                    await _cacheService.RemoveAsync("all_prodcuts_list");
                    await _cacheService.RemoveAsync($"product_{command.ProductId}");
                    await _cacheService.PublishEventAsync("event_message", new
                    {
                        Service = "ProductionService/RemoveProduct",
                        Action = "RemoveProduct işlemi yapıldı",
                        Timestamp = DateTime.UtcNow
                    });
                    return true;
                }
                await _cacheService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Production",
                    Level = LogDataMessage.LogLevel.Critical,
                    Message = "Product RemoveProductCommand ",
                });
                return false;

            }
            catch (Exception error)
            {
                await _cacheService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Production",
                    Level = LogDataMessage.LogLevel.Error,
                    Exception = error.Message,
                    Message = "Product RemoveProductCommand ",
                });
                return false;
                
            }
        }
    }
}
