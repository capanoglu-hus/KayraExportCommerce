using MediatR;

namespace ProductionService.Application.CQRSDesignPattern.Commands
{
    public class RemoveProductCommand : IRequest
    {
        /*getbyıd için yapı */
        public RemoveProductCommand(int productId)
        {
            ProductId = productId;
        }

        public int ProductId { get; set; }

      
    }
}
