using MediatR;

namespace ProductionService.Application.CQRSDesignPattern.Commands
{
    public class RemoveProductCommand : IRequest<bool>
    {
        /*getbyıd için yapı */
        public RemoveProductCommand(int productId)
        {
            ProductId = productId;
        }

        public int ProductId { get; set; }

      
    }
}
