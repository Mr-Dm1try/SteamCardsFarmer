using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class SCFForm : Form
    {
        private SteamAPI.SteamShopAPI ShopAPI;
        private SteamAPI.SteamMarketAPI MarketAPI;
        public SCFForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShopAPI = new SteamAPI.SteamShopAPI();
            MarketAPI = new SteamAPI.SteamMarketAPI(ShopAPI.GetGames());
        }

        private void FetchGamesButton_Click(object sender, EventArgs e)
        {
            ShopAPI.ReloadGamesDB((double)PriceDial.Value);
            foreach (SteamAPI.Data.Types.SSGame game in MarketAPI.Games)
            {
                gamesComboBox.Items.Add(game.Title + " - " + game.Price + "руб.");
            }
            MarketAPI.Games.Clear();
        }
    }
}
