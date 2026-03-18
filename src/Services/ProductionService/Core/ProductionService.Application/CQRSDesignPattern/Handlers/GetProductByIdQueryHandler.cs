using MediatR;
using ProductionService.Application.CQRSDesignPattern.Queries;
using ProductionService.Application.CQRSDesignPattern.Results;
using ProductionService.Persistence;
using ProductionService.Persistence.Context;
using static ProductionService.Persistence.LogDataMessage;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            try
            {
                /* cachete bulunan product kontrolü*/
                string cacheKey = $"product_{query.ProductId}";
                var cachedProduct = await _cacheService.GetAsync<GetProductByIdQueryResult>(cacheKey);
                /*cachte varsa direkt cacheten dön*/
                if (cachedProduct != null) return cachedProduct;
                /*veritabanında ilgili product bulma*/
                var value = await _context.Products.FindAsync(query.ProductId);

                if (value != null)
                {
                    var result = new GetProductByIdQueryResult
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
                    /*cache ekleme*/
                    await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));
                    return result;
                }
                /**/
                await _cacheService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Production",
                    Level = LogDataMessage.LogLevel.Critical,
                    Message = "Product bulunamadı",
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
                    Message = "GetProductByIdQueryResult sorun",
                });
                return null;
            }
        }

        
    }
}
