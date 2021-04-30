using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Net;

using SteamCardsFarmer.Model.API;
using SteamCardsFarmer.Model.Types;
using SteamCardsFarmer.ViewModel.AuxiliaryClasses;

namespace SteamCardsFarmer.ViewModel {
    /// <summary>Класс, отвечающий за логику MVVM</summary>
    class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private SteamMarketAPI marketAPI;
        private SteamShopAPI shopAPI;
        private string gameMaxPrice;
        private double mxGamePrice;
        private int gameMaxIndex;
        private List<SteamGame> gamesList;

        /// <summary>Свойство работает с максимальной ценой, устанавливаемой пользователем</summary>
        public string GameMaxPrice
        {
            get => gameMaxPrice;
            set
            {
                gameMaxPrice = value;
                OnPropertyChanged("GameMaxPrice");
            }
        }

        /// <summary>Свойство возвращает список с играми</summary>
        public List<SteamGame> Games
        {
            get => gamesList;
            private set
            {
                gamesList = value;
                OnPropertyChanged("Games");
            }
        }

        /// <summary>Конструктор класса, инициализирующий элементы</summary>
        public MainViewModel()
        {
            gameMaxPrice = ""; mxGamePrice = 0; gameMaxIndex = 0;
            shopAPI = new SteamShopAPI();
            FetchGamesCommand = new DelegateCommand(FetchGames, CanFetchGames);
            NextPageCommand = new DelegateCommand(IncreaseIndex, CanIncreaseIndex);
            PrevPageCommand = new DelegateCommand(DecreaseIndex, CanDecreaseIndex);
            GetCardsCommand = new DelegateCommand(GetCards, CanGetCards);
        }

        #region Commands
        
        /// <summary>Метод для извлечения игр из магазина</summary>
        /// <param name="obj">Объект, который вызывает процедуру</param>
        private void FetchGames(object obj)
        {
            try
            {
                mxGamePrice = Convert.ToDouble(gameMaxPrice);
                if (shopAPI.MaxPriceInDB() < mxGamePrice || mxGamePrice <= 0) 
                    shopAPI.ReloadGamesDB(mxGamePrice);
                gameMaxIndex = shopAPI.GamesCount() > 9 ? 9 : shopAPI.GamesCount() - 1;
                marketAPI = new SteamMarketAPI();
                Games = shopAPI.GetGamesInRange(gameMaxIndex - (gameMaxIndex >= 9 ? 9 : gameMaxIndex), gameMaxIndex);
            }
            catch (WebException e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Метод для увеличения максимального индекса отображаемой части списка игр</summary>
        /// <param name="obj">Объект, который вызывает процедуру</param>
        private void IncreaseIndex(object arg)
        {
            gameMaxIndex += gameMaxIndex + 10 <= shopAPI.GamesCount() ? 10 : shopAPI.GamesCount() - gameMaxIndex;
            Games = shopAPI.GetGamesInRange(gameMaxIndex - (gameMaxIndex >= 9 ? 9 : gameMaxIndex), gameMaxIndex);
        }

        /// <summary>Метод для уменьшения максимального индекса отображаемой части списка игр</summary>
        /// <param name="obj">Объект, который вызывает процедуру</param>
        private void DecreaseIndex(object arg)
        {
            gameMaxIndex -= gameMaxIndex - 10 >= 9 ? 10 : 10 - gameMaxIndex;
            Games = shopAPI.GetGamesInRange(gameMaxIndex - (gameMaxIndex >= 9 ? 9 : gameMaxIndex), gameMaxIndex);
        }

        /// <summary>Метод для извлечения карт для игр с Торговой площадки</summary>
        /// <param name="obj">Объект, который вызывает процедуру</param>
        private void GetCards(object arg)
        {
            try
            {
                Games = marketAPI.GetGamesWithCardsInRange(gameMaxIndex - (gameMaxIndex >= 9 ? 9 : gameMaxIndex), gameMaxIndex);
            }
            catch (WebException e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Метод проверяет, можно ли извлечь игры</summary>
        /// <param name="arg">Объект, который вызывает процедуру</param>
        private bool CanFetchGames(object arg) => shopAPI != null && mxGamePrice >= 0 && IsValid ? true : false;

        /// <summary>Метод, проверяющий, можно ли увеличить максимальный индекс отображаемой части списка игр</summary>
        /// <param name="obj">Объект, который вызывает процедуру</param>
        private bool CanIncreaseIndex(object arg) => shopAPI != null && gameMaxIndex < shopAPI.GamesCount() && Games != null;

        /// <summary>Метод, проверяющий, можно ли уменьшить максимальный индекс отображаемой части списка игр</summary>
        /// <param name="obj">Объект, который вызывает процедуру</param>
        public bool CanDecreaseIndex(object arg) => shopAPI != null && gameMaxIndex > 9 && shopAPI.GamesCount() > 0;

        /// <summary>Метод, проверяющий, можно ли извлечь карты для игр с Торговой площадки</summary>
        /// <param name="obj">Объект, который вызывает процедуру</param>
        public bool CanGetCards(object arg) => marketAPI != null;

        public ICommand FetchGamesCommand
        {
            get;
            private set;
        }

        public ICommand NextPageCommand
        {
            get;
            private set;
        }

        public ICommand PrevPageCommand
        {
            get;
            private set;
        }

        public ICommand GetCardsCommand
        {
            get;
            private set;
        }
        
        #endregion

        #region Validation
        string IDataErrorInfo.Error => null;
        string IDataErrorInfo.this[string propertyName] => GetValidationError(propertyName);
   
        /// <summary>Массив с именами свойств, которые проверяются на корректность</summary>
        static readonly string[] ValidatedProperties =
        {
            "GameMaxPrice"
        };
        /// <summary>Метод, проверяющий корректность цены</summary>
        private string ValidatePrice()
        {
            try
            {
                Convert.ToDouble(GameMaxPrice);
                return null;
            }
            catch (FormatException)
            {
                return "Должно быть введено число";
            }
        }
        /// <summary>Возвращает ошибку, полученную при валидации</summary>
        /// <param name="propertyName">Название свойства</param>
        string GetValidationError(string propertyName)
        {
            string error = null;
            switch (propertyName)
            {
                case "GameMaxPrice":
                    {
                        error = ValidatePrice();
                        break;
                    }
            }
            return error;
        }
        
        /// <summary>Свойство, значение которого зависит от корректности всех проверяемых свойств</summary>
        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperties)
                    if (GetValidationError(property) != null) return false;
                return true;
            }
        }
        #endregion

        #region PropertyChanges
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>Метод, который вызывается при изменении свойства и вызывает событие, связанное с этим свойством</summary>
        /// <param name="propertyName">Название свойства</param>
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
