using MediatR;
using ProductionService.Application.CQRSDesignPattern.Queries;
using ProductionService.Application.CQRSDesignPattern.Results;
using ProductService.Persistence.Context;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery,GetProductByIdQueryResult>
    {
        /*
         GetProductByIdQuery -> request isteği attığı yer - istek yapacağı
        GetProductByIdQueryResult -> geri dönüş için gerekli olan - isteğe karşılık bulacağı yer

         */
        private readonly ProductionServiceContext _context;

        public GetProductByIdQueryHandler(ProductionServiceContext context)
        {
            _context = context;
        }

        public async Task<GetProductByIdQueryResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var value = await _context.Products.FindAsync(query.ProductId);
            /*veritabanında productId sayesinde productı buluyor*/
            return new GetProductByIdQueryResult
            {
                ProductId = value.ProductId,
                Name = value.Name,
                Description = value.Description,
                Price = value.Price,
                StockQuantity = value.StockQuantity,
                Status = value.Status,
                CreatedAt = value.CreatedAt,
                UpdateddAt = value.UpdateddAt
            };
        }

        
    }
}
