using MediatR;
using ProductionService.Application.CQRSDesignPattern.Results;

namespace ProductionService.Application.CQRSDesignPattern.Queries
{
    public class GetProductByIdQuery: IRequest<GetProductByIdQueryResult>
    {

        public GetProductByIdQuery(int productId)
        {
            ProductId = productId;
        }
        
        public int ProductId { get; set; }
    
    }
}
