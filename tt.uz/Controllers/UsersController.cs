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
        private ITempUserService _userTempService;
        private IMapper _mapper;
        private IVerificationCodeService _vcodeService;
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _clientFactory;


        public UsersController(
            IUserService userService,
            ITempUserService userTempService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IVerificationCodeService vcodeService,
            IHttpClientFactory clientFactory)
        {
            _userService = userService;
            _userTempService = userTempService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _vcodeService = vcodeService;
            _clientFactory = clientFactory;
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
                return Ok(new { status = true});
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
            var userAccessTokenValidation = JsonConvert.DeserializeObject(userAccessResponseString);

            //if (!userAccessTokenValidation.Data.IsValid)
            //{
            //    return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid facebook token.", ModelState));
            //}

            //// 3. we've got a valid token so we can request user data from fb
            //var userInfoResponse = await Client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={model.AccessToken}");
            //var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);

            //// 4. ready to create the local user account (if necessary) and jwt
            //var user = await _userManager.FindByEmailAsync(userInfo.Email);

            //if (user == null)
            //{
            //    var appUser = new AppUser
            //    {
            //        FirstName = userInfo.FirstName,
            //        LastName = userInfo.LastName,
            //        FacebookId = userInfo.Id,
            //        Email = userInfo.Email,
            //        UserName = userInfo.Email,
            //        PictureUrl = userInfo.Picture.Data.Url
            //    };

            //    var result = await _userManager.CreateAsync(appUser, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));

            //    if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            //    await _appDbContext.Customers.AddAsync(new Customer { IdentityId = appUser.Id, Location = "", Locale = userInfo.Locale, Gender = userInfo.Gender });
            //    await _appDbContext.SaveChangesAsync();
            //}

            //// generate the jwt for the local user...
            //var localUser = await _userManager.FindByNameAsync(userInfo.Email);

            //if (localUser == null)
            //{
            //    return BadRequest(Errors.AddErrorToModelState("login_failure", "Failed to create local user account.", ModelState));
            //}

            //var jwt = await Tokens.GenerateJwt(_jwtFactory.GenerateClaimsIdentity(localUser.UserName, localUser.Id), _jwtFactory, localUser.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });

            //return new OkObjectResult(jwt);
            return Ok();
        }
    }
}