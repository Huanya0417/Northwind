using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NorthwindAPI.Models;
using NorthwindViewModel;

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

        [HttpGet("CustomerID")]
        public async Task<List<SelectListItem>> GetCustomerID()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>() { new SelectListItem { Value = "", Text = "--- Please Select ---"} };

            selectListItems.AddRange(await _context.Customers
                                                   .Select(o => new SelectListItem
                                                   {
                                                       Value = o.CustomerID,
                                                       Text = o.CompanyName,
                                                   }).ToListAsync());

            return selectListItems;
        }

        [HttpGet("EmployeeID")]
        public async Task<List<SelectListItem>> GetEmployeeID()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>() { new SelectListItem { Value = "", Text = "--- Please Select ---" } };

            selectListItems.AddRange(await _context.Employees
                                                   .Select(o => new SelectListItem
                                                   {
                                                       Value = Convert.ToString(o.EmployeeID),
                                                       Text = o.FirstName + " " + o.LastName,
                                                   }).ToListAsync());

            return selectListItems;
        }

        [HttpGet("ShipVia")]
        public async Task<List<SelectListItem>> GetShipVia()
        {
            //List<SelectListItem> selectListItems = new List<SelectListItem>();

            //selectListItems = await _context.Shippers
            //                                .Select(o => new SelectListItem
            //                                {
            //                                    Value = Convert.ToString(o.ShipperID),
            //                                    Text = o.CompanyName,
            //                                }).ToListAsync();

            List<SelectListItem> selectListItems = new List<SelectListItem>() { new SelectListItem { Value = "", Text = "--- Please Select ---" } };

            selectListItems.AddRange(await _context.Shippers
                                                   .Select(o => new SelectListItem
                                                   {
                                                       Value = Convert.ToString(o.ShipperID),
                                                       Text = o.CompanyName,
                                                   }).ToListAsync());

            return selectListItems;
        }

        [HttpGet("ProductID")]
        public async Task<List<SelectListItem>> GetProductID()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>() { new SelectListItem { Value = "", Text = "--- Please Select ---" } };

            selectListItems.AddRange(await _context.Products
                                                   .Select(o => new SelectListItem
                                                   {
                                                       Value = Convert.ToString(o.ProductID),
                                                       Text = o.ProductName,
                                                   }).ToListAsync());

            return selectListItems;
        }
    }
}
