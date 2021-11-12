using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    public class Player
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public ICollection<GamePlayer> GamePlayers { get; set; }
        public ICollection<Score> Scores { get; set; }

        public double? GetScore(Game game)
        {
            var score = Scores.FirstOrDefault(sc => sc.GameId == game.Id);
            return score != null ? score.Point : null;
        }
    }
}
