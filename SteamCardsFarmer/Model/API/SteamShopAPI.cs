using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using HtmlAgilityPack;

using SteamCardsFarmer.Model.Types;

namespace SteamCardsFarmer.Model.API {
    /// <summary>Этот класс является своеобразным API для магазина Steam.</summary>
    public sealed class SteamShopAPI {
        private readonly SteamGamesContext context;
        private readonly string baseUrl = "https://store.steampowered.com/search/?sort_by=Price_ASC&category1=998&category2=29";
        private readonly string baseImageUrl = "https://steamcdn-a.akamaihd.net/steam/apps/*gameID*/header.jpg";

        /// <summary> Событие обработки новой игры парсером </summary>
        public event Action<String> FoundNewGame;

        /// <summary>Конструктор класса. В нем создается экземпляр класса контекста БД для игр.</summary>
        public SteamShopAPI() => context = new SteamGamesContext();

        /// <summary> Обновление таблицы с играми в БД </summary>
        /// <param name="maxPrice"> Максимальная цена, до которой ищутся игры </param>
        public void ReloadGamesDB(double maxPrice) {
            foreach (var entity in context.SteamGames.ToList())                 //очищаем БД
                context.SteamGames.Remove(entity);            

            foreach (var game in SteamShopParse(maxPrice))
                context.SteamGames.Add(game.Value);

            context.SaveChanges();
        }

        /// <summary> Получение игр </summary>
        /// <returns> Возвращает лист игр из таблицы SSGames </returns>
        [Obsolete("По возможности используйте GetGamesInRange")]
        public List<SteamGame> GetGames() {
            if (GamesCount() > 0)
                return context.SteamGames.ToList();
            else
                throw new ObjectDisposedException("База данных пуста!");
        }

        /// <summary> Получение игр по диапазону </summary>
        /// <param name="first"> Порядковый индекс первой игры (с нуля) </param>
        /// <param name="last"> Поредковый индекс последней игры из диапазона </param>
        /// <returns> Возвращает заданный диапазон игр из БД </returns>
        public List<SteamGame> GetGamesInRange(int first, int last) {
            if (GamesCount() <= 0)
                throw new ObjectDisposedException("База данных пуста!");
            if (first > last)
                throw new ArgumentException("Правая граница диапазона должна быть >= левой!");
            if (first > 0 && first < GamesCount() - 1)
                throw new ArgumentException("Значение за пределами допустимого диапазона", "first");
            if (last > 0 && last < GamesCount() - 1)
                throw new ArgumentException("Значение за пределами допустимого диапазона", "last");

            List<SteamGame> games = context.SteamGames.ToList();
            List<SteamGame> result = new List<SteamGame>(last - first + 1);
            for (int i = first; i <= last; i++)
                result.Add(games.ElementAt(i));

            return result;
        }

        /// <summary> Получить количество игр </summary>
        /// <returns> Возвращает количество игр в БД </returns>
        public int GamesCount() {
            return context.SteamGames.Count();
        }

        /// <summary>Функция, которая парсит магазин игр Steam по полученной в аргументе максимальной цене. Возвращает словарь с играми.</summary>
        /// <param name="maxPrice">Добавьте максимальную цену.</param>
        private Dictionary<string, SteamGame> SteamShopParse(double maxPrice) {
            var url = baseUrl;
            Dictionary<string, SteamGame> games = new Dictionary<string, SteamGame>();

            using (var client = new WebClient()) {
                double currPrice = 0;
                
                try
                {
                    do {
                        var html = client.DownloadString(url);
                        var document = new HtmlDocument();
                        document.LoadHtml(html);

                        var gameNodes = document.DocumentNode.SelectNodes(@"//div[@id = 'search_results']/div[@id = 'search_result_container']/div[2]/a");
                        if (gameNodes == null)  // For what?
                            continue;

                        foreach (var node in gameNodes)
                        {
                            var title = node.SelectSingleNode("div[2]/div[1]/span[@class = 'title']").InnerText;
                            var price = node.SelectSingleNode("div[2]/div[4]/div[2]").InnerText;
                            var href = node.GetAttributeValue("href", "error");

                            if (!price.Contains('.') || href.Contains("/sub/"))     // обход бесплатных игр и паков
                                continue;

                            var id = href.Split(new[] { "app/" }, StringSplitOptions.None)[1].Split('/')[0];



                            var arr = price.Split(new[] { "pСѓР±." }, StringSplitOptions.None);     //разбиение по подстроке "руб."
                            currPrice = double.Parse((arr.Count() > 2) ? arr[1] : arr[0]);
                            if (currPrice > maxPrice)
                                break;

                            var image = baseImageUrl.Replace("*gameID*", id);

                            var game = new SteamGame()
                            {
                                Id = Convert.ToInt32(id),
                                Title = title,
                                Price = currPrice,
                                Link = href,
                                ImageUrl = image,
                                HasCards = false
                            };

                            try
                            {
                                games.Add(game.Title, game);
                                FoundNewGame?.Invoke("Новая обработанная игра: " + game.Title);
                            }
                            catch (ArgumentException)
                            {
                                continue;
                            }
                        }

                        var nextPageButtons = document.DocumentNode.SelectNodes(@"//div[@class = 'search_pagination_right']/a[@class = 'pagebtn']");
                        foreach (var button in nextPageButtons)
                            if (button.InnerText == "&gt;")         //знак >
                                url = button.GetAttributeValue(@"href", "error");
                            else
                                url = "end";
                    } while (currPrice <= maxPrice && url != "end");
                }
                catch (WebException e)
                {
                    throw new WebException("Нет доступа к ресурсу, проверьте подключение к интернету.");
                }      
                
            }           
            
            return games;
        }
    }
}
