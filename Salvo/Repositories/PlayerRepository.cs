using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salvo.Models;

namespace Salvo.Repositories
{
    public class PlayerRepository : RepositoryBase<Player>, IPlayerRepository
    {
        public PlayerRepository(SalvoContext repositiryContext): base(repositiryContext)
        {

        }
        public Player FindByEmail(string email)
        {
            return FindByCondition(player => player.Email == email).FirstOrDefault();
        }

        public void Save(Player player)
        {
            if (player.Id == 0)
                Create(player);
            else
                Update(player);
            //save
            SaveChanges();
        }
    }
}
