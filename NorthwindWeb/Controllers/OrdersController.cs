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

        [Route("Detail/{orderID}")]
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
        [Route("Edit/{orderID}")]
        public async Task<IActionResult> Edit(string orderID)
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

            // 取得 ShipVia 下拉選單內容
            response = await client.GetAsync($"api/SelectItems/ShipVia");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ViewBag.ShipViaList = JsonConvert.DeserializeObject<List<SelectListItem>>(json);
            }

            return View(ordersDTO);
        }

        [HttpPost]
        [Route("Edit/{orderID}")]
        public async Task<IActionResult> PostEdit(string orderID, OrdersDTO ordersDTO)
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

        [HttpGet]
        [Route("Delete/{orderID}")]
        public async Task<IActionResult> Delete(string orderID)
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

        [HttpPost]
        [Route("Delete/{orderID}")]
        public async Task<IActionResult> PostDelete(string orderID)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7145/");

            HttpResponseMessage response = await client.DeleteAsync($"api/Orders/{orderID}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "刪除失敗");
                return RedirectToAction("Delete", "Orders");
            }

            TempData["SuccessMsg"] = "刪除成功";

            return RedirectToAction("index", "Orders");
        }
    }
}
