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
    public class HandTest_GetHansAndHands
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
        ///A test for GetHan
        ///</summary>
        [TestMethod()]
        public void GetHanTest_A()
        {
            List<Meld> Concealed = Meld.FromString("O1O1 B1B2B3",' ');
            List<Meld> Exposed = Meld.FromString("B6B7B8 RERERE B5B5B5", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("O1"), Uradoras = Hand.FromString("") , GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East };
            
            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Dora" && x.Value == 2));
            Assert.AreEqual(2, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_B()
        {
            List<Meld> Concealed = Meld.FromString("C3C4C5 C4C4 C7C8C9 O7O8O9", ' ');
            List<Meld> Exposed = Meld.FromString("B7B8B9", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("O1"), Uradoras = Hand.FromString(""), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East };

            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Sanshoku Doujun" && x.Value == 1));
            Assert.AreEqual(1, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_C()
        {
            List<Meld> Concealed = Meld.FromString("C5C6C7 O5O6O7 B4B5B6 B7B7 RERERE", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = true; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("O3"), Uradoras = Hand.FromString("O5"), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East };

            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Riichi" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Uradora" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 1));
            Assert.AreEqual(3, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_D()
        {
            List<Meld> Concealed = Meld.FromString("C1C1 C2C3C4 O2O3O4 O3O4O5 O6O7O8", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = true; // TODO: Initialize to an appropriate value
            bool Tsumo = true; // TODO: Initialize to an appropriate value
            bool Ippatsu = true; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("B8"), Uradoras = Hand.FromString("C3"), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East };

            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Riichi" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Ippatsu" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Tsumo" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Uradora" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Pinfu" && x.Value == 1));
            Assert.AreEqual(5, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_E()
        {
            List<Meld> Concealed = Meld.FromString("C5C5 O3O4O5 O6O6O6 B5B6B7 WHWHWH", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = true; // TODO: Initialize to an appropriate value
            bool Tsumo = true; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("NO"), Uradoras = Hand.FromString("RE"), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East };

            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Riichi" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Tsumo" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 1));
            Assert.AreEqual(3, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_F()
        {
            List<Meld> Concealed = Meld.FromString("C6C6 C7C7C7 O2O3O4 O5O6O7 B5B6B7", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = true; // TODO: Initialize to an appropriate value
            bool Tsumo = true; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("C4"), Uradoras = Hand.FromString("O5"), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East };

            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Riichi" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Tsumo" && x.Value == 1));
            ////// MISSING HAITEI
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Tanyao" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Uradora" && x.Value == 1));
            Assert.AreEqual(4, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_G()
        {
            List<Meld> Concealed = Meld.FromString("C2C3C4 C5C6C7 O3O4O5 O7O8O9 B1B1", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = true; // TODO: Initialize to an appropriate value
            bool Tsumo = true; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("O7"), Uradoras = Hand.FromString("B3"), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East, WinningTile = Concealed.GetAllTiles().Find(x => x.ToString() == "[O4]") };

            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Riichi" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Tsumo" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Dora" && x.Value == 1));
            //TODO: Validate that the wait on pinfu is closed
            Assert.AreEqual(3, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_H()
        {
            List<Meld> Concealed = Meld.FromString("C3C4C5 O3O4O5 O7O7O7 B3B3 B6B7B8", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = true; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("O9"), Uradoras = Hand.FromString(""), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East };

            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Tsumo" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Tanyao" && x.Value == 1));
            Assert.AreEqual(2, actual.Count);
        }

        /// <summary>
        ///A test for GetHan
        ///</summary>
        [TestMethod()]
        public void GetHanTest_I()
        {
            List<Meld> Concealed = Meld.FromString("O4O4O4 B3B4B5 B7B7", ' ');
            List<Meld> Exposed = Meld.FromString("WEWEWE O1O1O1", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("C9"), Uradoras = Hand.FromString(""), GameWind = TileHonors.West };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East };

            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 1));
            Assert.AreEqual(1, actual.Count);
        }

        /// <summary>
        ///A test for GetHan
        ///</summary>
        [TestMethod()]
        public void GetHanTest_J()
        {
            List<Meld> Concealed = Meld.FromString("O4O4O4 B3B4B5 B7B7", ' ');
            List<Meld> Exposed = Meld.FromString("WEWEWE O1O1O1", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("C9"), Uradoras = Hand.FromString(""), GameWind = TileHonors.North };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.West };

            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 1));
            Assert.AreEqual(1, actual.Count);
        }

        /// <summary>
        ///A test for GetHan
        ///</summary>
        [TestMethod()]
        public void GetHanTest_K()
        {
            List<Meld> Concealed = Meld.FromString("O4O4O4 B3B4B5 B7B7", ' ');
            List<Meld> Exposed = Meld.FromString("WEWEWE O1O1O1", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("C9"), Uradoras = Hand.FromString(""), GameWind = TileHonors.West };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.West };

            List<Han> actual;
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 2));
            Assert.AreEqual(1, actual.Count);
        }

        /// <summary>
        ///A test for GetAllHands
        ///</summary>
        [TestMethod()]
        public void GetAllHandsTest()
        {
            List<Tile> ClosedTiles = Meld.FromString("O4O4O4 B3B4B5 B7B7",' ').GetAllTiles();
            List<Meld> ExposedMelds = Meld.FromString("WEWEWE O1O1O1", ' ');
            //bool Riichi = false;
            IGame Game = null;
            IPlayer Player = null;
            //List<List<Meld>> expected = null;
            List<List<Meld>> actual;
            actual = Hand.GetAllHands(ClosedTiles, ExposedMelds, Game, Player);
            //Assert.AreEqual(expected, actual);
            Assert.IsTrue(1 == actual.Count(x => x.Count == 3));
        }

        /// <summary>
        ///A test for GetAllHands
        ///</summary>
        [TestMethod()]
        public void GetAllHandsTest2()
        {
            List<Tile> ClosedTiles = Meld.FromString("O1O2O3 O1O2O3 O1O2O3 RERE", ' ').GetAllTiles();
            List<Meld> ExposedMelds = Meld.FromString("WEWEWE", ' ');
            //bool Riichi = false;
            IGame Game = null;
            IPlayer Player = null;
            //List<List<Meld>> expected = null;
            List<List<Meld>> actual;
            actual = Hand.GetAllHands(ClosedTiles, ExposedMelds, Game, Player);
            //Assert.AreEqual(expected, actual);
            Assert.IsTrue(2 == actual.Count(x => x.Count == 4));
        }

        [TestMethod()]
        public void GetBestHandTest2()
        {
            List<Tile> ClosedTiles = Meld.FromString("O1O2O3 O1O2O3 O1O2O3 RERE", ' ').GetAllTiles();
            List<Meld> ExposedMelds = Meld.FromString("WEWEWE", ' ');
            //bool Riichi = false;
            IGame Game = new IGameStub();
            IPlayer Player = new IPlayerStub { WinningTile = ClosedTiles.Find(x => x.ToString() == "[O1]") };
            //List<List<Meld>> expected = null;
            var actual = Hand.GetBestHand(typeof(Mahjong.ScorerJapanese),ClosedTiles, ExposedMelds, Game, Player);
            //3 concealed + 1 exposed triplet > 3 concealed runs + 1 exposed run
            //If the whole hand was concealed, we could get another han for the Pair of Identical Sequences
            Assert.AreEqual(3, actual.Count(x => x.Type == MeldType.Triplet));             
        }
    }
}
