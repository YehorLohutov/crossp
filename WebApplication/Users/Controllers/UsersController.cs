using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Users.Repository;
using Users.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Users.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("users-management")]
    public class UsersController : ControllerBase
    {        
        private readonly IConfiguration configuration;
        private readonly IUsersRepository usersRepository;

        public UsersController(IConfiguration configuration, IUsersRepository usersRepository)
        {
            this.configuration = configuration;
            this.usersRepository = usersRepository;
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AllowAnonymous]
        [Route("version")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public ActionResult<string> Version(ApiVersion apiVersion) => Ok($"Active {nameof(UsersController)} API ver is {apiVersion}");

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [Route("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Token))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Token>> Login([FromQuery] string username, [FromQuery] string password)
        {
            User user;
            ClaimsIdentity identity;
            try
            {
                user = await usersRepository.GetUserBy(username, password);
                identity = GetIdentity(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            string issuer;
            string audience;
            DateTime now;
            int lifetime;
            SymmetricSecurityKey symmetricSecurityKey;

            try
            {
                IConfigurationSection tokenOptions = configuration.GetSection("TokenOptions");
                issuer = tokenOptions.GetSection("issuer").Value;
                audience = tokenOptions.GetSection("audience").Value;
                now = DateTime.UtcNow;
                lifetime = int.Parse(tokenOptions["lifetime"]);
                string key = tokenOptions["key"];
                symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while creating access token");
            }

            JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(lifetime)),
                    signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256));
            
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            Token token = new Token(identity.Name, user.ExternalId, encodedJwt);
            return Ok(token);
        }

        [NonAction]
        private ClaimsIdentity GetIdentity(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim("UserExternalId", user.ExternalId),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "Default")
            };
            
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, 
                "Token", 
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
                );
            
            return claimsIdentity;
        }
    }
}
