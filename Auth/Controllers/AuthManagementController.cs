using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Auth.Configurations;
using Auth.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Controllers
{
    [Route("[controller]")]
    [EnableCors]
    [ApiController]
    public class AuthManagementController : ControllerBase
    {
        private readonly ILogger<AuthManagementController> _logger;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly JwtConfig _jwtConfig;
        public AuthManagementController(ILogger<AuthManagementController> logger,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> _optionsMonitor
            )
        {
            _logger = logger;
            _userManager = userManager;
            _jwtConfig = _optionsMonitor.CurrentValue;
        }

        [HttpOptions]
        public IActionResult Options()
        {
            return Ok();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request payload.....");
            }

            var emailExits = await _userManager.FindByEmailAsync(requestDto.Email);
            if (emailExits != null)
            {
                return BadRequest("Email already exist.....");
            }

            var newUser = new IdentityUser()
            {
                Email = requestDto.Email,
                UserName = requestDto.Email
            };

            var isCreated = await _userManager.CreateAsync(newUser, requestDto.Password);

            if (!isCreated.Succeeded)
            {
                //return BadRequest("Error creating user. Please try again later.");
                return BadRequest(isCreated.Errors.Select(x => x.Description).ToList());
            }

            return Ok(new UserRegistrationResponse()
            {
                Result = true,
                Token = GenerateToken(newUser)
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto requestDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request payload....");
            }

            var existingUser = await _userManager.FindByEmailAsync(requestDto.Email);
            if (existingUser == null)
            {
                return BadRequest("User doesn't exist...");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(existingUser, requestDto.Password);

            if (!isPasswordValid)
            {
                return BadRequest("Wrong username or password...");
            }

            var token = GenerateToken(existingUser);
            return Ok(new UserLoginResponse()
            {
                Token = token,
                Result = true
            });
        }

        private string GenerateToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescripter = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescripter);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetUserProfile()
        {
            // Get the user's identity from the claims principle
            var identity = User.Identity as ClaimsIdentity;

            // Extract user details from claims
            var userId = identity.FindFirst("sub")?.Value;
            var userName = identity.FindFirst(ClaimTypes.Name)?.Value;
            var userEmail = identity.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new
            {
                UserId = userId,
                UserName = userName,
                UserEmail = userEmail
            });
        }
    }
}
