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
            var t = GetOponent();

            List<string> salvoLocations = 
                GetOponent()?.Salvos
                    .Where(salvo => salvo.turn <= lastTurn)
                    .SelectMany(salvo => salvo.Locations)
                    .Select(salvoLoc => salvoLoc.Location).ToList();
            return Ships?.Where(ship => ship.Locations.Select(shipLoc => shipLoc.Location)
                    .All(shipLoc => salvoLocations == null ? false : salvoLocations.Contains(shipLoc)))
                    .Select(ship => ship.Type).ToList();

        }
        public GameState GetGameState()
        {
            var opponent = GetOponent();
            int myShips = Ships.Count;
            int? opShips = opponent?.Ships.Count;

            //place ships
            if (myShips == 0 )
                return GameState.PLACE_SHIPS;

            //wait
            if (opponent == null || opShips == null || opShips == 0)
                return GameState.WAIT;

            int myTurn = Salvos.Count;
            int opTurn = opponent.Salvos.Count;
            int mySunks = GetSunks().Count;
            int opSunks = opponent.GetSunks().Count;

            //game over?
            if (myTurn == opTurn)
            {
                //tie
                if (mySunks == 5 && opSunks == 5)
                    return GameState.TIE;
                //loss
                if (mySunks == 5)
                    return GameState.LOSS;
                //win
                if (opSunks == 5)
                    return GameState.WIN;
            }

            var creator = JoinDate == Game.CreationDate;

            //enter salvo and wait of creator
            if (creator)
            {
                if (myTurn <= opTurn)
                    return GameState.ENTER_SALVO;
                if (myTurn > opTurn)
                    return GameState.WAIT;
            }

            //enter salvo and wait of joined
            if (!creator)
            {
                if (myTurn < opTurn)
                    return GameState.ENTER_SALVO;
                if (myTurn >= opTurn)
                    return GameState.WAIT;
            }

            return GameState.ENTER_SALVO;
        }
    }
}
