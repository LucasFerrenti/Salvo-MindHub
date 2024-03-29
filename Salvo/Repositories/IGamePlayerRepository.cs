﻿using Salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Repositories
{
    public interface IGamePlayerRepository
    {
        public GamePlayer GetGamePlayerView(long idGamePlayer);
        public GamePlayer FindById(long Id);
        public void Save(GamePlayer gamePlayer);
    }
}
