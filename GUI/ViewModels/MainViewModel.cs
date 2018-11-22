using GUI.ViewModels.AuxiliaryClasses;
using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;
using GUI.Models.Types;

namespace GUI.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
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
        public List<SSGame> Games
        {
            get
            {
                if (marketAPI != null) return marketAPI.Games;
                else return null;
            }
            set
            {
                marketAPI.Games = value;
                OnPropertyChanged("Games");
            }
        }
        public string ChanceString
        {
            get => "Шанс: ";

        }
        public MainViewModel()
        {
            gameMaxPrice = ""; mxGamePrice = 0;
            shopAPI = new SteamShopAPI();
            FetchGamesCommand = new DelegateCommand(FetchGames, CanFetchGames);
            CalculatePayoffChanceCommand = new DelegateCommand(CalculatePayoffChance, CanCalculatePayoffChance);
        }
        private void FetchGames(object obj)
        {
            mxGamePrice = Convert.ToDouble(gameMaxPrice);
            shopAPI.ReloadGamesDB(mxGamePrice);
            marketAPI = new SteamMarketAPI(shopAPI.GetGames());
            Comparison<SSGame> gamesComparison = (firstGame, secondGame) => string.Compare(firstGame.Title, secondGame.Title);
            marketAPI.Games.Sort(gamesComparison);
            Games = marketAPI.Games;
        }
        private void CalculatePayoffChance(object obj)
        {
            marketAPI.WeedOutGames();
            Games = marketAPI.Games;
        }
        private bool CanFetchGames(object arg) => shopAPI != null && mxGamePrice >= 0 ? true : false;
        private bool CanCalculatePayoffChance(object arg) => marketAPI != null && marketAPI.Games.Count > 0 ? true : false;
        public ICommand FetchGamesCommand
        {
            get;
            private set;
        }
        public ICommand CalculatePayoffChanceCommand
        {
            get;
            private set;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
