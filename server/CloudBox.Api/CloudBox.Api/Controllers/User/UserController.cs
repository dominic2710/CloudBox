using CloudBox.Api.Functions.User;
using CloudBox.Api.Helpers;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudBox.Api.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private IUserFunction _userFunction;
        private UserOperator _userOperator;

        public UserController(IUserFunction userFunction, UserOperator userOperator)
        {
            _userFunction = userFunction;
            _userOperator = userOperator;
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var user = _userFunction.Authenticate(request.Email, request.Password);

            if (user == null)
                return BadRequest("Invalid username or password!");

            var response = new LoginResponse
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                Avatar = user.Avatar,
                LastLogonTime = user.LastLogonTime,
                PhoneNumber = user.PhoneNumber,
                SercurityGroupId = user.SercurityGroupId,
                Status = user.Status,
                AccessToken = user.AccessToken,
                UserName = user.UserName
            };

            return Ok(response);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            try
            {
                var response = _userFunction.Register(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult FetchMe()
        {
            var user = _userOperator.GetRequestUser();
            if (user == null)
                return BadRequest("Can't get user info. Please login again!");

            var response = new LoginResponse
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                Avatar = user.Avatar,
                LastLogonTime = user.LastLogonTime,
                PhoneNumber = user.PhoneNumber,
                SercurityGroupId = user.SercurityGroupId,
                Status = user.Status,
                AccessToken = user.AccessToken,
                UserName = user.UserName
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            return Ok(1);
        }
    }
}
