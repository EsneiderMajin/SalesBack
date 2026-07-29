using System;
using System.Web.Http;
using SalesBack.Services;

namespace SalesBack.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly IProductService _service = ServiceFactory.CreateProductService();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetActiveProducts()
        {
            try
            {
                var products = _service.GetActiveProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
