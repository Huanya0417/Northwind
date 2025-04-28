using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NorthwindViewModel;

namespace NorthwindWeb.Controllers
{
    public class OrdersController : Controller
    {
        public async Task<IActionResult> Index(string orderID, string customerName)
        {
            List<OrdersDTO> ordersDTOs = new List<OrdersDTO>();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7145/");
            HttpResponseMessage response = await client.GetAsync($"api/Orders?orderID={orderID}&customerName={customerName}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ordersDTOs = JsonConvert.DeserializeObject<List<OrdersDTO>>(json);
            }

            return View(ordersDTOs);
        }

        [Route("[action]/{orderID}")]
        public async Task<IActionResult> Detail(string orderID)
        {
            OrdersDTO ordersDTO = new OrdersDTO();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7145/");
            HttpResponseMessage response = await client.GetAsync($"api/Orders/{orderID}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ordersDTO = JsonConvert.DeserializeObject<OrdersDTO>(json);
            }

            return View(ordersDTO);
        }

        [HttpGet]
        [Route("[action]/{orderID}")]
        public async Task<IActionResult> Edit(string orderID)
        {
            //ViewBag.SelectDataList =
            //    new List<SelectListItem>() {
            //        new SelectListItem() { Value = "1", Text = "這是1",  },
            //        new SelectListItem() { Value = "2", Text = "這是2" },
            //        new SelectListItem() { Value = "3", Text = "這是3" },
            //        new SelectListItem() { Value = "4", Text = "這是4" },
            //        new SelectListItem() { Value = "5", Text = "這是5" }
            //    };

            //"api/SelectItem/Region"
            //    =>  List<SelectListItem>()
            //"api/SelectItem/Customer"
            //    =>  List<SelectListItem>()


            OrdersDTO ordersDTO = new OrdersDTO();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7145/");
            HttpResponseMessage response = await client.GetAsync($"api/Orders/{orderID}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ordersDTO = JsonConvert.DeserializeObject<OrdersDTO>(json);
            }

            return View(ordersDTO);
        }

        [HttpPost]
        [Route("[action]/{orderID}")]
        public async Task<IActionResult> Edit(string orderID, OrdersDTO ordersDTO)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7145/");

            string json = JsonConvert.SerializeObject(ordersDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync($"api/Orders/{orderID}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "更新失敗");
                return View(ordersDTO);
            }

            TempData["SuccessMsg"] = "修改成功";

            return RedirectToAction("Edit", "Orders");
        }
    }
}
