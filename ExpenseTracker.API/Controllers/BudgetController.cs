using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    public class BudgetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
