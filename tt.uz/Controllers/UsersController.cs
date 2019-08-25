using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using tt.uz.Dtos;
using tt.uz.Entities;
using tt.uz.Helpers;
using tt.uz.Services;

namespace tt.uz.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private IVerificationCodeService _vcodeService;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IVerificationCodeService vcodeService)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _vcodeService = vcodeService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserDto userDto)
        {
            var user = userDto.IsEmail ? _userService.AuthenticateWithEmail(userDto.Email, userDto.Password) : _userService.AuthenticateWithPhone(userDto.Phone, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                status = true,
                userData = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    Phone = user.Phone,
                    Token = tokenString
                }
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try
            {
                // save 
                _userService.Create(user, userDto.Password, userDto.IsEmail);
                return Ok(new { status = true, message = "User Data Saved" });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { status = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("validate")]
        public IActionResult Validate([FromBody]VCodeDto vCode)
        {
            try
            {
                return Ok(new
                {
                    status = _vcodeService.Verify(
                    vCode.IsEmail ? vCode.Email : vCode.Phone,
                    vCode.IsEmail ? VerificationCode.EMAIL : VerificationCode.PHONE,
                    vCode.Code
                    ),
                    message = "Verification completed"
                });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { status = false, message = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("signin/{provider}")]
        public IActionResult SignIn(string provider, string returnUrl = null) =>
        Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/" }, provider);

    }
}