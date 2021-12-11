using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salvo.Models;
using Salvo.Repositories;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private IPlayerRepository _repository;

        public PlayersController(IPlayerRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Post([FromBody] PlayerDTO player)
        {
            try
            {
                //validate email
                if (!ValidateEmail(player.Email))
                    return StatusCode(403, "Email invalido");
                //check is available
                Player dbPlayer = _repository.FindByEmail(player.Email);
                if (dbPlayer != null)
                    return StatusCode(403, "Email en uso");
                //validate user
                if (!ValidateUser(player.User))
                    return StatusCode(403, "Usuario invalido");
                //validate password
                if (!ValidatePassword(player.Password))
                    return StatusCode(403, "Contraceña invalida");
                //Create player
                Player newPlayer = new Player
                {
                    Email = player.Email,
                    Password = player.Password,
                    Name = player.User
                };
                //Save player in database
                _repository.Save(newPlayer);

                return StatusCode(201, newPlayer);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("available/{Email}", Name = "available")]
        public IActionResult Available(string email)
        {
            try
            {
                //check email
                if (!ValidateEmail(email))
                    return StatusCode(201, false);
                //search an return email status
                Player dbPlayer = _repository.FindByEmail(email);
                if (dbPlayer == null)
                    return StatusCode(201, true);
                else
                    return StatusCode(201, false);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        private bool ValidateEmail(string email)
        {
            //Check format
            email = email.ToLower(); //optional. It is to save all emails in lowercase.
            Regex emailPattern = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.\w{2,3})+)$");
            Match checkEmail = emailPattern.Match(email);
            if (!checkEmail.Success)
                return false;

            return true;
        }
        private bool ValidateUser(string user)
        {
            //check min length
            if (user.Length < 4)
                return false;
            //check max length
            if (user.Length > 16)
                return false;

            return true;
        }
        private bool ValidatePassword(string password)
        {

            Regex passwordPattern = new Regex(@"^((?=.*\d)|(?=.*\W+))(?=.*[a-z])(?=.*[A-Z])(?![.\n]).{8,100}$");
            Match checkPasswor = passwordPattern.Match(password);
            if (!checkPasswor.Success)
                return false;
            return true;
        }
    }
}
