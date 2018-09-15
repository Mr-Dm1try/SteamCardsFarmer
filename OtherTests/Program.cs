using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;

namespace OtherTests {
    class Program {
        
        static void Main(string[] args) {
            Random newR = new Random(45);
            //Pomoika();
            for (int i = 0; i < 16; i++) {
                int m = newR.Next(5, 16);
                for (int j = 0; j < m; j++)
                    Console.Write(Math.Round(newR.NextDouble()*(7.1 - 1.1) + 1.1, 2) + "; ");
                Console.WriteLine();
            }
            Console.ReadKey();
        }

        static void Pomoika() {
            //TODO: Currency synchronization 
            double syncUSD = 0.31;
            double syncRUB = 21.24;
            double oneUSDinRUB = syncRUB / syncUSD;

            var baseUrl = "https://steamcommunity.com/market/search?category_753_Game[]=tag_app_450860&category_753_cardborder[]=tag_cardborder_0&category_753_item_class[]=tag_item_class_2&page=1#p1_price_desc";
            var url = baseUrl;
            Dictionary<string, double> cards = new Dictionary<string, double>();

            using (var client = new WebClient()) {
                var html = client.DownloadString(url);
                var document = new HtmlDocument();
                document.LoadHtml(html);

                var cardNodes = document.DocumentNode.SelectNodes(@"//div[@id = 'searchResults']/div[@id = 'searchResultsTable']/div[@id = 'searchResultsRows']/a[@class = 'market_listing_row_link']");               

                foreach (var node in cardNodes) {
                    var title = node.SelectSingleNode(@"div[1]/div[2]/span[1]").InnerText;
                    var price = node.SelectSingleNode(@"div[1]/div[1]/div[2]/span[1]/span[1]").InnerText;
                    price = price.Split(new[] { " " }, StringSplitOptions.None)[0].Remove(0, 1).Replace('.', ',');

                    cards.Add(title, double.Parse(price) * oneUSDinRUB);

                    Console.WriteLine($"Finded: {title} | {price}");
                }

                var cardsCount = document.DocumentNode.SelectSingleNode(@"//div[@id = 'searchResults_ctn']/div[2]/span[@id = 'searchResults_total']").InnerText;
            }                        
        }
    }
}
