using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NorthwindViewModel;

namespace NorthwindWeb.Controllers
{
    public class OrdersController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<OrdersDTO> ordersDTOs = new List<OrdersDTO>();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7145/");
            HttpResponseMessage response = await client.GetAsync("api/Orders");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ordersDTOs = JsonConvert.DeserializeObject<List<OrdersDTO>>(json);
            }

            return View(ordersDTOs);
        }
    }
}
