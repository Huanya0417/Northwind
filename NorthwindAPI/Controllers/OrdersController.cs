using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindAPI.Models;
using NorthwindViewModel;
using static NuGet.Packaging.PackagingConstants;

namespace NorthwindAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public OrdersController(NorthwindContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersDTO>>> GetOrders(int? orderID, string? customerName)
        {
            List<OrdersDTO> ordersDTOs = new List<OrdersDTO>();

            var queryData = _context.Orders
                                    .Include(o => o.Customer)
                                    .Include(o => o.Employee)
                                    .Include(o => o.ShipViaNavigation)
                                    .AsQueryable();

            if (orderID != null && orderID > 0)
            {
                queryData = queryData.Where(o => o.OrderID == orderID);
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                queryData = queryData.Where(o => o.Customer.CompanyName == customerName);
            }

            ordersDTOs = await queryData.Select(o => new OrdersDTO
            {
                OrderID = o.OrderID,
                CustomerID = o.CustomerID,
                CustomerName = o.Customer.CompanyName,
                EmployeeID = o.EmployeeID,
                EmployeeName = o.Employee.FirstName + " " + o.Employee.LastName,
                OrderDate = o.OrderDate,
                RequiredDate = o.RequiredDate,
                ShippedDate = o.ShippedDate,
                ShipVia = o.ShipVia,
                ShipCompany = o.ShipViaNavigation.CompanyName,
                Freight = o.Freight,
                ShipName = o.ShipName,
                ShipAddress = o.ShipAddress,
                ShipCity = o.ShipCity,
                ShipRegion = o.ShipRegion,
                ShipPostalCode = o.ShipPostalCode,
                ShipCountry = o.ShipCountry,
            }).ToListAsync();

            return ordersDTOs;
        }

        // GET: api/Orders/5
        [HttpGet("{orderID}")]
        public async Task<ActionResult<OrdersDTO>> GetOrders(int orderID)
        {
            OrdersDTO ordersDTO = new OrdersDTO();

            var order = await _context.Orders
                                      .Include(o => o.Customer)
                                      .Include(o => o.Employee)
                                      .Include(o => o.ShipViaNavigation)
                                      .Include(o => o.Order_Details).ThenInclude(o => o.Product)
                                      .FirstOrDefaultAsync(o => o.OrderID == orderID);

            if (order == null)
            {
                return NotFound();
            }

            ordersDTO = new OrdersDTO
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                CustomerName = order.Customer.CompanyName,
                EmployeeID = order.EmployeeID,
                EmployeeName = order.Employee.FirstName + " " + order.Employee.LastName,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                ShipVia = order.ShipVia,
                ShipCompany = order.ShipViaNavigation.CompanyName,
                Freight = order.Freight,
                ShipName = order.ShipName,
                ShipAddress = order.ShipAddress,
                ShipCity = order.ShipCity,
                ShipRegion = order.ShipRegion,
                ShipPostalCode = order.ShipPostalCode,
                ShipCountry = order.ShipCountry,
                orderDetails = order.Order_Details.Select(o => new OrderDetailsDTO()
                {
                    OrderID = o.OrderID,
                    ProductID = o.ProductID,
                    ProductName = o.Product.ProductName,
                    UnitPrice = o.UnitPrice,
                    Quantity = o.Quantity,
                    Discount = o.Discount,
                }).ToList()
            };

            return ordersDTO;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{orderID}")]
        public async Task<IActionResult> PutOrders(int orderID, OrdersDTO ordersDTO)
        {
            if (orderID != ordersDTO.OrderID)
            {
                return BadRequest();
            }

            var entity = await _context.Orders.FindAsync(orderID);

            if (entity == null)
            {
                return NotFound();
            }

            entity.ShipVia = ordersDTO.ShipVia;
            entity.ShipName = ordersDTO.ShipName;
            entity.ShipAddress = ordersDTO.ShipAddress;
            entity.ShipCity = ordersDTO.ShipCity;
            entity.ShipRegion = ordersDTO.ShipRegion;
            entity.ShipPostalCode = ordersDTO.ShipPostalCode;
            entity.ShipCountry = ordersDTO.ShipCountry;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdersExists(orderID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{orderID}")]
        public async Task<IActionResult> DeleteOrders(int orderID)
        {
            var Order_Details = await _context.Order_Details.Where(o => o.OrderID == orderID).ToListAsync();

            if (Order_Details == null)
            {
                return NotFound();
            }

            _context.Order_Details.RemoveRange(Order_Details);

            var orders = await _context.Orders.FindAsync(orderID);

            if (orders == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(orders);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Orders>> PostOrders(Orders orders)
        {
            _context.Orders.Add(orders);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrders", new { id = orders.OrderID }, orders);
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
