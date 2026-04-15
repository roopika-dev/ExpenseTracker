using ExpenseTracker.UI.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExpenseTracker.UI.Controllers
{
    public class DashboardController : Controller
    {
        private ApiService _api = new ApiService();

        public async Task<ActionResult> Index()
        {
            var token = Session["Token"]?.ToString();

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            string fromDate = Request["fromDate"];
            string toDate = Request["toDate"];

            string url = "Dashboard";

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                url += $"?fromDate={fromDate}&toDate={toDate}";
            }

            var response = await _api.GetAsync(url, token);

            // 🔥 HANDLE TOKEN EXPIRED
            if (response == "UNAUTHORIZED")
            {
                Session.Clear(); // clear session immediately
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(response) || response == "null")
            {
                return View(new { total = 0, monthly = new object[0], category = new object[0], recent = new object[0] });
            }

            var data = JsonConvert.DeserializeObject(response);

            if (data == null)
            {
                return View(new { total = 0, monthly = new object[0], category = new object[0], recent = new object[0] });
            }

            return View((object)data);
        }
    }
}