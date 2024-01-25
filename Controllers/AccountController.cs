using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MailProject.Models;
using MailProject.Requests;
using System.Security.Claims;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using MailProject.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace MailProject.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public AccountController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var identity = GetIdentity(request.UserName, request.Password);
            if(identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken
            (
                issuer: JWToptions.ISSUER,
                audience: JWToptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(JWToptions.LIFETIME)),
                signingCredentials: new SigningCredentials(JWToptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );
            var encodeJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodeJwt,
                username = identity.Name
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(User.Identity.AuthenticationType);
            return Ok(new {result="1", message="logged out successfuly"});
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            var test = User;
            return Ok($"Ваш логин: {User.Identity.Name}");
        }
        public ClaimsIdentity GetIdentity(string username, string password)
        {
            try
            {
                User user = _dbContext.User.FirstOrDefault(u => u.UserName == username && u.Password == password);
                //User user = _dbContext.User.FirstOrDefault();
                if (user != null)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName.ToString()),
                };
                    ClaimsIdentity claimsIdentity =
                        new(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    return claimsIdentity;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
