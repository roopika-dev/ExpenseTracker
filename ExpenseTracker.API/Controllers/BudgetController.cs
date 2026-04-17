using ExpenseTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _service;

        public BudgetController(IBudgetService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [HttpPost]
        public async Task<IActionResult> Set(int month, int year, decimal amount)
        {
            await _service.SetBudget(GetUserId(), month, year, amount);
            return Ok(new { message = "Budget set successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> Get(int month, int year)
        {
            var data = await _service.GetBudget(GetUserId(), month, year);
            return Ok(new { budget = data });
        }
    }
}