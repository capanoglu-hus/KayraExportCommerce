using MediatR;

namespace ProductionService.Application.CQRSDesignPattern.Commands
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateddAt { get; set; }
    }
}
