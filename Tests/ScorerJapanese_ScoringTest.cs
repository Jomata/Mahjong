using Mahjong;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for ScorerJapaneseTest and is intended
    ///to contain all ScorerJapaneseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ScorerJapanese_ScoringTest
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
        ///A test for GetPayments
        ///</summary>
        [TestMethod()]
        public void GetPaymentsTest_RonB2()
        {
            List<Meld> Concealed = Meld.FromString("O1O1 B1B2B3", ' ');
            List<Meld> Exposed = Meld.FromString("B6B7B8 RERERE B5B5B5", ' ');

            IPlayerStub Winner = new IPlayerStub();
            Winner.Tsumo = false;
            Winner.IsDealer = false;
            Winner.WinningTile = Concealed.GetAllTiles().Find(x => x.ToString() == "[B2]");

            IPlayerStub RonD = new IPlayerStub();

            IGameStub Game = new IGameStub();
            Game.CurrentPlayer = RonD;
            Game.Players.Add(Winner);
            Game.Players.Add(RonD);
            Game.Doras = Hand.FromString("O1");
            
            IDictionary<IPlayer, int> actual = ScorerJapanese.GetPayments(Concealed, Exposed, Game, Winner);

            Assert.IsTrue(actual.Count(x => x.Value < 0) == 1, "Winning by Ron, only 1 player should pay");
            CollectionAssert.Contains(actual.Keys.ToArray(), RonD, "Player that dealt into run must pay");
            Assert.AreEqual(-3900, actual[RonD], "Player that dealt into must pay 3900");
        }

        [TestMethod()]
        public void GetPaymentsTest_RonC5()
        {
            List<Meld> Concealed = Meld.FromString("C3C4C5 C4C4 C7C8C9 B7B8B9", ' ');
            List<Meld> Exposed = Meld.FromString("O7O8O9", ' ');

            IPlayerStub Winner = new IPlayerStub();
            Winner.Tsumo = false;
            Winner.IsDealer = false;
            Winner.WinningTile = Concealed.GetAllTiles().Find(x => x.ToString() == "[C5]");

            IPlayerStub RonD = new IPlayerStub();

            IGameStub Game = new IGameStub();
            Game.CurrentPlayer = RonD;
            Game.Players.Add(Winner);
            Game.Players.Add(RonD);
            Game.Doras = Hand.FromString("B3");

            IDictionary<IPlayer, int> actual = ScorerJapanese.GetPayments(Concealed, Exposed, Game, Winner);

            Assert.IsTrue(actual.Count(x => x.Value < 0) == 1, "Winning by Ron, only 1 player should pay");
            CollectionAssert.Contains(actual.Keys.ToArray(), RonD, "Player that dealt into run must pay");
            Assert.AreEqual(-1000, actual[RonD], "Player that dealt into must pay 1000");
        }

        ///// <summary>
        /////A test for GetFu
        /////</summary>
        //[TestMethod()]
        //public void GetFuTest()
        //{
        //    List<Meld> Concealed = null; // TODO: Initialize to an appropriate value
        //    List<Meld> Exposed = null; // TODO: Initialize to an appropriate value
        //    bool Riichi = false; // TODO: Initialize to an appropriate value
        //    bool Tsumo = false; // TODO: Initialize to an appropriate value
        //    bool Ippatsu = false; // TODO: Initialize to an appropriate value
        //    IGame game = null; // TODO: Initialize to an appropriate value
        //    IPlayer player = null; // TODO: Initialize to an appropriate value
        //    List<Fu> expected = null; // TODO: Initialize to an appropriate value
        //    List<Fu> actual;
        //    actual = ScorerJapanese.GetFu(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for GetBasicValue
        /////</summary>
        //[TestMethod()]
        //public void GetBasicValueTest()
        //{
        //    List<Meld> Concealed = null; // TODO: Initialize to an appropriate value
        //    List<Meld> Exposed = null; // TODO: Initialize to an appropriate value
        //    IGame Game = null; // TODO: Initialize to an appropriate value
        //    IPlayer Winner = null; // TODO: Initialize to an appropriate value
        //    int expected = 0; // TODO: Initialize to an appropriate value
        //    int actual;
        //    actual = ScorerJapanese.GetBasicValue(Concealed, Exposed, Game, Winner);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}
    }
}
