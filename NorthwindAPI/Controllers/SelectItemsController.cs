using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NorthwindAPI.Models;

namespace NorthwindAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelectItemsController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public SelectItemsController(NorthwindContext context)
        {
            _context = context;
        }

        /// <summary> 取得客戶代碼與姓名 </summary>
        /// <returns></returns>
        [HttpGet("CustomerID")]
        public async Task<List<SelectListItem>> GetCustomerID()
        {
            List<SelectListItem> selectListItems = GetBasicSelectLists();

            selectListItems.AddRange(await _context.Customers
                                                   .Select(o => new SelectListItem
                                                   {
                                                       Value = o.CustomerID,
                                                       Text = o.CompanyName,
                                                   }).ToListAsync());

            return selectListItems;
        }

        /// <summary> 取得公司代碼與姓名 </summary>
        /// <returns></returns>
        [HttpGet("EmployeeID")]
        public async Task<List<SelectListItem>> GetEmployeeID()
        {
            List<SelectListItem> selectListItems = GetBasicSelectLists();

            selectListItems.AddRange(await _context.Employees
                                                   .Select(o => new SelectListItem
                                                   {
                                                       Value = Convert.ToString(o.EmployeeID),
                                                       Text = o.FirstName + " " + o.LastName,
                                                   }).ToListAsync());

            return selectListItems;
        }

        /// <summary> 取得貨運公司代碼與姓名 </summary>
        /// <returns></returns>
        [HttpGet("ShipVia")]
        public async Task<List<SelectListItem>> GetShipVia()
        {
            List<SelectListItem> selectListItems = GetBasicSelectLists();

            selectListItems.AddRange(await _context.Shippers
                                                   .Select(o => new SelectListItem
                                                   {
                                                       Value = Convert.ToString(o.ShipperID),
                                                       Text = o.CompanyName,
                                                   }).ToListAsync());

            return selectListItems;
        }

        /// <summary> 取得產品代碼與名稱 </summary>
        /// <returns></returns>
        [HttpGet("ProductID")]
        public async Task<List<SelectListItem>> GetProductID()
        {
            List<SelectListItem> selectListItems = GetBasicSelectLists();

            selectListItems.AddRange(await _context.Products
                                                   .Select(o => new SelectListItem
                                                   {
                                                       Value = Convert.ToString(o.ProductID),
                                                       Text = o.ProductName,
                                                   }).ToListAsync());

            return selectListItems;
        }

        /// <summary> 取得基本下拉選單設定 </summary>
        /// <returns></returns>
        private List<SelectListItem> GetBasicSelectLists()
        {
            return new List<SelectListItem>() { new SelectListItem { Value = "", Text = "--- Please Select ---" } }; ;
        }
    }
}
