using Microsoft.EntityFrameworkCore;
using Salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Repositories
{
    public class GamePlayerRepository : RepositoryBase<GamePlayer>, IGamePlayerRepository
    {
        public GamePlayerRepository(SalvoContex repositoryContext) : base (repositoryContext)
        {

        }
        public GamePlayer GetGamePlayerView(int idGamePlayer)
        {
            return FindAll(source => source.Include(gamePlayer => gamePlayer.Ships)
                                                .ThenInclude(ship => ship.Locations)
                                            .Include(gamePlayer => gamePlayer.Salvos)
                                                .ThenInclude(salvo => salvo.Locations)
                                            .Include(gamePlayer => gamePlayer.Game)
                                                .ThenInclude(game => game.GamePlayers)
                                                    .ThenInclude(gp => gp.Player)
                                            .Include(gamePlayer => gamePlayer.Game)
                                                .ThenInclude(game => game.GamePlayers)
                                                    .ThenInclude(gp => gp.Salvos)
                                                    .ThenInclude(salvo => salvo.Locations)
                                            .Include(gamePlayer => gamePlayer.Game)
                                                .ThenInclude(game => game.GamePlayers)
                                                    .ThenInclude(gp => gp.Ships)
                                                    .ThenInclude(ship => ship.Locations))
                                    .Where(gamePlayer => gamePlayer.Id == idGamePlayer)
                                    .OrderBy(game => game.JoinDate)
                                    .FirstOrDefault();
        }

        public void Save(GamePlayer gamePlayer)
        {
            Create(gamePlayer);
            SaveChanges();
        }
    }
}
