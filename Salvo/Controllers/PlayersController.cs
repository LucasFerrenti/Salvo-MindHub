using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salvo.Models;
using Salvo.Repositories;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;

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

                string code = Guid.NewGuid().ToString();
                SendEmailConfirmation(player.Email, code);

                //Create player
                Player newPlayer = new Player
                {
                    Email = player.Email,
                    Password = player.Password,
                    Name = player.User,
                    IsActive = false,
                    ActivationCode = code
                };
                //Save player in database
                _repository.Save(newPlayer);


                return StatusCode(201, "Registro Completado");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("available/{email}", Name = "available")]
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

        [HttpPost("activate/{email}/{code}", Name = "activate")]
        public IActionResult Activate(string email, string code)
        {
            try
            {
                //check input email
                if (!ValidateEmail(email))
                    return StatusCode(403, "No se encontro al usuario");
                //search player
                var player = _repository.FindByEmail(email);
                //check player exist
                if (player == null)
                    return StatusCode(403, "No se encontro al usuario");
                //check player already active
                if (player.IsActive)
                    return StatusCode(403, "El usuario se encuentra activado");
                //check activation code
                if (player.ActivationCode != code)
                {
                    player.ActivationCode = "";
                    _repository.Save(player);
                    return StatusCode(403, "Codigo de activacion invalido, genere uno nuevo");
                }
                //active and save player
                player.ActivationCode = "";
                player.IsActive = true;
                _repository.Save(player);
                return Ok("usuario activado");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("resend/{email}",Name = "resend")]
        public IActionResult ReSendEmailActivation(string email)
        {
            try
            {
                if (!ValidateEmail(email))
                    return StatusCode(403, "Usuario invalido");
                var player = _repository.FindByEmail(email);
                if (player == null)
                    return StatusCode(403, "Usuario invalido");
                if (player.IsActive)
                    return StatusCode(403, "El usuario se encuentra activado");
                string code = Guid.NewGuid().ToString();
                SendEmailConfirmation(email, code);
                return Ok("Email reenviado");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
            
        private bool ValidateEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;
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
        private void SendEmailConfirmation(string email, string code)
        {
            string myEmail = "salvo.game.g1@gmail.com";
            string url = Request.Host.Value;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = new NetworkCredential(myEmail, "Salvo2021");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(myEmail, "Salvo Game");
            mail.To.Add(new MailAddress(email));
            mail.Subject = "Verificar Cuenta";
            mail.Body = "https://" + url + "/activate.html?user=" + email + "/" + code;
            client.Send(mail);
            client.Dispose();
        }
    }
}
