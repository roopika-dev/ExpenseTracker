using ExpenseTracker.Core.DTOs.Expense;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Infrastructure.Services; // ✅ IMPORTANT
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

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
        public async Task<IActionResult> Get()
        {
            var data = await _service.GetAll(GetUserId());
            return Ok(data);
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
            await _service.Add(GetUserId(), dto);
            return Ok();
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
        [AllowAnonymous] // ✅ ADD THIS
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAll();
            return Ok(categories);
        }
    }
}