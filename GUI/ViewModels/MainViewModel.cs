using GUI.ViewModels.AuxiliaryClasses;
using System.ComponentModel;

namespace GUI.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private SteamMarketAPI marketAPI;
        private SteamShopAPI shopAPI;
        private double gameMaxPrice;
        public double GameMaxPrice
        {
            get { return gameMaxPrice; }
            set
            {
                gameMaxPrice = value;
                OnPropertyChanged("GameMaxPrice");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
