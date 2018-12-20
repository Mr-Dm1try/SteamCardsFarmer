using System.ComponentModel.DataAnnotations;

namespace SteamCardsFarmer.Model.Types
{
    /// <summary>
    ///   <para>Этот класс хранит данные по играм из Steam, необходимый для дальнейшей работы с программой: ID игры, ее название, цену, ссылку на игру, 
    ///   URL изображения, присутствие карточек в игре, и, если они есть, их число, среднюю цену и шанс игры окупиться.</para>
    /// </summary>
    public class SteamGame {
        /// <summary>ID игры</summary>
        public int Id { get; set; }
        /// <summary>Название игры</summary>
        public string Title { get; set; }
        /// <summary>Цена игры</summary>
        public double Price { get; set; }
        /// <summary>Ссылка на игру</summary>
        public string Link { get; set; }
        /// <summary>URL изображения</summary>
        public string ImageUrl { get; set; }
        /// <summary>Количество карточек</summary>
        public int CardsCount { get; set; }
        /// <summary>Средняя цена карточек</summary>
        public double CardsAveragePrice { get; set; }
        /// <summary>Шанс окупиться</summary>
        public double ChanceToPayOff { get; set; }
        /// <summary>Имеет ли игра карточки?</summary>
        public bool HasCards { get; set; }
    }
}
