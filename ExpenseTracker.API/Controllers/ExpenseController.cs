using ExpenseTracker.Core.DTOs.Expense;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Infrastructure.Services; // ✅ IMPORTANT
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _service;
        private readonly CategoryService _categoryService;

        public ExpenseController(IExpenseService service, CategoryService categoryService)
        {
            _service = service;
            _categoryService = categoryService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        // ===================== GET ALL =====================
        [HttpGet]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 5)
        {
            var data = await _service.GetAll(GetUserId());

            var totalCount = data.Count;

            var pagedData = data
                .OrderByDescending(x => x.ExpenseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                data = pagedData
            });
        }
        // ===================== GET BY ID =====================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetAll(GetUserId());

            var item = data.FirstOrDefault(x => x.Id == id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // ===================== ADD =====================
        [HttpPost]
        public async Task<IActionResult> Add(CreateExpenseDto dto)
        {
            try
            {
                await _service.Add(GetUserId(), dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // ===================== UPDATE =====================
        [HttpPut]
        public async Task<IActionResult> Update(UpdateExpenseDto dto)
        {
            await _service.Update(GetUserId(), dto);
            return Ok();
        }

        // ===================== DELETE =====================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(GetUserId(), id);
            return Ok();
        }

        // ===================== GET CATEGORIES =====================
        [HttpGet("categories")]
        [AllowAnonymous] //ADD THIS
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAll();
            return Ok(categories);
        }

        [HttpGet("export")]
        [AllowAnonymous] // allow query token
        public async Task<IActionResult> Export(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                // manually set user from token
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var userId = int.Parse(jwt.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

                var data = await _service.GetAll(userId);

                var csv = new StringBuilder();
                csv.AppendLine("Category,Amount,Date,Notes");

                foreach (var item in data)
                {
                    csv.AppendLine($"{item.CategoryName},{item.Amount},{item.ExpenseDate:yyyy-MM-dd},{item.Notes}");
                }

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());

                return File(bytes, "text/csv", "Expenses.csv");
            }

            return Unauthorized();
        }
    }
}