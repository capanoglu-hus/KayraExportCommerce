using MediatR;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductService.Persistence.Context;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand>
    {
        private readonly ProductionServiceContext _context;

        public RemoveProductCommandHandler(ProductionServiceContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveProductCommand command, CancellationToken cancellationToken)
        {
            var values = await _context.Products.FindAsync(command.ProductId);
            /*veritabanında ilgili productı buluyor*/
            _context.Products.Remove(values);
            /*bulunan ilgili productı siliyor*/
            await _context.SaveChangesAsync();
        }

        
    }
}
