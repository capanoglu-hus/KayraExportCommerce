using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductionService.Application.CQRSDesignPattern.Queries;
using ProductionService.Application.CQRSDesignPattern.Results;
using ProductionService.Domain.Entities;
using ProductionService.Persistence;
using ProductionService.Persistence.Context;
using static ProductionService.Persistence.LogDataMessage;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            try
            {
                /*cachete liste var mı kontrol ediyor */
                string cacheKey = "all_prodcuts_list";
                var cachedData = await _cacheService.GetAsync<List<GetProductQueryResult>>(cacheKey);
                if (cachedData != null) return cachedData;
                /*yoksa veritabanında listeyi çekiyor*/
                var values = await _context.Products.ToListAsync();
                /*veritabanında bulunan bütün productları listeliyor */
                if(values != null)
                {
                    await _cacheService.SetAsync(cacheKey, values, TimeSpan.FromMinutes(30));
                    /* cacheKey -> rediste tutualn ad 
                     * values - değerler 
                     timeSpan -> redisten ne zaman silinecek
                    */
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
                await _cacheService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Production",
                    Level = LogDataMessage.LogLevel.Critical,
                    Message = "Products bulunamadı ",
                });
                return null;
            }
            catch (Exception error)
            {
                await _cacheService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Production",
                    Level = LogDataMessage.LogLevel.Error,
                    Exception = error.Message,
                    Message = "Product GetProductQueryResult ",
                });
                return null;
            }
  
        }

    }
}
