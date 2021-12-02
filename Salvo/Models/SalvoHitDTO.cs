using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    public class SalvoHitDTO
    {
        public int Turn { get; set; }
        public ICollection<ShipHitDTO> Hits { get; set; }
    }
}
