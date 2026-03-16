using MediatR;
using ProductionService.Application.CQRSDesignPattern.Commands;
using ProductService.Persistence.Context;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class UpdateProductCommandHandler :IRequestHandler<UpdateProductCommand>
    {
        private readonly ProductionServiceContext _context;

        public UpdateProductCommandHandler(ProductionServiceContext context)
        {
            _context = context;
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
        } 
    }
}
