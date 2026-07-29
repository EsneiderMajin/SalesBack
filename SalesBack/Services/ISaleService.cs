using SalesBack.DTOs;

namespace SalesBack.Services
{
    public interface ISaleService
    {
        SaleDetailResponse CreateSale(CreateSaleRequest request);
        SaleDetailResponse GetSaleById(int id);
    }
}
