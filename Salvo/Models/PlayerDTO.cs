﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    public class PlayerDTO
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}