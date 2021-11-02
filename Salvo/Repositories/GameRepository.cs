using Salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Repositories
{
    public class GameRepository : RepositoryBase<Game>, IGameRepository
    {
        public GameRepository(SalvoContex repositoryContext) : base(repositoryContext)
        {

        }

        public IEnumerable<Game> GetAllGames()
        {
            return FindAll().OrderBy(game => game.CreationDate).ToList();
        }
    }
}
