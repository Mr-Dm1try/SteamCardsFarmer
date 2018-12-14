using System.ComponentModel.DataAnnotations;

namespace SteamCardsFarmer.Model.Types
{
    public class SteamGame {        
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
        public int CardsCount { get; set; }
        public double CardsAveragePrice { get; set; }
        public double ChanceToPayOff { get; set; }
        public bool HasCards { get; set; }
    }
}
