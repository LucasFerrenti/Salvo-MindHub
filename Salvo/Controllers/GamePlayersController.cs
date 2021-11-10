using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salvo.Repositories;
using Salvo.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/gamePlayers")]
    [ApiController]
    public class GamePlayersController : ControllerBase
    {
        private IGamePlayerRepository _repository;

        public GamePlayersController(IGamePlayerRepository repository)
        {
            _repository = repository;
        }

        // GET api/<GamePlayersController>/5
        [HttpGet("{id}", Name ="GetGameView")]
        public IActionResult GetGameView(int id)
        {
            try
            {
                var gp = _repository.GetGamePlayerView(id);
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

                    Ships = gp.Ships.Select(s => new ShipDTO 
                    {
                        Id = s.Id,
                        Type = s.Type,
                        Locations = s.Locations.Select(l => new ShipLocationDTO
                        {
                            Id = l.Id,
                            Location = l.Location
                        }).ToList()
                    }).ToList(),

                    Salvos = gp.Game.GamePlayers.SelectMany(gps => gps.Salvos).Select(sv => new SalvoDTO
                    {
                        Id = sv.Id,
                        turn = sv.turn,
                        Player = new PlayerDTO
                        {
                            Id = sv.GamePlayer.Player.Id,
                            Email = sv.GamePlayer.Player.Email
                        },
                        Locations = sv.Locations.Select(l => new SalvoLocationDTO
                        {
                            Id = l.Id,
                            Location = l.Location
                        }).ToList()
                    }).ToList()
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