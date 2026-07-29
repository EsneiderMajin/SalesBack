using SalesBack.DTOs;

namespace SalesBack.Services
{
    public interface ISaleService
    {
        int CreateSale(CreateSaleRequest request);
        SaleDetailResponse GetSaleById(int id);
    }
}
