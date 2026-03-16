using MediatR;

namespace ProductionService.Application.CQRSDesignPattern.Commands
{
    public class CreateProductCommand:IRequest
    {
        /*
         * IRequest -> Presentation sınıfında kullanacak isteği buraya attığımı bildirecez
         */
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateddAt { get; set; }
    }
}
