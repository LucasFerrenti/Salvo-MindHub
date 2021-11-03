using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                foreach (var g in games)
                {
                    contex.Games.Add(g);
                }
                contex.SaveChanges();
            }
        }
    }
}
