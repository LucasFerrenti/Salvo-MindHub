﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Salvo.Models
{
    public class SalvoContex : DbContext
    {
        public SalvoContex(DbContextOptions<SalvoContex> opt) : base(opt)
        {

        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
    }
}