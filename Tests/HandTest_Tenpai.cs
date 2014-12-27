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
    public class HandTest_Tenpai
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
        ///A test for GetDiscardsForTenpai
        ///</summary>
        [TestMethod()]
        public void GetDiscardsForTenpaiTest_1()
        {
            Type ScorerType = typeof(Mahjong.ScorerJapanese);
            List<Tile> ClosedTiles = Hand.FromString("[O2][O3][O4]   [O7][O7]  [B1][B2][B3]   [B6][B8]   [B8]   [C5][C6][C7]");
            List<Meld> ExposedMelds = new List<Meld>(); // TODO: Initialize to an appropriate value
            IGame Game = new IGameStub();
            IPlayer Player = new IPlayerStub();
            //List<Tile> expected = null;
            List<Tile> actual;
            actual = Hand.GetDiscardsForTenpai(ScorerType, ClosedTiles, ExposedMelds, Game, Player);
            //Assert.AreEqual(expected, actual);
            Assert.IsTrue(actual.Count(t => t.ToString() == "[B6]") == 1, "We should be able to discard B6 and wait on O7 or B8");
            Assert.IsTrue(actual.Count(t => t.ToString() == "[B8]") == 1, "We should be able to discard B8 and wait on B7");
        }

        [TestMethod()]
        public void GetDiscardsForTenpaiTest_2()
        {
            bool testedAtLeastOnce = false;
            do
            {
                Type ScorerType = typeof(Mahjong.ScorerJapanese);
                //List<Tile> ClosedTiles = Hand.FromString("[O2][O3][O4]   [O7][O7]  [B1][B2][B3]   [B6][B8]   [B8]   [C5][C6][C7]");
                List<Tile> ClosedTiles = Hand.GetRandomHand();
                ClosedTiles.Sort();
                List<Meld> ExposedMelds = new List<Meld>(); // TODO: Initialize to an appropriate value
                IGame Game = new IGameStub();
                IPlayer Player = new IPlayerStub();
                //List<Tile> expected = null;
                //List<Tile> actual;

                var ShantenInfo = Hand.GetShantenTiles(ClosedTiles, ExposedMelds);

                if (ShantenInfo.Key == 0)
                {
                    var TenpaiDiscards = Hand.GetDiscardsForTenpai(ScorerType, ClosedTiles, ExposedMelds, Game, Player);
                    Assert.AreEqual(ShantenInfo.Value.Count, TenpaiDiscards.Count);
                    
                    CollectionAssert.AreEquivalent(ShantenInfo.Value, TenpaiDiscards); //AreEquivalent doesnt care about order

                    ShantenInfo.Value.Sort();
                    TenpaiDiscards.Sort();
                    CollectionAssert.AreEqual(ShantenInfo.Value, TenpaiDiscards);

                    testedAtLeastOnce = true;
                }
            }
            while (!testedAtLeastOnce);
        }


        [TestMethod()]
        public void GetDiscardsForTenpaiTest_4()
        {
            //removing a B2 should allow us to wait for an O3
            Type ScorerType = typeof(Mahjong.ScorerJapanese);
            List<Tile> ClosedTiles = Hand.FromString("[O1][O1] [O2][O4] [O8][O8][O8] [B1][B2][B2][B3]  [B4][B4][B4]");
            List<Meld> ExposedMelds = new List<Meld>(); // TODO: Initialize to an appropriate value
            IGame Game = new IGameStub();
            IPlayer Player = new IPlayerStub();
            //List<Tile> expected = null;
            List<Tile> actual;
            actual = Hand.GetDiscardsForTenpai(ScorerType, ClosedTiles, ExposedMelds, Game, Player);
            //Assert.AreEqual(expected, actual);
            Assert.IsTrue(actual.Count(t => t.ToString() == "[B2]") == 1, "Removing a B2 should allow us to wait for an O3");
        }

        //[O6][O7][O8][O9][B1][B3][B4][B5][B6][C6][C7][C8][We][We]
        [TestMethod()]
        public void GetDiscardsForTenpaiTest_5()
        {
            //discard O6 or O9, wait for a B2
            Type ScorerType = typeof(Mahjong.ScorerJapanese);
            //There's an issue when you have 13456, and you can use 456, and then 1_3 and wait for the 2
            List<Tile> ClosedTiles = Hand.FromString("[O6][O7][O8][O9] [B1][B3] [B4][B5][B6] [C6][C7][C8]  [We][We]");
            List<Meld> ExposedMelds = new List<Meld>(); // TODO: Initialize to an appropriate value
            IGame Game = new IGameStub();
            IPlayer Player = new IPlayerStub();
            //List<Tile> expected = null;
            List<Tile> actual;
            actual = Hand.GetDiscardsForTenpai(ScorerType, ClosedTiles, ExposedMelds, Game, Player);
            //Assert.AreEqual(expected, actual);
            Assert.IsTrue(actual.Count(t => t.ToString() == "[O6]") == 1);
            Assert.IsTrue(actual.Count(t => t.ToString() == "[O9]") == 1);
        }

        [TestMethod()]
        public void GetDiscardsForTenpaiTest_3()
        {
            int nTotalTestTimes = 10;
            int nCurrentTests = 0;
            do
            {
                Type ScorerType = typeof(Mahjong.ScorerJapanese);
                //List<Tile> ClosedTiles = Hand.FromString("[O2][O3][O4]   [O7][O7]  [B1][B2][B3]   [B6][B8]   [B8]   [C5][C6][C7]");
                List<Tile> ClosedTiles = Hand.GetRandomHand();
                ClosedTiles.Sort();
                List<Meld> ExposedMelds = new List<Meld>(); // TODO: Initialize to an appropriate value
                IGame Game = new IGameStub();
                IPlayer Player = new IPlayerStub();
                //List<Tile> expected = null;
                //List<Tile> actual;

                var ShantenInfo = Hand.GetShantenTiles(ClosedTiles, ExposedMelds);

                if (ShantenInfo.Key == 0)
                {
                    var TenpaiDiscards = Hand.GetDiscardsForTenpai(ScorerType, ClosedTiles, ExposedMelds, Game, Player);
                    Assert.AreEqual(ShantenInfo.Value.Count, TenpaiDiscards.Count);

                    CollectionAssert.AreEquivalent(ShantenInfo.Value, TenpaiDiscards); //AreEquivalent doesnt care about order

                    ShantenInfo.Value.Sort();
                    TenpaiDiscards.Sort();
                    CollectionAssert.AreEqual(ShantenInfo.Value, TenpaiDiscards);

                    nCurrentTests++;
                }
            }
            while (nCurrentTests < nTotalTestTimes);
        }

        [TestMethod()]
        public void GetDiscardsForTenpaiTest_6()
        {
            Type ScorerType = typeof(Mahjong.ScorerJapanese);
            List<Tile> ClosedTiles = Hand.FromString("[O4][O5][O6] [O9][O9] [B3] [B6][B7][B8][B9] [C1] [C7][C8][C9]");
            //List<Tile> ClosedTiles = Hand.GetRandomHand();
            ClosedTiles.Sort();
            List<Meld> ExposedMelds = new List<Meld>(); // TODO: Initialize to an appropriate value
            IGame Game = new IGameStub();
            IPlayer Player = new IPlayerStub();
            //List<Tile> expected = null;
            //List<Tile> actual;

            var ShantenInfo = Hand.GetShantenTiles(ClosedTiles, ExposedMelds);

            if (ShantenInfo.Key == 0)
            {
                var TenpaiDiscards = Hand.GetDiscardsForTenpai(ScorerType, ClosedTiles, ExposedMelds, Game, Player);
                Assert.AreEqual(ShantenInfo.Value.Count, TenpaiDiscards.Count);

                CollectionAssert.AreEquivalent(ShantenInfo.Value, TenpaiDiscards); //AreEquivalent doesnt care about order

                ShantenInfo.Value.Sort();
                TenpaiDiscards.Sort();
                CollectionAssert.AreEqual(ShantenInfo.Value, TenpaiDiscards);
            }
        }
    }
}
