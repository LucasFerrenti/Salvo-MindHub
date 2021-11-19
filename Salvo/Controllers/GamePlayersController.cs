using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salvo.Repositories;
using Salvo.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/gamePlayers")]
    [ApiController]
    [Authorize("PlayerOnly")]
    public class GamePlayersController : ControllerBase
    {
        private IGamePlayerRepository _repository;

        public GamePlayersController(IGamePlayerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}", Name ="GetGameView")]
        public IActionResult GetGameView(int id)
        {
            try
            {
                var userEmail = User.FindFirst("Player").Value;

                var gp = _repository.GetGamePlayerView(id);

                if (gp.Player.Email != userEmail)
                    return Forbid();

                var gameView = new GameViewDTO
                {
                    Id = gp.Id,
                    CreationDate = gp.Game.CreationDate,

                    GamePlayers = gp.Game.GamePlayers.Select(gps => new GamePlayerDTO
                    {
                        Id = gps.Id,
                        JoinDate = gps.JoinDate,
                        Player = new PlayerDTO
                        {
                            Id = gps.Player.Id,
                            Email = gps.Player.Email
                        }
                    }).ToList(),

                    Ships = gp.Ships.Select(ship => new ShipDTO 
                    {
                        Id = ship.Id,
                        Type = ship.Type,
                        Locations = ship.Locations.Select(location => new ShipLocationDTO
                        {
                            Id = location.Id,
                            Location = location.Location
                        }).ToList()
                    }).ToList(),

                    Salvos = gp.Game.GamePlayers.SelectMany(gps => gps.Salvos.Select(salvo => new SalvoDTO
                    {
                        Id = salvo.Id,
                        turn = salvo.turn,
                        Player = new PlayerDTO
                        {
                            Id = gps.Player.Id,
                            Email = gps.Player.Email
                        },
                        Locations = salvo.Locations.Select(location => new SalvoLocationDTO
                        {
                            Id = location.Id,
                            Location = location.Location
                        }).ToList()
                    })).ToList()
                };

                return Ok(gameView);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}