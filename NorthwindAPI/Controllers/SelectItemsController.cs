using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("ShipVia")]
        public async Task<List<SelectListItem>> GetShipVia()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems = await _context.Shippers
                                            .Select(o => new SelectListItem
                                            {
                                                Value = Convert.ToString(o.ShipperID),
                                                Text = o.CompanyName,
                                            }).ToListAsync();

            return selectListItems;
        }
    }
}
