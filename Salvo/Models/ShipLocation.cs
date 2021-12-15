using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    public class ShipLocation
    {
        public long Id { get; set; }
        public long shipId { get; set; }
        public string Location { get; set; }

        public Ship Ships { get; set; }
    }
}
