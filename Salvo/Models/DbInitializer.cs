using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salvo.Models;

namespace Salvo.Models
{
    public static class DbInitializer
    {
        public static void Initialize(SalvoContex contex)
        {
            if (!contex.Players.Any())
            {
                // Create Instance of Player
                Player[] players = new Player[]
                {
                    new Player
                    {
                        Name = "Jack Bauer",
                        Email = "j.bauer@ctu.gov",
                        Password = "24",
                    },
                    new Player
                    {
                        Name = "Chloe O'Brian",
                        Email = "c.obrian@ctu.gov",
                        Password = "42",
                    },
                    new Player
                    {
                        Name = "Kim Bauer",
                        Email = "kim_bauer@gmail.com",
                        Password = "kb",
                    },
                    new Player
                    {
                        Name = "Tony Almeida",
                        Email = "t.almeida@ctu.gov",
                        Password = "mole",
                    },
                };
                //Add Contex
                foreach (var p in players)
                {
                    contex.Players.Add(p);
                }
                //Save Contex
                contex.SaveChanges();
            }
            
            if (!contex.Games.Any())
            {
                // Create Instance of Games
                DateTime time = DateTime.Now;
                Game[] games = new Game[]
                {
                    new Game { CreationDate = time},
                    new Game { CreationDate = time = time.AddHours(1) },
                    new Game { CreationDate = time = time.AddHours(1) },
                    new Game { CreationDate = time = time.AddHours(1) },
                    new Game { CreationDate = time = time.AddHours(1) },
                    new Game { CreationDate = time = time.AddHours(1) },
                    new Game { CreationDate = time = time.AddHours(1) },
                    new Game { CreationDate = time = time.AddHours(1) }
                };
                //Add Contex
                foreach (var g in games)
                {
                    contex.Games.Add(g);
                }
                //Save Contex
                contex.SaveChanges();
            }

            if (!contex.GamePlayers.Any())
            {
                Game game1 = contex.Games.Find(1L);
                Game game2 = contex.Games.Find(2L);
                Game game3 = contex.Games.Find(3L);
                Game game4 = contex.Games.Find(4L);
                Game game5 = contex.Games.Find(5L);
                Game game6 = contex.Games.Find(6L);
                Game game7 = contex.Games.Find(7L);
                Game game8 = contex.Games.Find(8L);

                Player player1 = contex.Players.Find(1L);
                Player player2 = contex.Players.Find(2L);
                Player player3 = contex.Players.Find(3L);
                Player player4 = contex.Players.Find(4L);

                // Create Instance of GP
                GamePlayer[] gamePlayers = new GamePlayer[]
                {                    
                    new GamePlayer { Game = game1, Player = player1, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game1, Player = player2, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game2, Player = player1, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game2, Player = player2, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game3, Player = player2, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game3, Player = player4, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game4, Player = player2, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game4, Player = player1, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game5, Player = player4, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game5, Player = player1, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game6, Player = player3, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game7, Player = player4, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game8, Player = player3, JoinDate = DateTime.Now },
                    new GamePlayer { Game = game8, Player = player4, JoinDate = DateTime.Now },
                };
                //Add Contex
                foreach (var gp in gamePlayers)
                {
                    contex.GamePlayers.Add(gp);
                }
                //Save Contex
                contex.SaveChanges();
            }
        }
    }
}
