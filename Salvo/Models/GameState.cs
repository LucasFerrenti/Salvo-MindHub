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
        WAITING_OPPONENT, //Esperando a un oponente
        WAITING_OPPONENT_SHIPS, //Esperando barcos del oponente
        WIN,
        LOSS,
        TIE
    }
}
