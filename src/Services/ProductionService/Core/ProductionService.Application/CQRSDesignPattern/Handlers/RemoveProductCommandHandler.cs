using MediatR;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductionService.Persistence;
using ProductionService.Persistence.Context;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand>
    {
        private readonly ProductionServiceContext _context;
        private readonly ICacheService _cacheService;

        public RemoveProductCommandHandler(ICacheService cacheService, ProductionServiceContext context)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task Handle(RemoveProductCommand command, CancellationToken cancellationToken)
        {
            var values = await _context.Products.FindAsync(command.ProductId);
            /*veritabanında ilgili productı buluyor*/
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
        }

        
    }
}
