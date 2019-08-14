using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tt.uz.Services;

namespace tt.uz.Controllers
{
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
        public ActionResult Get()
        {
            return _categoryService.GetAll;
        }
    }
}