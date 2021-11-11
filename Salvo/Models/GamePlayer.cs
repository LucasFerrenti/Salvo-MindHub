using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    public class GamePlayer
    {
        public long Id { get; set; }
        public long GameID { get; set; }
        public long PlayerID { get; set; }
        public DateTime? JoinDate { get; set; }

        public Game Game { get; set; }
        public Player Player { get; set; }
        public ICollection<Ship> Ships { get; set; }
        public ICollection<Salvo> Salvos { get; set; }

        public double? GetScore()
        {

            return Player.GetScore(Game);
        }
    }
}
