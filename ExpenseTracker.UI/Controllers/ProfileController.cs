using ExpenseTracker.UI.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExpenseTracker.UI.Controllers
{
    public class ProfileController : Controller
    {
        private ApiService _api = new ApiService();

        // ================= PROFILE GET =================
        public async Task<ActionResult> Index()
        {
            var token = Session["Token"]?.ToString();

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var response = await _api.GetAsync("Auth/profile", token);

            dynamic data = JsonConvert.DeserializeObject(response);

            // ✅ FIX: UPDATE SESSION HERE ALSO
            if (data != null)
            {
                Session["UserName"] = data.name;
                Session["Email"] = data.email;
            }

            return View((object)data);
        }

        // ================= UPDATE PROFILE =================
        [HttpPost]
        public async Task<ActionResult> Update(string name, string email)
        {
            var token = Session["Token"]?.ToString();

            var dto = new
            {
                name = name,
                email = email
            };

            await _api.PutAsync("Auth/profile", dto, token);

            // ✅ UPDATE SESSION
            Session["UserName"] = name;
            Session["Email"] = email;

            TempData["Success"] = "Profile updated successfully!";

            return RedirectToAction("Index");
        }

        // ================= CHANGE PASSWORD =================
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            var token = Session["Token"]?.ToString();

            var dto = new
            {
                oldPassword = oldPassword,
                newPassword = newPassword
            };

            var response = await _api.PostAsync("Auth/change-password", dto, token);

            // ✅ SAFER CHECK
            if (!string.IsNullOrEmpty(response) && response.ToLower().Contains("incorrect"))
            {
                TempData["Error"] = "Old password is incorrect";
            }
            else
            {
                TempData["Success"] = "Password changed successfully!";
            }

            return RedirectToAction("Index");
        }
    }
}