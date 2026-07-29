using System;
using SalesBack.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using SalesBack.DTOs;
using System.Linq;
using System.Data.Entity;

namespace SalesBack.Services
{
    public class SalesService : ISaleService
    {
        // cadena de conexión leida directo desde Web.config
        // este Service usa ado.net para el flujo de creacion.
        private readonly string _connectionString =
           ConfigurationManager.ConnectionStrings["SalesDbContext"].ConnectionString;

        // crea la venta vía Stored Procedure + ADO.NET,
        public int CreateSale(CreateSaleRequest request)
        {
            string itemsXml = BuildItemsXml(request.Items);

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_CreateSale", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                    cmd.Parameters.AddWithValue("@ItemsXml", itemsXml);

                    // @SaleId es parámetro de salida
                    var saleIdParam = new SqlParameter("@SaleId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(saleIdParam);

                    // no esperamos un result set, solo que el SP
                    cmd.ExecuteNonQuery();
                    return (int)saleIdParam.Value;
                }
            }
        }

        // se serializa a XML para pasarla como un solo parámetro nvarchar
        private string BuildItemsXml(List<SaleItemRequest> items)
        {
            var sb = new StringBuilder("<items>");
            foreach (var item in items)
                sb.Append($"<item productId=\"{item.ProductId}\" quantity=\"{item.Quantity}\"/>");
            sb.Append("</items>");
            return sb.ToString();
        }


        public SaleDetailResponse GetSaleById(int id)
        {
            using (var db = new SalesDbContext())
            {
                var sale = db.Sales.Include(s => s.Customer)
                                    .Include(s => s.SaleItems.Select(i => i.Product))
                                    .FirstOrDefault(s => s.SaleId == id);

                if (sale == null) return null; // El Controller traduce a 404

                return new SaleDetailResponse
                {
                    SaleId = sale.SaleId,
                    CustomerName = sale.Customer.Name,
                    TotalAmount = sale.TotalAmount,
                    CreatedAt = sale.CreatedAt,
                    Items = sale.SaleItems.Select(i => new SaleDetailItemResponse
                    {
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Subtotal = i.Subtotal
                    }).ToList()
                };
            }
        }
    }
}