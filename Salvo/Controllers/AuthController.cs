using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salvo.Repositories;
using Salvo.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Salvo.Utilities.Encrypt;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IPlayerRepository _repository;

        public AuthController(IPlayerRepository repository)
        {
            _repository = repository;
        }

        //LOGIN
        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] PlayerDTO player)
        {
            try
            {
                Player user = _repository.FindByEmail(player.Email);
                // check player exist
                if (user == null)
                    return StatusCode(401, "Unauthorized: Email invalido");
                // check password
                if (!String.Equals(user.Password, Encrypt.GetSHA256(player.Password)))
                    return StatusCode(401, "Unauthorized: Contraceña incorrecta");
                // check is active
                if (!user.IsActive)
                    return StatusCode(403, "Usuario sin activar, verifique su correo electronico para activarlo");
                //login
                var claims = new List<Claim>
                    {
                        new Claim("Player", user.Email)
                    };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        //LOGOUT
        // POST: api/Auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        //GetAuth
        // POST: api/Auth/getauth
        [HttpGet("getauth")]
        public IActionResult GetAuth()
        {
            try
            {
                var userClaim = User.FindFirst("Player");
                var a = userClaim == null ? "Guest" : userClaim.Value;
                return StatusCode(201, a);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
                throw;
            }
        }
    }
}
