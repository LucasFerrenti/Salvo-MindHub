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

        [HttpGet("{id}", Name = "GetPlayer")]
        public void GetPlayer()
        {

        }

        [HttpPost]
        public IActionResult Post([FromBody] PlayerDTO player)
        {
            try
            {
                //Check data
                var result = CheckPlayerData(player);
                if (result.Value.ToString() != "Usuario validado")
                {
                    return result;
                }
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
        private ObjectResult CheckPlayerData(PlayerDTO player)
        {
            //Check Email--
            //Check input
            if (String.IsNullOrEmpty(player.Email))
                return StatusCode(403, "Email vacio");

            //Check format
            player.Email = player.Email.ToLower(); //optional. It is to save all emails in lowercase.
            Regex emailPattern = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.\w{2,3})+)$");
            Match checkEmail = emailPattern.Match(player.Email);
            if (!checkEmail.Success)
                return StatusCode(403, "Email invalido");

            //Check if available
            Player dbPlayer = _repository.FindByEmail(player.Email);
            if (dbPlayer != null)
                return StatusCode(403, "Email en uso");

            //Check password--
            //FULL REGEX FOR THE PASSWORD @"^((?=.*\d)|(?=.*\W+))(?=.*[a-z])(?=.*[A-Z])(?![.\n]).{8,16}$"
            //Check Input
            if (String.IsNullOrEmpty(player.Password))
                return StatusCode(403, "Contraseña vacia");

            //Check Length
            if (player.Password.Length > 16)
                return StatusCode(403, "Contraseña Invalida:\nDebe contener menos de 16 caracteres");
            if (player.Password.Length < 8)
                return StatusCode(403, "Contraseña Invalida:\nDebe contener al menos 8 caracteres");

            //Check uppercase
            Match checkMayus = new Regex(@"(?=.*[A-Z]).").Match(player.Password);
            if (!checkMayus.Success)
                return StatusCode(403, "Contraseña Invalida:\nDebe contener al menos una letra mayuscula");

            //Check lowercase
            Match checkMinus = new Regex(@"(?=.*[a-z]).").Match(player.Password);
            if (!checkMinus.Success)
                return StatusCode(403, "Contraseña Invalida:\nDebe contener al menos una letra minuscula");

            //Check number or special characters
            Match checkNumSpe = new Regex(@"((?=.*\d)|(?=.*\W+)).").Match(player.Password);
            if (!checkNumSpe.Success)
                return StatusCode(403, "Contraseña Invalida:\nDebe contener al menos un numero o caracter especial");

            //Check new line
            Match checkNewLine = new Regex(@"(?=.*\n).").Match(player.Password);
            if (checkNewLine.Success)
                return StatusCode(403, "Contraseña Invalida:\nNo puede contener saltos de linea");

            //Check user
            if (String.IsNullOrEmpty(player.User))
                return StatusCode(403, "Usuario Invalido");

            //passed all verifications
            return Ok("Usuario validado");
        }
    }
}
