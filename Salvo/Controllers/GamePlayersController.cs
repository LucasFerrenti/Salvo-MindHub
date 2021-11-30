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
        [HttpPost("{id}/ships")]
        public IActionResult Post(long id, [FromBody]List<ShipDTO> ships)
        {
            try
            {
                //check gp exist
                GamePlayer gp = _repository.FindById(id);
                if (gp == null)
                    return StatusCode(403, "No existe el juego");
                //check player
                var userClaim = User.FindFirst("Player");
                var userEmail = userClaim == null ? "Guest" : userClaim.Value;
                if (gp.Player.Email != userEmail)
                    return StatusCode(403, "El usuario no se encuentra en el juego");
                //check ships
                if (gp.Ships.Count >= 5)
                    return StatusCode(403, "Ya se han posicionado los barcos");
                //save ships
                gp.Ships = ships.Select(ships => new Ship
                {
                    GamePlayerId = gp.Id,
                    Type = ships.Type,
                    Locations = ships.Locations.Select(loc => new ShipLocation
                    {
                        shipId = ships.Id,
                        Location = loc.Location
                    }).ToList(),
                }).ToList();
                _repository.Save(gp);
                return StatusCode(201, "barcos posicionados!!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("{id}/salvos")]
        public IActionResult Post(long id, [FromBody]SalvoDTO salvo)
        {
            try
            {
                //check gp exist
                GamePlayer gamePlayer = _repository.FindById(id);
                if (gamePlayer == null)
                    return StatusCode(403, "No existe el juego");

                //check player
                var userClaim = User.FindFirst("Player");
                var userEmail = userClaim == null ? "Guest" : userClaim.Value;
                if (gamePlayer.Player.Email != userEmail)
                    return StatusCode(403, "El usuario no se encuentra en el juego");

                //check oponent
                GamePlayer oponent = gamePlayer.GetOponent();
                if (oponent == null)
                    return StatusCode(403, "No hay oponente");

                //check ships
                if (gamePlayer.Ships.Count == 0)
                    return StatusCode(403, "Falta posicionar tus barcos");
                if (oponent.Ships.Count == 0)
                    return StatusCode(403, "Falta posicionar los barcos del oponente");

                //check turns
                GamePlayer creator = gamePlayer.Game.CreationDate == gamePlayer.JoinDate ? gamePlayer : oponent;
                int gpTurn = gamePlayer.Salvos.Count;
                int opTurn = oponent.Salvos.Count;
                int diffTurn = gpTurn - opTurn;
                
                //save
                if (creator == gamePlayer && diffTurn >= 1 || creator == oponent && diffTurn >= 0)
                {
                    return StatusCode(403, "no se puede adelantar");
                }

                gamePlayer.Salvos.Add(new Models.Salvo
                {
                    GamePlayerId = gamePlayer.Id,
                    Locations = salvo.Locations.Select(loc => new SalvoLocation
                    {
                        Location = loc.Location,
                    }).ToList(),
                    turn = gpTurn + 1
                });
                
                _repository.Save(gamePlayer);
                return StatusCode(201, "salvos disparados!!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}