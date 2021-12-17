using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salvo.Models;
using Salvo.Utilities.Encrypt;
namespace Salvo.Models
{
    public static class DbInitializer
    {
        public static void Initialize(SalvoContext context)
        {
            if (!context.Players.Any())
            {
                // Create Instance of Players
                Player[] players = new Player[]
                {
                    new Player
                    {
                        Name = "Text0",
                        Email = "test0@test.com",
                        Password = Encrypt.GetSHA256("test0"),
                        IsActive = true,
                        ActivationCode = ""
                    },
                    new Player
                    {
                        Name = "Text1",
                        Email = "test1@test.com",
                        Password = Encrypt.GetSHA256("test1"),
                        IsActive = true,
                        ActivationCode = ""
                    },
                    new Player
                    {
                        Name = "Text2",
                        Email = "test2@test.com",
                        Password = Encrypt.GetSHA256("test2"),
                        IsActive = true,
                        ActivationCode = ""
                    },
                    new Player
                    {
                        Name = "Text3",
                        Email = "test3@test.com",
                        Password = Encrypt.GetSHA256("test3"),
                        IsActive = true,
                        ActivationCode = ""
                    },
                    new Player
                    {
                        Name = "Text4",
                        Email = "test4@test.com",
                        Password = Encrypt.GetSHA256("test4"),
                        IsActive = true,
                        ActivationCode = ""
                    },
                };
                //Add Contex
                foreach (var p in players)
                {
                    context.Players.Add(p);
                }
                //Save Contex
                context.SaveChanges();
            }
        }
    }
}
