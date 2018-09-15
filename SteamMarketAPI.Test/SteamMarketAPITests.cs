using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamMarketAPI;
using SteamAPI.Data;
using SteamAPI.Data.Types;

namespace SteamMarketAPI.Test {
    [TestClass]
    public class SteamMarketAPITests {
        public TestContext TestContext { get; set; }

        // Для корректной работы теста необходимо изменить доступность SteamMarketAPI.GetChanseToPayOff на public
        //[DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
        //    "data.xml",
        //    "info",
        //    DataAccessMethod.Sequential)]
        //[TestMethod]
        //public void GetChanceToPayOff_BaseTest() {
        //    SteamMarketAPI newApi = new SteamMarketAPI(new List<SSGame>());

        //    double gamePrice = Convert.ToDouble(TestContext.DataRow["price"]);

        //    string cards = TestContext.DataRow["cards"].ToString();
        //    cards = cards.Remove(cards.Length - 1, 1);
        //    List<double> cardPrices = new List<double>();
        //    foreach (var item in cards.Split(';')) 
        //        cardPrices.Add(double.Parse(item));

        //    double expected = Convert.ToDouble(TestContext.DataRow["expected"]);

        //    var cardsCount = cardPrices.Count;

        //    double actual = newApi.GetChanceToPayOff(gamePrice, cardPrices, cardsCount);

        //    string fileName = "TestResults.txt";
        //    using (FileStream aFile = new FileStream(fileName, FileMode.OpenOrCreate)) {
        //        using (StreamWriter sw = new StreamWriter(aFile)) {
        //            aFile.Seek(0, SeekOrigin.End);

        //            sw.Write("ENTRY DATA: ");
        //            sw.Write("price - " + gamePrice + "\t");
        //            sw.Write("cards - " + cards + "\t");
        //            sw.Write("expected - " + expected + "\t");
        //            sw.Write(" | RESULT: " + actual + "\t");
        //            sw.Write(" | BOOL: " + (actual >= expected));

        //            sw.WriteLine();
        //        }
        //    }

        //    Assert.IsTrue(actual >= expected);
        //}

    }
    
}
