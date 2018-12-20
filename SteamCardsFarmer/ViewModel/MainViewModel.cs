﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

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
            get
            {
                if (marketAPI != null) return /*marketAPI.GetGamesWithCards()*/null;
                else return null;
            }
        }

        /// <summary>Свойство возвращает строку "Шанс: ", если это возможно</summary>
        public string ChanceString
        {
            get
            {
                if (marketAPI == null) return null;
                else return "Шанс: ";
            }
        }

        /// <summary>Конструктор класса, инициализирующий элементы</summary>
        public MainViewModel()
        {
            gameMaxPrice = ""; mxGamePrice = 0;
            shopAPI = new SteamShopAPI();
            FetchGamesCommand = new DelegateCommand(FetchGames, CanFetchGames);
        }

        #region Commands
        //Мехн, добавь адекватное описание функции и смысла параметра
        /// <summary>Функция извлечения игр</summary>
        /// <param name="obj"></param>
        private void FetchGames(object obj)
        {
            mxGamePrice = Convert.ToDouble(gameMaxPrice);
            shopAPI.ReloadGamesDB(mxGamePrice);
            //marketAPI = new SteamMarketAPI(shopAPI.GetGames());
            Comparison<SteamGame> gamesComparison = (firstGame, secondGame) => string.Compare(firstGame.Title, secondGame.Title);
            //marketAPI.Games.Sort(gamesComparison);
            //marketAPI.WeedOutGames();
            OnPropertyChanged("Games");
        }

        //Аналогично
        /// <summary>Функция узнает, можно ли извлечь игры</summary>
        /// <param name="arg"></param>
        private bool CanFetchGames(object arg) => shopAPI != null && mxGamePrice >= 0 && IsValid ? true : false;

        public ICommand FetchGamesCommand
        {
            get;
            private set;
        }
        #endregion

        #region Validation
        string IDataErrorInfo.Error => null;
        string IDataErrorInfo.this[string propertyName] => GetValidationError(propertyName);
        //Щито?(
        /// <summary>???</summary>
        static readonly string[] ValidatedProperties =
        {
            "GameMaxPrice"
        };
        /// <summary>Функция валидации цены</summary>
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
        //Ну ты понял
        /// <summary></summary>
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
        //И тут тоже понял
        /// <summary></summary>
        /// <param name="propertyName">Название свойства</param>
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
