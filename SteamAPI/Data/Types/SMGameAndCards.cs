using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SteamAPI.Data.Types {
    public class SMGameAndCards {
        [Key]
        public SSGame Game { get; set; }
        public int CardsCount { get; set; }
        public double CardsAveragePrice { get; set; }
        public double ChanceToPayOff { get; set; }
    }
}
