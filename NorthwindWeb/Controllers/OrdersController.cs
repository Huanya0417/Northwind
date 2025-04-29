using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NorthwindViewModel;
using System.Text;

namespace NorthwindWeb.Controllers
{
    public class OrdersController : Controller
    {
        /// <summary> 查詢所有訂單資料，並可依照條件進行篩選 </summary>
        /// <param name="orderID"></param>
        /// <param name="customerName"></param>
        /// <returns></returns>
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

        #region Detail

        /// <summary> 查詢訂單明細資料 </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
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

            // 取得頁面所需的下拉式選單內容
            await GetSelectListItem(client);

            return View(ordersDTO);
        }

        #endregion Detail

        #region Edit

        /// <summary> 修改頁面查詢訂單明細資料 </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
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

            // 取得頁面所需的下拉式選單內容
            await GetSelectListItem(client);

            return View(ordersDTO);
        }

        /// <summary> 修改頁面更新資料 </summary>
        /// <param name="orderID"></param>
        /// <param name="ordersDTO"></param>
        /// <returns> 導引至修改頁面 </returns>
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

            return Redirect(orderID);
        }

        #endregion Edit

        #region Delete

        /// <summary> 刪除頁面查詢訂單明細資料 </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
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

            // 取得頁面所需的下拉式選單內容
            await GetSelectListItem(client);

            return View(ordersDTO);
        }

        /// <summary> 刪除頁面刪除資料 </summary>
        /// <param name="orderID"></param>
        /// <returns> 導引至所有訂單頁面 </returns>
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

        #endregion Delete

        #region Add

        /// <summary> 新增頁面 </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Add")]
        public async Task<IActionResult> Add()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7145/");

            // 取得頁面所需的下拉式選單內容
            await GetSelectListItem(client);

            return View();
        }

        /// <summary> 新增頁面新增資料 </summary>
        /// <param name="orderID"></param>
        /// <param name="ordersDTO"></param>
        /// <returns> 導引至所有訂單頁面 </returns>
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> PostAdd(string orderID, OrdersDTO ordersDTO)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7145/");

            string json = JsonConvert.SerializeObject(ordersDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"api/Orders", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "新增失敗");
                return View(ordersDTO);
            }

            TempData["SuccessMsg"] = "新增成功";

            return RedirectToAction("index", "Orders");
        }

        #endregion Add

        #region Method

        /// <summary> 取得頁面所需的下拉式選單內容 </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task GetSelectListItem(HttpClient client)
        {
            HttpResponseMessage response;

            // 取得 CustomerID 下拉選單內容
            response = await client.GetAsync("api/SelectItems/CustomerID");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ViewBag.CustomerIDList = JsonConvert.DeserializeObject<List<SelectListItem>>(json);
            }

            // 取得 EmployeeID 下拉選單內容
            response = await client.GetAsync("api/SelectItems/EmployeeID");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ViewBag.EmployeeIDList = JsonConvert.DeserializeObject<List<SelectListItem>>(json);
            }

            // 取得 ShipVia 下拉選單內容
            response = await client.GetAsync("api/SelectItems/ShipVia");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ViewBag.ShipViaList = JsonConvert.DeserializeObject<List<SelectListItem>>(json);
            }

            // 取得 ProductID 下拉選單內容
            response = await client.GetAsync("api/SelectItems/ProductID");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                ViewBag.ProductIDList = JsonConvert.DeserializeObject<List<SelectListItem>>(json);
            }
        }

        #endregion Method
    }
}
