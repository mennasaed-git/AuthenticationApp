using AuthenticationApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;
        public AuthController( IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] User logindata)
        {
           
             

            if (AuthenticateUser(logindata))
            {
                var claims = new[]
                     {

                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                        new Claim("userName",logindata.UserName)
                    };
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var signin = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                                    _config["Jwt:Issuer"],
                                    _config["Jwt:Audience"],
                                    claims,
                                    expires: DateTime.Now.AddMinutes(120),
                                    signingCredentials: signin);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }

            return BadRequest("Invalid Credentials");
        }

       private bool AuthenticateUser(User user)
        {
            if(user.UserName == "admin" && user.Password == "P@$$w0rd")
            {
                return true;
            }
            return false;
        }
    }
}
