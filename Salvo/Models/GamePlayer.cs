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

        public GamePlayer GetOponent()
        {
            return Game.GamePlayers.FirstOrDefault(gps => gps.Id != Id);
        }
        
        public ICollection<SalvoHitDTO> GetHits()
        {
            return Salvos.Select(salvo => new SalvoHitDTO
            {
                Turn = salvo.turn,
                Hits = GetOponent()?.Ships.Select(ship => new ShipHitDTO
                {
                    Type = ship.Type,
                    Hits = salvo.Locations
                            .Where(salvoLoc => ship.Locations.Any(shipLoc => shipLoc.Location == salvoLoc.Location))
                            .Select(salvoLoc => salvoLoc.Location).ToList()
                }).ToList()
            }).ToList();
        }
        public ICollection<string> GetSunks()
        {
            int lastTurn = Salvos.Count;
            List<string> salvoLocations = 
                GetOponent()?.Salvos
                    .Where(salvo => salvo.turn <= lastTurn)
                    .SelectMany(salvo => salvo.Locations)
                    .Select(salvoLoc => salvoLoc.Location).ToList();
            return
                Ships?.Where(ship => ship.Locations.Select(shipLoc => shipLoc.Location)
                    .All(shipLoc => salvoLocations.Contains(shipLoc)))
                    .Select(ship => ship.Type).ToList();
            //var yop = Ships?.Where(ship => ship.Locations.Select(shipLoc => shipLoc.Location)
            //            .All(shipLoc => salvoLocations.Contains(shipLoc)))
            //            .Select(ship => ship.Type).ToList();
            //var prof = Ships?.Where(ship => ship.Locations.Select(shipLoc => shipLoc.Location)
            //        .All(shipLoc => salvoLocations != null ? salvoLocations.Any(salvoLoc => salvoLoc == shipLoc) : false))
            //        .Select(ship => ship.Type).ToList();
        }
    }
}
