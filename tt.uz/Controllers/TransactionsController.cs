using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using tt.uz.Helpers;
using tt.uz.Transactions;

namespace tt.uz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        public TransactionsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        [HttpPost("payme")]
        public IActionResult Payme([FromBody]Request request)
        {
            try
            {
                new Merchant().Authorize(Request, Configuration);
                return Ok(new { result = new { }, id = request.Id });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { error = new { code = ex.Code, message = new { en = ex.Message } }, id = request.Id });
            }
        }
    }
}