using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Salvo.Models;
using Salvo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private IGameRepository _gameRepository;
        private IGamePlayerRepository _gpRepository;
        private IPlayerRepository _playerRepository;
        public GamesController(IGameRepository gameRepository, IPlayerRepository playerRepository, IGamePlayerRepository gamePlayerRepository)
        {
            _gameRepository = gameRepository;
            _gpRepository = gamePlayerRepository;
            _playerRepository = playerRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                //get games
                var games = _gameRepository.GetAllGamesWithPlayers().Select(g => new GameDTO
                    {
                        Id = g.Id,
                        CreationDate = g.CreationDate,
                        GamePlayers = g.GamePlayers.Select(gp => new GamePlayerDTO
                        {
                            Id = gp.Id,
                            JoinDate = gp.JoinDate,
                            Player = new PlayerDTO
                            {
                                Id = gp.Player.Id,
                                Email = gp.Player.Email
                            },
                            Point = gp.GetScore()
                        }).ToList()
                    }).ToList();
                //get email
                var email = User.FindFirst("Player");
                //create game list
                var gameList = new GameListDTO
                {
                    Email = email == null ? "Guest" : email.Value,
                    Games = games
                };

                return Ok(gameList);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                var userClaim = User.FindFirst("Player");
                var email = userClaim == null ? "Guest" : userClaim.Value;

                var player = _playerRepository.FindByEmail(email);
                var timeNow = DateTime.Now;

                GamePlayer gp = new GamePlayer
                {
                    Game = new Game
                    {
                        CreationDate = timeNow
                    },
                    PlayerID = player.Id,
                    JoinDate = timeNow,
                };

                _gpRepository.Save(gp);
                return Created("Game Creado", gp.Id);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
