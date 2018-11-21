using System.ComponentModel.DataAnnotations;

namespace GUI.Models.Types
{
    public class SMGameAndCards {
        [Key]
        public int Key { get; set; }
        public SSGame Game { get; set; }
        public int CardsCount { get; set; }
        public double CardsAveragePrice { get; set; }
        public double ChanceToPayOff { get; set; }
    }
}
