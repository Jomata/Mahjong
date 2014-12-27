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
    public class HandTest_Empty
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
            List<Tile> AllTiles = Meld.FromString("", ' ').GetAllTiles();
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsAllTerminals(Melds);
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for IsAllTerminalsAndHonors
        ///</summary>
        [TestMethod()]
        public void IsAllTerminalsAndHonorsTest()
        {
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsLittleThreeDragons(Melds);
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for IsNoPointsHand
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("Mahjong.dll")]
        //public void IsNoPointsHandTest()
        //{
        //    List<Meld> Melds = Meld.FromString("", ' ');
        //    TileHonors PlayerWind = new TileHonors(); // TODO: Initialize to an appropriate value
        //    TileHonors GameWind = new TileHonors(); // TODO: Initialize to an appropriate value
        //    bool expected = false;
        //    bool actual;
        //    actual = ScorerJapanese.IsNoPointsHand(Melds, PlayerWind, GameWind);
        //    Assert.AreEqual(expected, actual);
            
        //}

        /// <summary>
        ///A test for IsPairOfIdenticalSequences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mahjong.dll")]
        public void IsPairOfIdenticalSequencesTest()
        {
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
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
            List<Meld> Melds = Meld.FromString("", ' ');
            bool expected = false;
            bool actual;
            actual = ScorerJapanese.IsTwoPairsOfIdenticalSequences(Melds);
            Assert.AreEqual(expected, actual);
            
        }
    }
}
