using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductionService.Application.CQRSDesignPattern.Queries;
using ProductionService.Application.CQRSDesignPattern.Results;
using ProductionService.Persistence;
using ProductionService.Domain.Entities;
using ProductionService.Persistence.Context;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class GetProductQueryHandler :IRequestHandler<GetProductQuery,List<GetProductQueryResult>>
    {
        /* geriye dönmesi gerekn bir yapı olması gerektiği için */
        private readonly ProductionServiceContext _context;
        private readonly ICacheService _cacheService;
        public GetProductQueryHandler(ProductionServiceContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<List<GetProductQueryResult>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = "all_prodcuts_list";
            var cachedData = await _cacheService.GetAsync<List<GetProductQueryResult>>(cacheKey);
            if (cachedData != null) return cachedData;
            var values = await _context.Products.ToListAsync();
            /*veritabanında bulunan bütün productları listeliyor */
            await _cacheService.SetAsync(cacheKey, values, TimeSpan.FromMinutes(30));
            return values.Select(x => new GetProductQueryResult
            {
                ProductId = x.ProductId,
                Name = x.Name,
                Price = x.Price,
                Description = x.Description,
                StockQuantity = x.StockQuantity,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                UpdateddAt = x.UpdateddAt
            }).ToList();
        }

    }
}
