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
        private IGamePlayerRepository _gpRepository;
        private IScoreRepository _scoreRepository;

        public GamePlayersController(IGamePlayerRepository gpRepository, IScoreRepository scoreRepository)
        {
            _gpRepository = gpRepository;
            _scoreRepository = scoreRepository;
        }

        [HttpGet("{id}", Name ="GetGameView")]
        public IActionResult GetGameView(int id)
        {
            try
            {
                var userEmail = User.FindFirst("Player").Value;

                var gp = _gpRepository.GetGamePlayerView(id);

                
                if (gp?.Player.Email != userEmail)
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
                    })).ToList(),
                    Hits = gp.GetHits(),
                    HitsOpponent = gp.GetOponent()?.GetHits(),
                    Sunks = gp.
                    GetSunks(),
                    SunksOpponent = gp.GetOponent()?.GetSunks(),
                    GameState = Enum.GetName(typeof(GameState), gp.GetGameState())
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
                GamePlayer gp = _gpRepository.FindById(id);
                if (gp == null)
                    return StatusCode(403, "No existe el juego");

                //check player
                var userClaim = User.FindFirst("Player");
                var userEmail = userClaim == null ? "Guest" : userClaim.Value;
                if (gp.Player.Email != userEmail)
                    return StatusCode(403, "El usuario no se encuentra en el juego");

                //check ships
                if (gp.Ships.Count == 5)
                    return StatusCode(403, "Ya se han posicionado los barcos");
                
                //check ships input
                if (ships.Count != 5)
                {
                    return StatusCode(403, "Solo se pueden colocar 5 barcos");
                }
                
                //save
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
                _gpRepository.Save(gp);
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
                GamePlayer gamePlayer = _gpRepository.FindById(id);
                if (gamePlayer == null)
                    return StatusCode(403, "No existe el juego");

                //check player
                var userClaim = User.FindFirst("Player");
                var userEmail = userClaim == null ? "Guest" : userClaim.Value;
                if (gamePlayer.Player.Email != userEmail)
                    return StatusCode(403, "El usuario no se encuentra en el juego");

                //check game state
                var finishStates = new List<GameState> { GameState.WIN, GameState.LOSS, GameState.TIE };
                if (finishStates.Contains(gamePlayer.GetGameState()))
                    return StatusCode(403, "Juego finalizado");

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
                var creator = gamePlayer.JoinDate == gamePlayer.Game.CreationDate;
                int gpTurn = gamePlayer.Salvos.Count;
                int opTurn = oponent.Salvos.Count;
                int diffTurn = gpTurn - opTurn;

                if (creator && diffTurn >= 1 || !creator && diffTurn >= 0)
                    return StatusCode(403, "no se puede adelantar");

                //check salvos
                if (salvo.Locations.Count != 5)
                    return StatusCode(403, "Solo se Pueden Ingresar 5 salvos");

                //save
                gamePlayer.Salvos.Add(new Models.Salvo
                {
                    GamePlayerId = gamePlayer.Id,
                    Locations = salvo.Locations.Select(loc => new SalvoLocation
                    {
                        Location = loc.Location,
                    }).ToList(),
                    turn = gpTurn + 1
                });
                _gpRepository.Save(gamePlayer);

                //check game state and save the score
                switch (gamePlayer.GetGameState())
                {
                    case GameState.WIN:
                        SubmitScore(1, gamePlayer);
                        SubmitScore(0, oponent);
                        break;
                    case GameState.LOSS:
                        SubmitScore(0, gamePlayer);
                        SubmitScore(1, oponent);
                        break;
                    case GameState.TIE:
                        SubmitScore(0.5, gamePlayer);
                        SubmitScore(0.5, oponent);
                        break;
                }

                return StatusCode(201, "salvos disparados!!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        private void SubmitScore(double point, GamePlayer gp)
        {
            var score = new Score
            {
                Point = point,
                PlayerId = gp.PlayerID,
                GameId = gp.GameID,
                FinishDate = DateTime.UtcNow
            };
            _scoreRepository.Save(score);
        }
    }
}