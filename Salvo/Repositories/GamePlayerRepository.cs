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
        public GamePlayerRepository(SalvoContext repositoryContext) : base (repositoryContext)
        {

        }
        public GamePlayer FindById(long id)
        {
            return FindByCondition(gp => gp.Id ==id)
                .Include(gp => gp.Player)
                .Include(gp => gp.Game)
                    .ThenInclude(game => game.GamePlayers)
                        .ThenInclude(gps => gps.Salvos)
                            .ThenInclude(salvos => salvos.Locations)
                .Include(gp => gp.Game)
                    .ThenInclude(game => game.GamePlayers)
                        .ThenInclude(gps => gps.Ships)
                            .ThenInclude(ships => ships.Locations)
                .FirstOrDefault();
        }
        public GamePlayer GetGamePlayerView(long idGamePlayer)
        {
            return FindAll(source => source
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
            //check 
            if (gamePlayer.Id == 0)
                Create(gamePlayer);
            else
                Update(gamePlayer);
            //save
            SaveChanges();
        }
    }
}
