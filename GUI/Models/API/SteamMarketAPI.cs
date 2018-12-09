using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using GUI.Models;
using GUI.Models.Types;

namespace GUI.Models.API {
    public class SteamMarketAPI {
        private readonly SteamGamesContext context;
        private readonly String baseUrl = "https://steamcommunity.com/market/search?category_753_Game[]=tag_app_*gameID*&category_753_cardborder[]=tag_cardborder_0&category_753_item_class[]=tag_item_class_2#p1_price_desc";
        
        //TODO: Currency synchronization 
        private static double syncUSD = 0.31;
        private static double syncRUB = 21.24;
        private static double OneUSDinRUB = syncRUB / syncUSD;

        private static double incomeRatio = 0.8695652173913043;     // Если умножить цену на это число, то отсеется комиссия ТП стима

        public List<SSGame> Games { get; set; }

        public SteamMarketAPI(List<SSGame> games) {
            context = new SteamGamesContext();
            Games = games;
        }

        public List<SMGameAndCards> GetGamesWithCards() {
            var result = context.SMGamesWithCards.ToList();
            if (result.Count > 0)
                return result;
            else
                throw new Exception("Database is empty!");
        }

        public void WeedOutGames() {
            foreach (var entity in context.SMGamesWithCards.ToList())
                context.SMGamesWithCards.Remove(entity);

            List<SMGameAndCards> gamesAndCards = new List<SMGameAndCards>();
            foreach (var game in Games) {
                List<double> cardPrices = GetCardPrices(game, out int cardsCount);

                double sumOfAdditionalCards = (cardsCount != cardPrices.Count) ? (cardsCount - cardPrices.Count) * cardPrices.Last() : 0;
                double cardsAvgPrice = (cardPrices.Sum() + sumOfAdditionalCards) / cardsCount;

                double chanceToPayOff = GetChanceToPayOff(game.Price, cardPrices, cardsCount);

                gamesAndCards.Add(new SMGameAndCards {
                    Game = game,
                    CardsCount = cardsCount,
                    CardsAveragePrice = cardsAvgPrice,
                    ChanceToPayOff = chanceToPayOff
                });                     
            }

            foreach (var game in gamesAndCards) 
                context.SMGamesWithCards.Add(game);

            context.SaveChanges();
        }

        private double GetChanceToPayOff(double gamePrice, List<double> cardPrices, int cardsCount) { 
            cardPrices.Sort((a, b) => b.CompareTo(a));                              //Сортировка по убыванию цены
            int dropCardsCount = Convert.ToInt32(Math.Ceiling(cardsCount / 2.0));   //Количество выпадающих карт

            if (cardPrices.Take(dropCardsCount).Sum() * incomeRatio < gamePrice)    //чисто эмпирическая штука
                return 0.1;

            int positiveCases = 0;                                                  //Позитивные суммы цен карт (больше цены игры)

            int allPossibleCases = 1;                                               //Количество возможных событий:
            int temp = cardsCount;                                                  // Подсчитывается с предположением,
            for (int i = 1; i <= dropCardsCount; i++)                               // что одна карта не выпадает 
                allPossibleCases *= (i % 2 == 0) ? temp-- : temp;                   // более 2 раз

            int[] usesCount = new int[cardPrices.Count];                            //Подсчет использований карточек

            BruteForce(gamePrice, cardPrices, usesCount, dropCardsCount, ref positiveCases);

            return (double)positiveCases / allPossibleCases;                        //Вероятность удачного исхода
        }

        private bool BruteForce(double gamePrice, List<double> cardPrices, int[] usesCount, int dropCardsCount, ref int positiveCases) {
            int depth = usesCount.Sum();                                            //Количество использованных карточек

            if (depth < dropCardsCount) {
                int index = cardPrices.Count - 1;
                while (index > 0 && usesCount[index] == 0) { index--; }
                if (usesCount[index] == 2)
                    index++;

                bool flag = true;

                while (flag && index < usesCount.Length - dropCardsCount + 2) {
                    usesCount[index]++;
                    flag = BruteForce(gamePrice, cardPrices, usesCount, dropCardsCount, ref positiveCases);
                    usesCount[index]--;
                    index++;
                }

                return flag;                                                        //Целесообразность дальнейших вычислений
            }
            else if (depth == dropCardsCount) {
                double sum = 0;
                for (int i = 0; i < usesCount.Length; i++)                          //Подсчет суммы выбранных карт с учетом возможных повторений
                    sum += usesCount[i] * cardPrices[i];

                sum *= incomeRatio;                                                 //Минус комиссия ТП

                positiveCases += Arrangement(usesCount.Where(a => a != 0).Count(), dropCardsCount);    //Прибавление всех возможных размещений выбранных карт к положительным исходам

                return sum > gamePrice;                                             //Целесообразность дальнейших вычислений
            }
            else
                throw new Exception("Unexpected error! Something went wrong!");
        }

        private int Arrangement(int K, int N) {
            if (K > N)
                throw new ArgumentOutOfRangeException("K > N!!!");
            else {
                int result = 1;

                for (int n = N; n > N - K; n--)
                    result *= n;

                return result;
            }
        }

        private List<double> GetCardPrices(SSGame game, out int cardsCount) {
            string url = baseUrl.Replace("*gameID*", game.Key.ToString());
            List<double> cards = new List<double>();

            using (var client = new WebClient()) {
                var html = client.DownloadString(url);
                var document = new HtmlDocument();
                document.LoadHtml(html);

                var cardNodes = document.DocumentNode.SelectNodes(@"//div[@id = 'searchResults']/div[@id = 'searchResultsTable']/div[@id = 'searchResultsRows']/a[@class = 'market_listing_row_link']");

                foreach (var node in cardNodes) {
                    var price = node.SelectSingleNode(@"div[1]/div[1]/div[2]/span[1]/span[1]").InnerText;
                    price = price.Split(new[] { " " }, StringSplitOptions.None)[0].Remove(0, 1).Replace('.', ',');

                    cards.Add(double.Parse(price) * OneUSDinRUB);
                }

                cardsCount = int.Parse(document.DocumentNode.SelectSingleNode(@"//div[@id = 'searchResults_ctn']/div[2]/span[@id = 'searchResults_total']").InnerText);
            }

            return cards;
        }
    }
}
