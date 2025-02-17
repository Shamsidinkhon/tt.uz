﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tt.uz.Dtos;
using tt.uz.Entities;
using tt.uz.Helpers;
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

        [AllowAnonymous]
        [HttpGet("get-category-attributes")]
        public IActionResult GetCategoryAttributes(int Id)
        {
            try
            {
                List<CoreAttribute> attributes = _categoryService.GetCategoryAttribites(Id);
                return Ok(attributes);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
            
        }

        [AllowAnonymous]
        [HttpGet("update-categories")]
        public IActionResult UpdateCategories()
        {
            try
            {
                return Ok(_categoryService.UpdateCategories());
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
            
        }
    }
}