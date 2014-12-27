using Mahjong;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for HandTest and is intended
    ///to contain all HandTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HandTest_GetShanten
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetShantenNumber
        ///</summary>
        [TestMethod()]
        public void GetShantenTest_1()
        {
            //3 completed melds, 1 completed pair. We keep 1 tile and need 2 more. Shanten 2 - 1
            List<Tile> ClosedTiles = Hand.FromString("O2O3O4 O2 O3O3 B5B5B5 WEWEWE EA NO"); 
            List<Meld> ExposedMelds = Meld.FromString("",' '); 
            int expected = 2 - 1;
            int actual;
            actual = Hand.GetShantenNumber(ClosedTiles, ExposedMelds);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetShantenTest_2()
        {
            //We don't need any- we're on tenpai for seven pairs, if we remove one of the EAs, we wait on NO
            List<Tile> ClosedTiles = Hand.FromString("O1O1 O5O5 O9O9 EAEAEA SOSO WEWE NO");
            List<Meld> ExposedMelds = Meld.FromString("", ' ');
            int expected = 1 - 1;
            int actual;
            actual = Hand.GetShantenNumber(ClosedTiles, ExposedMelds);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetShantenTest_3()
        {
            //Waiting on 2 tiles to complete triplets
            List<Tile> ClosedTiles = Hand.FromString("O1O1O1 O5O5 O9O9 EAEAEA SOSO WEWE");
            List<Meld> ExposedMelds = Meld.FromString("", ' ');
            int expected = 2 - 1;
            int actual;
            actual = Hand.GetShantenNumber(ClosedTiles, ExposedMelds);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetShantenTest_4()
        {
            //We need that Green to be a Red, so we're 1 tile away from win. This is technically tenpai, so it should be shanten 0
            List<Tile> ClosedTiles = Hand.FromString("NONONO EAEAEA SOSOSO WEWE REREGR"); 
            List<Meld> ExposedMelds = Meld.FromString("", ' ');
            int expected = 1 - 1;
            int actual;
            actual = Hand.GetShantenNumber(ClosedTiles, ExposedMelds);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetShantenTest_5()
        {
            //We need to complete the last triplet, either as a triplet or as a sequence, so shanten 1 (2 tiles needed to change, 1 tenpai + 1 shanten)
            List<Tile> ClosedTiles = Hand.FromString("NONONO EAEAEA SOSOSO WEWE O1C1B1");
            List<Meld> ExposedMelds = Meld.FromString("", ' ');
            int expected = 2 - 1;
            int actual;
            actual = Hand.GetShantenNumber(ClosedTiles, ExposedMelds);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetShantenTest_6()
        {
            //Let's try with 4 incomplete sequences
            List<Tile> ClosedTiles = Hand.FromString("RERE O1O2NO B1B2SO C1C2WE O7O8EA");
            List<Meld> ExposedMelds = Meld.FromString("", ' ');
            int expected = 4 - 1;
            int actual;
            actual = Hand.GetShantenNumber(ClosedTiles, ExposedMelds);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetShantenTest_7()
        {
            //We only have the pair. The rest of our hand is a mess. So assuming we keep 4 tiles, we need to complete 4 melds, so we need 4 * 2 = 8 tiles
            //...or my code can be smarter than me and we can go for a 7 pairs, we already have 1 pair, so we need 6 more pairs = 6 more tiles
            List<Tile> ClosedTiles = Hand.FromString("NONO O1O4O7 B1B4B7 C1C4C7 REGRWH");
            List<Meld> ExposedMelds = Meld.FromString("", ' ');
            int expected = 6 - 1;
            int actual;
            actual = Hand.GetShantenNumber(ClosedTiles, ExposedMelds);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetShantenTest_8()
        {
            //Worst hand ever. You have nothing. 7 pairs maybe?
            List<Tile> ClosedTiles = Hand.FromString("SONO O1O4O7 B1B4B7 C1C4C7 REGRWH");
            List<Meld> ExposedMelds = Meld.FromString("", ' ');
            int expected = 7 - 1;
            int actual;
            actual = Hand.GetShantenNumber(ClosedTiles, ExposedMelds);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetShantenTest_9()
        {
            //We only need to finish a triplet or sequence
            List<Tile> ClosedTiles = Hand.FromString("C1C2C3 C4C5C6 C7C8C9 NONO GRREWH");
            List<Meld> ExposedMelds = Meld.FromString("", ' ');
            int expected = 2 - 1;
            int actual;
            actual = Hand.GetShantenNumber(ClosedTiles, ExposedMelds);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetShantenTest_8_AllHands()
        {
            //Worst hand ever. You have nothing. 7 pairs maybe?
            List<Tile> ClosedTiles = Hand.FromString("SONO O1O4O7 B1B4B7 C1C4C7 REGRWH");
            List<Meld> ExposedMelds = Meld.FromString("", ' ');
            int expected = 0;            
            var actual = Hand.GetAllHands(ClosedTiles, ExposedMelds,null,null);
            Assert.AreEqual(expected, actual.Count);
        }

        [TestMethod()]
        public void GetShantenTest_Consistency()
        {
            //Worst hand ever. You have nothing. 7 pairs maybe?
            //List<Tile> ClosedTiles = Hand.FromString("SONO O1O4O7 B1B4B7 C1C4C7 REGRWH");
            List<Tile> ClosedTiles = Hand.GetRandomHand();
            List<Meld> ExposedMelds = Meld.FromString("", ' ');

            int shantenNumber = Hand.GetShantenNumber(ClosedTiles, ExposedMelds);
            var shantenInfo = Hand.GetShantenTiles(ClosedTiles, ExposedMelds);

            Assert.AreEqual(shantenNumber, shantenInfo.Key);
        }

        [TestMethod()]
        public void GetShantenTilesTest_1()
        {
            var rng = new Random();

            for (int i = 0; i < 100; i++)
            {
                List<Tile> ClosedTiles = Hand.GetRandomHand();
                ClosedTiles.Sort();
                List<Meld> ExposedMelds = Meld.FromString("", ' ');

                var shantenInfo = Hand.GetShantenTiles(ClosedTiles, ExposedMelds);
                var baseShantenNumber = shantenInfo.Key;
                if (shantenInfo.Key > 0)
                {
                    foreach (var replaceableTile in shantenInfo.Value)
                    {
                        var newTile = Tile.Instantiate(rng);
                        var newHand = ClosedTiles.FindAll(t => t != replaceableTile).Clone();
                        newHand.Add(newTile);

                        var newShantenNumber = Hand.GetShantenNumber(newHand, ExposedMelds);
                        Assert.IsTrue(newShantenNumber <= baseShantenNumber, "Shanten number should not increase if we remove one of the shanten tiles");
                    }
                }
            }
        }

        [TestMethod()]
        public void GetShantenTilesTest_2()
        {
            //The following should be changeable:      ??              ??          ??              ??  ??  ??
            List<Tile> ClosedTiles = Hand.FromString("[O3][O7][O8][O9][B3][B5][B6][B8][C1][C2][C3][C9][Ea][Gr]");
            List<Meld> ExposedMelds = Meld.FromString("", ' ');

            //Stupid game smarter than me. If we keep B3 and B8, and remove 3 of O3,C9,EA,GR, we can get:
            //[O7][O8][O9]  [B3]XX[B5]   [B6]XX[B8]   [C1][C2][C3] XX{O3/C9/EA/GR}
            //that way we only need to get 3 tiles, and thus are in Shanten 2

            //var shantenInfo = Hand.GetShantenTiles(ClosedTiles, ExposedMelds);
            //int TilesNeededToWin = 4;
            //int ExpectedShanten = TilesNeededToWin-1;

            //Assert.AreEqual(ExpectedShanten,shantenInfo.Key);
            //Assert.AreEqual(6, shantenInfo.Value.Count, "There should be 6 replaceable tiles");
            //Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[O3]"));
            //Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[B3]"));
            //Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[B8]"));
            //Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[C9]"));
            //Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[EA]"));
            //Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[GR]"));
        }

        [TestMethod()]
        public void GetShantenTilesTest_3()
        {
            //The following should be changeable:              ??                          ??  ??  ??  ??  ??
            List<Tile> ClosedTiles = Hand.FromString("[O1][O3][O7][B1][B2][B4][B5][B7][B8][C2][C5][So][We][Gr]");
            List<Meld> ExposedMelds = Meld.FromString("", ' ');
            int TilesNeededToWin = 5;
            int ExpectedShanten = TilesNeededToWin - 1;

            var shantenInfo = Hand.GetShantenTiles(ClosedTiles, ExposedMelds);
            Assert.AreEqual(ExpectedShanten, shantenInfo.Key);
            Assert.AreEqual(6, shantenInfo.Value.Count, "There should be 6 replaceable tiles");
            Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[O7]"));
            Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[C2]"));
            Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[C5]"));
            Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[So]"));
            Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[We]"));
            Assert.IsTrue(shantenInfo.Value.Any(tile => tile.ToString() == "[Gr]"));
        }
    }
}
