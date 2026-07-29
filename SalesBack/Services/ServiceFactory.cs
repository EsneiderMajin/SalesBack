namespace SalesBack.Services
{
    public static class ServiceFactory
    {
        public static IProductService CreateProductService() => new ProductService();
        public static ICustomerService CreateCustomerService() => new CustomerService();
        public static ISaleService CreateSaleService() => new SalesService();
    }
}
