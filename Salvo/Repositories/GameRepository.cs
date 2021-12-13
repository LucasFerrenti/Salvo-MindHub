using Microsoft.EntityFrameworkCore;
using Salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Repositories
{
    public class GameRepository : RepositoryBase<Game>, IGameRepository
    {
        public GameRepository(SalvoContext repositoryContext) : base(repositoryContext)
        {

        }

        public Game FindById(long Id)
        {
            return FindByCondition(game => game.Id == Id)
                    .Include(game => game.GamePlayers)
                        .ThenInclude(gp => gp.Player)
                    .FirstOrDefault();
        }

        public IEnumerable<Game> GetAllGames()
        {
            return FindAll().OrderBy(game => game.CreationDate).ToList();
        }

        public IEnumerable<Game> GetAllGamesWithPlayers()
        {
            return FindAll(source => source
                    .Include(game => game.GamePlayers)
                        .ThenInclude(gameplayer => gameplayer.Player)
                            .ThenInclude(player => player.Scores))
                    .Include(game => game.GamePlayers)
                        .ThenInclude(gamePlayer => gamePlayer.Salvos)
                            .ThenInclude(salvo => salvo.Locations)
                    .Include(game => game.GamePlayers)
                        .ThenInclude(gamePlayer => gamePlayer.Ships)
                            .ThenInclude(ship => ship.Locations)
                    .OrderBy(game => game.CreationDate).ToList();
        }
    }
}
