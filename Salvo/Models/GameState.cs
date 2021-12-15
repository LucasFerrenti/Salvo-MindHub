using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    public enum GameState
    {
        PLACE_SHIPS,
        WAIT,
        ENTER_SALVO,
        WIN,
        LOSS,
        TIE
    }
}
