using Microsoft.AspNetCore.Mvc;
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
    public class GamesController : ControllerBase
    {
        private IGameRepository _repository;
        public GamesController(IGameRepository repository)
        {
            _repository = repository;
        }

        // GET: api/<GamesController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var games = _repository.GetAllGames();
                return Ok(games);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
