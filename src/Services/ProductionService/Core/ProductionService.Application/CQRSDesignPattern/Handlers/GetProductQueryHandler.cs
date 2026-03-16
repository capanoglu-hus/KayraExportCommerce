using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductionService.Application.CQRSDesignPattern.Queries;
using ProductionService.Application.CQRSDesignPattern.Results;
using ProductService.Persistence.Context;

namespace ProductionService.Application.CQRSDesignPattern.Handlers
{
    public class GetProductQueryHandler :IRequestHandler<GetProductQuery,List<GetProductQueryResult>>
    {
        /* geriye dönmesi gerekn bir yapı olması gerektiği için */
        private readonly ProductionServiceContext _context;

        public GetProductQueryHandler(ProductionServiceContext context)
        {
            _context = context;
        }

        public async Task<List<GetProductQueryResult>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var values = await _context.Products.ToListAsync();
            /*veritabanında bulunan bütün productları listeliyor */
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
