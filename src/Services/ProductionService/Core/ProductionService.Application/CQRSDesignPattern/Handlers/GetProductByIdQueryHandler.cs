using MediatR;
using ProductionService.Application.CQRSDesignPattern.Queries;
using ProductionService.Application.CQRSDesignPattern.Results;
using ProductionService.Persistence;
using ProductionService.Persistence.Context;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery,GetProductByIdQueryResult>
    {
        /*
         GetProductByIdQuery -> request isteği attığı yer - istek yapacağı
        GetProductByIdQueryResult -> geri dönüş için gerekli olan - isteğe karşılık bulacağı yer

         */
        private readonly ProductionServiceContext _context;
        private readonly ICacheService _cacheService;

        public GetProductByIdQueryHandler(ProductionServiceContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<GetProductByIdQueryResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            string cacheKey = $"product_{query.ProductId}";
            var cachedProduct = await _cacheService.GetAsync<GetProductByIdQueryResult>(cacheKey);
            if (cachedProduct != null) return cachedProduct;

            var value = await _context.Products.FindAsync(query.ProductId);

            if (value != null)
            {
               await _cacheService.SetAsync(cacheKey, value, TimeSpan.FromMinutes(30));
            }

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
