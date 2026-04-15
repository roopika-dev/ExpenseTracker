using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IExpenseService _service;

        public DashboardController(IExpenseService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime? fromDate, DateTime? toDate)
        {
            var data = await _service.GetAll(GetUserId());

            // ✅ FILTER (NEW)
            if (fromDate.HasValue)
                data = data.Where(x => x.ExpenseDate >= fromDate.Value).ToList();

            if (toDate.HasValue)
                data = data.Where(x => x.ExpenseDate <= toDate.Value).ToList();

            var total = data.Sum(x => x.Amount);

            var monthly = data
                .GroupBy(x => x.ExpenseDate.Month)
                .Select(x => new
                {
                    month = x.Key,
                    total = x.Sum(a => a.Amount)
                });

            var category = data
                .GroupBy(x => x.CategoryName)
                .Select(x => new
                {
                    category = x.Key,
                    total = x.Sum(a => a.Amount)
                });

            return Ok(new
            {
                total,
                monthly,
                category,
                recent = data.OrderByDescending(x => x.ExpenseDate).Take(5)
            });
        }
    }
}