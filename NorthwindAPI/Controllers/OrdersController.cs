using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindAPI.Models;
using NorthwindViewModel;

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

        /// <summary>
        /// 查詢訂單 API
        /// GET：api/Orders
        ///      api/Orders?orderID=AAA
        ///      api/Orders?orderID=AAA&customerName=BBB
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="customerName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersDTO>>> GetOrders(int? orderID, string? customerName)
        {
            List<OrdersDTO> ordersDTOs = new List<OrdersDTO>();

            // 取得訂單資料
            var queryData = _context.Orders
                                    .Include(o => o.Customer)
                                    .Include(o => o.Employee)
                                    .AsQueryable();

            if (orderID != null && orderID > 0)
            {
                queryData = queryData.Where(o => o.OrderID == orderID);
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                queryData = queryData.Where(o => o.Customer.CompanyName == customerName);
            }

            // 將查詢資料轉為 DTO
            ordersDTOs = await queryData.Select(o => new OrdersDTO
            {
                OrderID = o.OrderID,
                CustomerName = o.Customer.CompanyName,
                EmployeeName = o.Employee.FirstName + " " + o.Employee.LastName,
                OrderDate = o.OrderDate,
                RequiredDate = o.RequiredDate,
                ShippedDate = o.ShippedDate,
            }).ToListAsync();

            return ordersDTOs;
        }

        /// <summary>
        /// 查詢單筆訂單 API
        /// GET：api/Orders/AAA
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [HttpGet("{orderID}")]
        public async Task<ActionResult<OrdersDTO>> GetOrders(int orderID)
        {
            OrdersDTO ordersDTO = new OrdersDTO();

            // 取得單筆訂單資料
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

            // 將資料轉為 DTO
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

        /// <summary>
        /// 更新訂單 API
        /// PUT：api/Orders/AAA
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="ordersDTO"></param>
        /// <returns></returns>
        [HttpPut("{orderID}")]
        public async Task<IActionResult> PutOrders(int orderID, OrdersDTO ordersDTO)
        {
            if (orderID != ordersDTO.OrderID)
            {
                return BadRequest();
            }

            // 取得單筆訂單資料
            var entity = await _context.Orders.FindAsync(orderID);

            if (entity == null)
            {
                return NotFound();
            }

            // 更新訂單資料
            try
            {
                entity.ShipVia = ordersDTO.ShipVia;
                entity.ShipName = ordersDTO.ShipName;
                entity.ShipAddress = ordersDTO.ShipAddress;
                entity.ShipCity = ordersDTO.ShipCity;
                entity.ShipRegion = ordersDTO.ShipRegion;
                entity.ShipPostalCode = ordersDTO.ShipPostalCode;
                entity.ShipCountry = ordersDTO.ShipCountry;

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

        /// <summary>
        /// 刪除訂單 API
        /// DELETE：api/Orders/AAA
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [HttpDelete("{orderID}")]
        public async Task<IActionResult> DeleteOrders(int orderID)
        {
            // 取得訂單明細資料 List
            var Order_Details = await _context.Order_Details.Where(o => o.OrderID == orderID).ToListAsync();

            if (Order_Details == null)
            {
                return NotFound();
            }

            // 取得單筆訂單資料
            var orders = await _context.Orders.FindAsync(orderID);

            if (orders == null)
            {
                return NotFound();
            }

            // 刪除訂單明細資料與訂單資料
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Order_Details.RemoveRange(Order_Details);
                    _context.Orders.Remove(orders);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return Ok();
        }

        /// <summary>
        /// 新增訂單 API
        /// POST：api/Orders
        /// </summary>
        /// <param name="ordersDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Orders>> PostOrders(OrdersDTO ordersDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 將訂單 DTO 資料轉為 EntityModel
                    Orders orders = new Orders()
                    {
                        CustomerID = ordersDTO.CustomerID,
                        EmployeeID = ordersDTO.EmployeeID,
                        OrderDate = ordersDTO.OrderDate,
                        RequiredDate = ordersDTO.RequiredDate,
                        ShippedDate = ordersDTO.ShippedDate,
                        ShipVia = ordersDTO.ShipVia,
                        Freight = ordersDTO.Freight,
                        ShipName = ordersDTO.ShipName,
                        ShipAddress = ordersDTO.ShipAddress,
                        ShipCity = ordersDTO.ShipCity,
                        ShipRegion = ordersDTO.ShipRegion,
                        ShipPostalCode = ordersDTO.ShipPostalCode,
                        ShipCountry = ordersDTO.ShipCountry
                    };

                    _context.Orders.Add(orders);
                    await _context.SaveChangesAsync();
                    
                    // 將明細 DTO 資料轉為 EntityModel
                    List<Order_Details> order_Details = ordersDTO.orderDetails.Select(o => new Order_Details()
                    {
                        OrderID = orders.OrderID,
                        ProductID = o.ProductID ?? 0,
                        UnitPrice = o.UnitPrice ?? 0,
                        Quantity = o.Quantity ?? 0,
                        Discount = o.Discount ?? 0
                    }).ToList();

                    _context.Order_Details.AddRange(order_Details);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return Ok();
        }

        /// <summary> 確認訂單是否存在 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool OrdersExists(int orderID)
        {
            return _context.Orders.Any(e => e.OrderID == orderID);
        }
    }
}
