using ExpenseTracker.UI.Models;
using ExpenseTracker.UI.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExpenseTracker.UI.Controllers
{
    public class AccountController : Controller
    {
        private ApiService _api = new ApiService();

        // ================= LOGIN GET =================
        public ActionResult Login()
        {
            return View();
        }

        // ================= LOGIN POST =================
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            var response = await _api.PostAsync("Auth/login", model);

            if (string.IsNullOrEmpty(response))
            {
                TempData["Error"] = "Server error. Try again.";
                return RedirectToAction("Login");
            }

            // 🔥 HANDLE UNAUTHORIZED (safety)
            if (response == "UNAUTHORIZED")
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login");
            }

            // ✅ SUCCESS → TOKEN (NOT JSON)
            if (!response.Trim().StartsWith("{"))
            {
                Session["Token"] = response.Trim();

                // 🔥 GET USER DETAILS
                var profileResponse = await _api.GetAsync("Auth/profile", Session["Token"].ToString());

                // 🔥 HANDLE TOKEN ISSUE DURING PROFILE CALL
                if (profileResponse == "UNAUTHORIZED")
                {
                    Session.Clear();
                    TempData["Error"] = "Session expired. Please login again.";
                    return RedirectToAction("Login");
                }

                if (!string.IsNullOrEmpty(profileResponse))
                {
                    dynamic user = JsonConvert.DeserializeObject(profileResponse);

                    Session["UserName"] = user.name;
                    Session["Email"] = user.email;
                }

                TempData["Success"] = "Login successful!";
                return RedirectToAction("Index", "Dashboard");
            }

            // ❌ ERROR RESPONSE (JSON)
            dynamic result = JsonConvert.DeserializeObject(response);

            TempData["Error"] = result?.message != null
                ? result.message.ToString()
                : "Invalid email or password";

            return RedirectToAction("Login");
        }

        // ================= REGISTER GET =================
        public ActionResult Register()
        {
            return View();
        }

        // ================= REGISTER POST =================
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var response = await _api.PostAsync("Auth/register", model);

            TempData["Success"] = "Registration successful! Please login.";

            return RedirectToAction("Login");
        }

        // ================= LOGOUT =================
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}