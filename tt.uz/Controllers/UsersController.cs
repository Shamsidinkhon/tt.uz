using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;
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
        private IExternalLoginService _externalLoginService;
        private ITempUserService _userTempService;
        private IMapper _mapper;
        private IVerificationCodeService _vcodeService;
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _clientFactory;
        private IHttpContextAccessor _httpContextAccessor;

        public UsersController(
            IUserService userService,
            ITempUserService userTempService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IVerificationCodeService vcodeService,
            IHttpClientFactory clientFactory,
            IExternalLoginService externalLoginService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _userTempService = userTempService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _vcodeService = vcodeService;
            _clientFactory = clientFactory;
            _externalLoginService = externalLoginService;
            _httpContextAccessor = httpContextAccessor;
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
        public IActionResult Register([FromBody]TempUserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<TempUser>(userDto);

            try
            {
                // save 
                _userTempService.Create(user, userDto.Password, userDto.IsEmail);
                return Ok(new { status = true });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { status = false, message = ex.Message });
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
                    vCode.Code,
                    vCode.IsEmail
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
        [HttpPost("facebook")]
        public IActionResult Facebook([FromBody]AccessTokenDTO model)
        {
            var client = _clientFactory.CreateClient();
            // 1.generate an app access token
            var appAccessTokenResponse = client.GetAsync($"https://graph.facebook.com/oauth/access_token?client_id={_appSettings.FacebookAppId}&client_secret={_appSettings.FacebookAppSecret}&grant_type=client_credentials");
            var responseString = appAccessTokenResponse.Result.Content.ReadAsStringAsync().Result;
            AccessTokenDTO appAccessToken = JsonConvert.DeserializeObject<AccessTokenDTO>(responseString);
            // 2. validate the user access token
            var userAccessTokenValidationResponse = client.GetAsync($"https://graph.facebook.com/debug_token?input_token={model.AccessToken}&access_token={appAccessToken.AccessToken}");
            var userAccessResponseString = userAccessTokenValidationResponse.Result.Content.ReadAsStringAsync().Result;
            var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessResponseString);

            if (!userAccessTokenValidation.Data.IsValid)
            {
                return BadRequest(new { status = false, message = "Invalid facebook token." });
            }

            // 3. we've got a valid token so we can request user data from fb
            var userInfoResponse = client.GetAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={model.AccessToken}");
            var userInfoResponseString = userInfoResponse.Result.Content.ReadAsStringAsync().Result;
            var userInfo = JsonConvert.DeserializeObject<ExternalLoginDTO>(userInfoResponseString);

            // 4. ready to create the local user account (if necessary) and jwt
            var user = _userService.FindByEmail(userInfo.Email);

            if (user == null)
            {
                var appUser = new User
                {
                    Email = userInfo.Email,
                };
                _userService.CreateExternalUser(appUser);
            }

            // generate the jwt for the local user...
            var localUser = _userService.FindByEmail(userInfo.Email);

            if (localUser == null)
            {
                return BadRequest(new { status = false, message = "Failed to create local user account." });
            }

            _externalLoginService.CreateOrUpdate(localUser, userInfo, ExternalLogin.FACEBOOK);



            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, localUser.Id.ToString())
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
                    Id = localUser.Id,
                    Email = localUser.Email,
                    Phone = localUser.Phone,
                    Token = tokenString
                }
            });
        }

        [HttpPost("user-balance")]
        public int UserBalance()
        {
            int userId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
            var user = _userService.GetById(userId);
            return user.Balance;
        }

        [AllowAnonymous]
        [HttpPost("get-profile")]
        public UserProfile GetProfile(int userId)
        {
            var profile = _userService.GetProfile(userId, Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name));
            return profile;
        }
    }
}