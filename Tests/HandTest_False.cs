using Mahjong;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tests
{
    /// <summary>
    ///This is a test class for HandTest and is intended
    ///to contain all HandTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HandTest_False
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
        ///A test for Is13Orphans
        ///</summary>
        [TestMethod()]
        public void Is13OrphansTest()
        {
            List<Tile> AllTiles = Hand.FromString("C2O1O9B1B9NOEASOWEWHWHGRREC9");
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.Is13Orphans(AllTiles);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Is13Orphans
        ///</summary>
        [TestMethod()]
        public void Is13OrphansTest2()
        {
            List<Tile> AllTiles = Hand.FromString("C2C9O1O9B1B9EASONOWEWHREGREA");
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.Is13Orphans(AllTiles);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsAllGreen
        ///</summary>
        [TestMethod()]
        public void IsAllGreenTest()
        {
            List<Meld> Melds = Meld.FromString("B1B2B3 B2B3B4 B6B6B6 B8B8B8 GRGR", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsAllGreen(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsAllHonors
        ///</summary>
        [TestMethod()]
        public void IsAllHonorsTest()
        {
            List<Meld> Melds = Meld.FromString("EAEAEA WEWEWE C5C5 WHWHWH GRGRGR", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsAllHonors(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsAllSimples
        ///</summary>
        [TestMethod()]
        public void IsAllSimplesTest()
        {
            List<Meld> Melds = Meld.FromString("C2C3C4 o2o2o2 o6o7o8 b9b9 B6B7B8", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsAllSimples(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsAllTerminals
        ///</summary>
        [TestMethod()]
        public void IsAllTerminalsTest()
        {
            List<Meld> Melds = Meld.FromString("C1C1C1 O1O2O3 O9O9O9 B1B1 B9B9B9", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsAllTerminals(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsAllTerminalsAndHonors
        ///</summary>
        [TestMethod()]
        public void IsAllTerminalsAndHonorsTest_Triplets()
        {
            List<Meld> Melds = Meld.FromString("C1C1C1 O9O9O9 O3O3O3 B9B9B9 GRGR", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsAllTerminalsAndHonors(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsAllTerminalsAndHonors
        ///</summary>
        [TestMethod()]
        public void IsAllTerminalsAndHonorsTest_Pairs()
        {
            List<Meld> Melds = Meld.FromString("C2C2 O1O1 B9B9 EAEA SOSO WHWH RERE", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsAllTerminalsAndHonors(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsAllTriplets
        ///</summary>
        [TestMethod()]
        public void IsAllTripletsTest()
        {
            List<Meld> Melds = Meld.FromString("C7C8C9 O3O3O3 B1B1B1 B7B7B7 NONO", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsAllTriplets(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsBigFourWinds
        ///</summary>
        [TestMethod()]
        public void IsBigFourWindsTest()
        {
            List<Meld> Melds = Meld.FromString("O3O3 EAEAEA GRGRGR WEWEWE NONONO", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsBigFourWinds(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsBigThreeDragons
        ///</summary>
        [TestMethod()]
        public void IsBigThreeDragonsTest()
        {
            List<Meld> Melds = Meld.FromString("C5C6C7 B4B4 WHWHWH NONONO RERERE", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsBigThreeDragons(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsFlush
        ///</summary>
        [TestMethod()]
        public void IsFlushTest()
        {
            List<Meld> Melds = Meld.FromString("O1O2O3 B4B4B4 B5B5 B6B6B6 B7B8B9", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsFlush(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsFullStraight
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mahjong.dll")]
        public void IsFullStraightTest()
        {
            List<Meld> Melds = Meld.FromString("C9C9C9 B2B3B4 B4B5B6 B7B8B9 WHWH", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsFullStraight(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsHalfFlush
        ///</summary>
        [TestMethod()]
        public void IsHalfFlushTest()
        {
            List<Meld> Melds = Meld.FromString("O1O1O1 C3C4C5 O7O8O9 B1B2 WHWHWH", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsHalfFlush(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsLittleFourWinds
        ///</summary>
        [TestMethod()]
        public void IsLittleFourWindsTest()
        {
            List<Meld> Melds = Meld.FromString("B6B7B8 EAEAEA SOSOSO WEWEWE GRGR", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsLittleFourWinds(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsLittleThreeDragons
        ///</summary>
        [TestMethod()]
        public void IsLittleThreeDragonsTest()
        {
            List<Meld> Melds = Meld.FromString("O2O3O4 B6B7B8 WHWHWH GRGRGR SOSO", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsLittleThreeDragons(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsNoPointsHand
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mahjong.dll")]
        public void IsNoPointsHandTest()
        {
            List<Meld> Melds = Meld.FromString("C1C2C3 C5C5 B1B1B1 O7O8O9 B4B5B6", ' ');
            TileHonors PlayerWind = TileHonors.East;
            TileHonors GameWind = TileHonors.South;
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsNoPointsHand(Melds, PlayerWind, GameWind, Melds.GetAllTiles().Find(t => t.ToString() == "[B1]"));
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsPairOfIdenticalSequences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mahjong.dll")]
        public void IsPairOfIdenticalSequencesTest()
        {
            List<Meld> Melds = Meld.FromString("C2C3C4 O6O7O8 O5O6O7 B9B9B9 GRGR", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsPairOfIdenticalSequences(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsSevenPairs
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mahjong.dll")]
        public void IsSevenPairsTest()
        {
            List<Meld> Melds = Meld.FromString("C2C3C4 O6O7O8 O5O6O7 B9B9B9 GRGR", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsSevenPairs(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsSevenPairs
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mahjong.dll")]
        public void IsSevenPairsTest2()
        {
            List<Meld> Melds = Meld.FromString("C7C8C9 O1O1O1 O7O8O9 B9B9B9 WHWH", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsSevenPairs(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsTerminalOrHonorOnAllSets
        ///</summary>
        [TestMethod()]
        public void IsTerminalOrHonorOnAllSetsTest()
        {
            List<Meld> Melds = Meld.FromString("C7C8C9 O1O2O3 O7O8O9 B9B9B9 C5C5", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsTerminalOrHonorOnAllSets(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsTerminalsOnlyOnAllSets
        ///</summary>
        [TestMethod()]
        public void IsTerminalsOnlyOnAllSetsTest()
        {
            List<Meld> Melds = Meld.FromString("C1C2C3 C9C9C9 NONO B1B2B3 B7B8B9", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsTerminalsOnAllSets(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsThreeColorSequences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mahjong.dll")]
        public void IsThreeColorSequencesTest()
        {
            List<Meld> Melds = Meld.FromString("C2C3C4 O1O1O1 B2B3B4 C2C3C4 SOSO", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsThreeColorSequences(Melds);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsTwoPairsOfIdenticalSequences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mahjong.dll")]
        public void IsTwoPairsOfIdenticalSequencesTest()
        {
            List<Meld> Melds = Meld.FromString("C5C5 O7O8O9 O7O8O9 C7C8C9 B7B8B9", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsTwoPairsOfIdenticalSequences(Melds);
            Assert.AreEqual(expected, actual);
        }
    }
}
