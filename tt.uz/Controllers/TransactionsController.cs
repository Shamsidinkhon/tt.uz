using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tt.uz.Helpers;

namespace tt.uz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        [HttpPost("payme")]
        public IActionResult Payme()
        {
            try
            {
                return Ok(new { result = new { }, id = 123 });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { error = new { code = ex.HResult, message = new { en = ex.Message } }, id = 123 });
            }
        }
    }
}