using ExpenseTracker.UI.Models;
using ExpenseTracker.UI.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExpenseTracker.UI.Controllers
{
    public class ExpenseController : Controller
    {
        private ApiService _api = new ApiService();

        // ===================== LIST =====================
        public async Task<ActionResult> Index(string category)
        {
            var token = Session["Token"]?.ToString();

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var response = await _api.GetAsync("Expense", token);

            if (string.IsNullOrEmpty(response) || !response.Trim().StartsWith("["))
                return View(new List<ExpenseViewModel>());

            var data = JsonConvert.DeserializeObject<List<ExpenseViewModel>>(response);

            // ✅ APPLY FILTER
            if (!string.IsNullOrEmpty(category))
            {
                data = data
                   .Where(x => x.CategoryName == category)
                    .ToList();
            }

            // ✅ ADD ICON + COLOR
            AddCategoryStyles(data);

            // ✅ SEND CATEGORY LIST FOR DROPDOWN
            ViewBag.FilterCategories = data
                .Select(x => x.CategoryName)
                .Distinct()
                .ToList();

            return View(data);
        }

        // ===================== CREATE GET =====================
        public async Task<ActionResult> Create()
        {
            await LoadCategories();
            return View();
        }

        // ===================== CREATE POST =====================
        [HttpPost]
        public async Task<ActionResult> Create(ExpenseViewModel model)
        {
            var token = Session["Token"]?.ToString();

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var dto = new
            {
                categoryId = model.CategoryId,
                amount = model.Amount,
                expenseDate = model.ExpenseDate,
                notes = model.Notes
            };

            await _api.PostAsync("Expense", dto, token);

            TempData["Success"] = "Expense added successfully!"; // ✅ ADD

            return RedirectToAction("Index");
        }

        // ===================== EDIT GET =====================
        public async Task<ActionResult> Edit(int id)
        {
            var token = Session["Token"]?.ToString();

            var response = await _api.GetAsync($"Expense/{id}", token);

            var data = JsonConvert.DeserializeObject<ExpenseViewModel>(response);

            await LoadCategories(); // VERY IMPORTANT

            return View(data);
        }

        // ===================== EDIT POST =====================
        [HttpPost]
        public async Task<ActionResult> Edit(ExpenseViewModel model)
        {
            var token = Session["Token"]?.ToString();

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var dto = new
            {
                id = model.Id,
                categoryId = model.CategoryId,
                amount = model.Amount,
                expenseDate = model.ExpenseDate,
                notes = model.Notes
            };

            await _api.PutAsync("Expense", dto, token);

            TempData["Success"] = "Expense updated successfully!"; // ✅ ADD

            return RedirectToAction("Index");
        }

        // ===================== DELETE =====================
        public async Task<ActionResult> Delete(int id)
        {
            var token = Session["Token"]?.ToString();

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            await _api.DeleteAsync($"Expense/{id}", token);

            TempData["Success"] = "Expense deleted successfully!"; // ✅ ADD

            return RedirectToAction("Index");
        }

        // ===================== COMMON CATEGORY METHOD =====================
        private void SetCategories()
        {
            ViewBag.Categories = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Food" },
                new SelectListItem { Value = "2", Text = "Fuel" },
                new SelectListItem { Value = "3", Text = "Travel" },
                new SelectListItem { Value = "4", Text = "Bills" },
                new SelectListItem { Value = "5", Text = "Shopping" },
                new SelectListItem { Value = "6", Text = "Juices" },
                new SelectListItem { Value = "7", Text = "Chai" },
                new SelectListItem { Value = "8", Text = "Smoking" }
            };
        }

        private async Task LoadCategories()
        {
            var token = Session["Token"]?.ToString();

            var response = await _api.GetAsync("Expense/categories", token);

            // ✅ STEP 1: Check response
            if (string.IsNullOrEmpty(response) || !response.Trim().StartsWith("["))
            {
                ViewBag.Categories = new List<SelectListItem>();
                return;
            }

            var data = JsonConvert.DeserializeObject<List<CategoryViewModel>>(response);

            //STEP 2: Check null
            if (data == null)
            {
                ViewBag.Categories = new List<SelectListItem>();
                return;
            }

            //STEP 3: Map
            ViewBag.Categories = data.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();
        }

        private void AddCategoryStyles(List<ExpenseViewModel> data)
        {
            foreach (var item in data)
            {
                switch (item.CategoryName)
                {
                    case "Fuel":
                        item.Icon = "⛽";
                        item.Color = "orange";
                        break;

                    case "Food":
                    case "Dining":
                        item.Icon = "🍔";
                        item.Color = "green";
                        break;

                    case "Shopping":
                    case "Amazon":
                    case "Flipkart":
                        item.Icon = "🛒";
                        item.Color = "blue";
                        break;

                    case "Bills":
                        item.Icon = "💡";
                        item.Color = "red";
                        break;

                    case "Travel":
                        item.Icon = "🚗";
                        item.Color = "purple";
                        break;

                    default:
                        item.Icon = "📌";
                        item.Color = "gray";
                        break;
                }
            }
        }
    }
}