using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tt.uz.Dtos;
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
        [HttpGet("get-all")]
        public IActionResult Get()
        {
            IEnumerable<CategoryDTO> categories = _categoryService.GetAll();
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("get-with-children")]
        public IActionResult GetWithChildren()
        {
            IEnumerable<CategoryDTO> categories = _categoryService.GetWithChildren();
            return Ok(categories);
        }
    }
}