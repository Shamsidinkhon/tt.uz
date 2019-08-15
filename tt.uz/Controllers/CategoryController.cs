using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tt.uz.Entities;
using tt.uz.Services;

namespace tt.uz.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;

        public CategoryController(
           ICategoryService categoryService
            )
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Category> categories = _categoryService.GetAll();
            return Ok(categories);
        }
    }
}