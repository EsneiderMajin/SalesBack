using System;
using System.Web.Http;
using SalesBack.Services;

namespace SalesBack.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _service = ServiceFactory.CreateCustomerService();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetActiveCustomers()
        {
            try
            {
                var products = _service.GetActiveCustomers();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
