﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;

using HtmlAgilityPack;

using SteamCardsFarmer.Model.Types;

namespace SteamCardsFarmer.Model.API {
    /// <summary>Класс работает с карточками игр.</summary>
    public class SteamMarketAPI {
        private readonly SteamGamesContext context;
        private readonly string baseUrl = "https://steamcommunity.com/market/search?category_753_Game[]=tag_app_*gameID*&category_753_cardborder[]=tag_cardborder_0&category_753_item_class[]=tag_item_class_2#p1_price_desc";
        
        //TODO: Currency synchronization 
        private static double syncUSD = 0.31;
        private static double syncRUB = 21.24;
        private static double OneUSDinRUB = syncRUB / syncUSD;

        private static double incomeRatio = 0.86958;     // Если умножить цену на это число, то отсеется комиссия ТП стима

        private List<SteamGame> _games;

        /// <summary> Событие обработки игры просеивателем </summary>
        public event Action<String> GameHasBeenWeededOut;

        /// <summary>Конструктор класса. Задает связи с базой данных.</summary>
        public SteamMarketAPI() {
            context = new SteamGamesContext();
            _games = context.SteamGames.ToList();
        }

        /// <summary>Получение игр с карточками</summary>
        /// <param name="first">Индекс первой необходимой игры</param>
        /// <param name="last">Индекс последней необходимой игры</param>
        /// <returns>Возвращает лист игр с карточками из таблицы SMGamesWithCards</returns>
        public List<SteamGame> GetGamesWithCardsInRange(int first, int last) {
            if (GamesCount() <= 0)
                throw new ObjectDisposedException("База данных пуста!");
            if (first > last)
                throw new ArgumentException("Правая граница диапазона должна быть >= левой!");
            if (first > GamesCount() - 1)
                throw new ArgumentException("Значение за пределами допустимого диапазона", "first");
            if (last < 0)
                throw new ArgumentException("Значение за пределами допустимого диапазона", "last");

            List<SteamGame> result = new List<SteamGame>(last - first + 1);
            for (int i = first; i <= last; i++)
                if (!_games.ElementAt(i).HasCards) {
                    WeedOutGame(_games.ElementAt(i));
                    result.Add(_games.ElementAt(i));
                }

            return result;        
        }

        /// <summary>Возвращает количество игр.</summary>        
        public int GamesCount() {
            return context.SteamGames.Count();
        }

        /// <summary>Получить карточки для игры и рассчитать ее шанс на окупаемость</summary>
        /// <param name="game">Обрабатываемая игра</param>
        private void WeedOutGame(SteamGame game) {
            
            List<double> cardPrices = GetCardPrices(game, out int cardsCount);

            if (cardPrices.Count == 0) {                // если карт нет - значит что-то пошло не так. Мы не должны об этом умалчивать
                GameHasBeenWeededOut?.Invoke($"Карты для игры {game.Title} не были найдены");
                return;
            }

            double sumOfAdditionalCards = (cardsCount != cardPrices.Count) ? (cardsCount - cardPrices.Count) * cardPrices.Last() : 0;
            double cardsAvgPrice = cardPrices.Sum() + sumOfAdditionalCards;     

            double chanceToPayOff = Math.Round(GetChanceToPayOff(game.Price, cardPrices, cardsCount), 3);

            game.CardsCount = cardsCount;
            game.CardsAveragePrice = cardsAvgPrice;
            game.ChanceToPayOff = chanceToPayOff;
            game.HasCards = true;
                        
            context.SaveChanges();
            GameHasBeenWeededOut?.Invoke($"Игра {game.Title} обработана");
        }

        /// <summary>Возвращает шанс окупаемости игры</summary>
        /// <param name="gamePrice">Цена игры</param>
        /// <param name="cardPrices">Цены карточек</param>
        /// <param name="cardsCount">Количество карточек</param>
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

        /// <summary>Перебор в лоб с некоторыми улучшениями</summary>
        /// <param name="gamePrice">Цена игры</param>
        /// <param name="cardPrices">Цены карточек</param>
        /// <param name="usesCount">Массив количества использованных выпадений каждой карточки</param>
        /// <param name="dropCardsCount">Число выпавших карточек</param>
        /// <param name="positiveCases">Количество положительных исходов</param>
        /// <returns>Алгоритм возвращает true, если дальнейшие вычисления целесообразны, и false в ином случае</returns>
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

        /// <summary>Неполный факториал (N! - K!)</summary>
        /// <param name="K">Нижняя граница</param>
        /// <param name="N">Верхняя граница</param>
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

        /// <summary>Возвращает список с ценами карточек</summary>
        /// <param name="game">Проверяемая игра</param>
        /// <param name="cardsCount">Количество карточек</param>
        private List<double> GetCardPrices(SteamGame game, out int cardsCount) {
            //string url = baseUrl.Replace("*gameID*", game.Key.ToString());                                                                                                                   
            string url = baseUrl.Replace("*gameID*", game.Link.Split( new[] { "app/" }, StringSplitOptions.None)[1].Split('/')[0]); // Пока не разберемся с полем Key
            List<double> cards = new List<double>();

            using (var client = new WebClient()) {
                try
                {
                    var html = client.DownloadString(url);
                    var document = new HtmlDocument();
                    document.LoadHtml(html);

                    var cardNodes = document.DocumentNode.SelectNodes(@"//div[@id = 'searchResults']/div[@id = 'searchResultsTable']/div[@id = 'searchResultsRows']/a[@class = 'market_listing_row_link']");

                    if (cardNodes != null)
                    {
                        foreach (var node in cardNodes)
                        {
                            var price = node.SelectSingleNode(@"div[1]/div[1]/div[2]/span[1]/span[1]").InnerText;
                            price = price.Split(new[] { " " }, StringSplitOptions.None)[0].Remove(0, 1).Replace('.', ',');

                            cards.Add(double.Parse(price) * OneUSDinRUB);
                        }
                    }
                    else
                        throw new NullReferenceException($"Не найдены карточки для игры {game.Title}");

                    cardsCount = int.Parse(document.DocumentNode.SelectSingleNode(@"//div[@id = 'searchResults_ctn']/div[2]/span[@id = 'searchResults_total']").InnerText);
                }
                catch (WebException e)
                {
                    throw new WebException("Нет доступа к ресурсу, проверьте подключение к интернету.");
                }
            }

            return cards;
        }
    }
}
