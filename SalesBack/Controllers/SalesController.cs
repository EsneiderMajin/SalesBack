using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using SalesBack.DTOs;
using SalesBack.Services;

namespace SalesBack.Controllers
{
    [RoutePrefix("api/sales")]
    public class SalesController : ApiController
    {
        private readonly ISaleService _service = ServiceFactory.CreateSaleService();

        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateSale([FromBody] CreateSaleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (request.Items == null || !request.Items.Any())
                return BadRequest("La venta debe tener al menos un item.");
            try
            {
                int saleId = _service.CreateSale(request);
                return Created($"api/sales/{saleId}", new { SaleId = saleId });
            }
            catch (SqlException ex) when (ex.Message.Contains("inactivo"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetSaleById(int id)
        {
            var detail = _service.GetSaleById(id);
            if (detail == null) return NotFound();
            return Ok(detail);
        }
    }
}


