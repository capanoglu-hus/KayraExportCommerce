using MediatR;
using ProductionService.Application.CQRSDesignPattern.Results;

namespace ProductionService.Application.CQRSDesignPattern.Queries
{
    public class GetProductQuery :IRequest<List<GetProductQueryResult>>
    {
        /* product query ile ilgili her şey buraya gelecek */
    }
}
