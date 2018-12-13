using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using SteamCardsFarmer.Model.API;
using SteamCardsFarmer.Model.Types;
using SteamCardsFarmer.ViewModel.AuxiliaryClasses;

namespace SteamCardsFarmer.ViewModel {
    class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private SteamMarketAPI marketAPI;
        private SteamShopAPI shopAPI;
        private string gameMaxPrice;
        private double mxGamePrice;
        public string GameMaxPrice
        {
            get => gameMaxPrice;
            set
            {
                gameMaxPrice = value;
                OnPropertyChanged("GameMaxPrice");
            }
        }
        public List<SMGameAndCards> Games
        {
            get
            {
                if (marketAPI != null) return marketAPI.GetGamesWithCards();
                else return null;
            }
        }
        public string ChanceString
        {
            get
            {
                if (marketAPI == null) return null;
                else return "Шанс: ";
            }
        }
        public MainViewModel()
        {
            gameMaxPrice = ""; mxGamePrice = 0;
            shopAPI = new SteamShopAPI();
            FetchGamesCommand = new DelegateCommand(FetchGames, CanFetchGames);
        }
        #region Commands
        private void FetchGames(object obj)
        {
            mxGamePrice = Convert.ToDouble(gameMaxPrice);
            shopAPI.ReloadGamesDB(mxGamePrice);
            marketAPI = new SteamMarketAPI(shopAPI.GetGames());
            Comparison<SSGame> gamesComparison = (firstGame, secondGame) => string.Compare(firstGame.Title, secondGame.Title);
            marketAPI.Games.Sort(gamesComparison);
            marketAPI.WeedOutGames();
            OnPropertyChanged("Games");
        }
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
        static readonly string[] ValidatedProperties =
        {
            "GameMaxPrice"
        };
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
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
