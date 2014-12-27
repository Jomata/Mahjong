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
    public class ScorerJapanese_GetHansTest
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
            List<Tile> ConcealedTiles = Hand.FromString("O1O1B1B2B3");
            List<Meld> Exposed = Meld.FromString("B6B7B8 RERERE B5B5B5", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("O1"), Uradoras = Hand.FromString(""), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East, WinningTile = ConcealedTiles.Find(x => x.ToString()=="[B2]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Dora" && x.Value == 2));
            Assert.AreEqual(2, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_B()
        {
            List<Tile> ConcealedTiles = Hand.FromString("C3C4C5C4C4C7C8C9O7O8O9");
            List<Meld> Exposed = Meld.FromString("B7B8B9", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("O1"), Uradoras = Hand.FromString(""), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East, WinningTile = ConcealedTiles.Find(x => x.ToString() == "[C5]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Sanshoku Doujun" && x.Value == 1));
            Assert.AreEqual(1, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_C()
        {
            List<Tile> ConcealedTiles = Hand.FromString("C5C6C7O5O6O7B4B5B6B7B7RERERE");
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = true; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("O3"), Uradoras = Hand.FromString("O5"), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East, WinningTile = ConcealedTiles.Find(x => x.ToString() == "[O5]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Riichi" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Uradora" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 1));
            Assert.AreEqual(3, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_D()
        {
            List<Tile> ConcealedTiles = Hand.FromString("C1C1C2C3C4O2O3O4O3O4O5O6O7O8");
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = true; // TODO: Initialize to an appropriate value
            bool Tsumo = true; // TODO: Initialize to an appropriate value
            bool Ippatsu = true; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("B8"), Uradoras = Hand.FromString("C3"), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East, WinningTile = ConcealedTiles.Find(x => x.ToString() == "[C2]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
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
            List<Tile> ConcealedTiles = Hand.FromString("C5C5O3O4O5O6O6O6B5B6B7WHWHWH");
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = true; // TODO: Initialize to an appropriate value
            bool Tsumo = true; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("NO"), Uradoras = Hand.FromString("RE"), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East, WinningTile = ConcealedTiles.Find(x => x.ToString() == "[Wh]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Riichi" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Tsumo" && x.Value == 1));
            Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 1));
            Assert.AreEqual(3, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_F()
        {
            List<Tile> ConcealedTiles = Hand.FromString("C6C6C7C7C7O2O3O4O5O6O7B5B6B7");
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = true; // TODO: Initialize to an appropriate value
            bool Tsumo = true; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("C4"), Uradoras = Hand.FromString("O5"), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East, WinningTile = ConcealedTiles.Find(x => x.ToString() == "[O4]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
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
            List<Tile> ConcealedTiles = Hand.FromString("C2C3C4C5C6C7O3O4O5O7O8O9B1B1");
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = true; // TODO: Initialize to an appropriate value
            bool Tsumo = true; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("O7"), Uradoras = Hand.FromString("B3"), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East, WinningTile = ConcealedTiles.Find(x => x.ToString() == "[O4]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
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
            List<Tile> ConcealedTiles = Hand.FromString("C3C4C5O3O4O5O7O7O7B3B3B6B7B8");
            List<Meld> Exposed = Meld.FromString("", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = true; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("O9"), Uradoras = Hand.FromString(""), GameWind = TileHonors.East };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East, WinningTile = ConcealedTiles.Find(x => x.ToString() == "[O7]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
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
            List<Tile> ConcealedTiles = Hand.FromString("O4O4O4B3B4B5B7B7");
            List<Meld> Exposed = Meld.FromString("WEWEWE O1O1O1", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("C9"), Uradoras = Hand.FromString(""), GameWind = TileHonors.West };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.East, WinningTile = ConcealedTiles.Find(x => x.ToString() == "[B7]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
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
            List<Tile> ConcealedTiles = Hand.FromString("O4O4O4B3B4B5B7B7");
            List<Meld> Exposed = Meld.FromString("WEWEWE O1O1O1", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString("C9"), Uradoras = Hand.FromString(""), GameWind = TileHonors.North };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.West, WinningTile = ConcealedTiles.Find(x => x.ToString() == "[O4]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 1));
            Assert.AreEqual(1, actual.Count);
        }

        ///// <summary>
        /////A test for GetHan
        /////</summary>
        //[TestMethod()]
        //public void GetHanTest_K()
        //{
        //    List<Tile> ConcealedTiles = Hand.FromString("O4O4O4B3B4B5B7B7");
        //    List<Meld> Exposed = Meld.FromString("WEWEWE O1O1O1", ' ');
        //    bool Riichi = false; // TODO: Initialize to an appropriate value
        //    bool Tsumo = false; // TODO: Initialize to an appropriate value
        //    bool Ippatsu = false; // TODO: Initialize to an appropriate value

        //    IGame game = new IGameStub { Doras =Hand.FromString("C9"), Uradoras = Hand.FromString(""), GameWind = TileHonors.West };
        //    IPlayer player = new IPlayerStub { PlayerWind = TileHonors.West };

        //    List<Han> actual;
        //    var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
        //    actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

        //    Assert.IsTrue(1 == actual.Count(x => x.Name == "Yakuhai" && x.Value == 2));
        //    Assert.AreEqual(1, actual.Count);
        //}

        [TestMethod()]
        public void GetHanTest_L()
        {
            List<Tile> ConcealedTiles = Hand.FromString("O4O5O6B5B5B5C7C7");
            List<Meld> Exposed = Meld.FromString("O7O8O9 O1O2O3", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString(""), Uradoras = Hand.FromString(""), GameWind = TileHonors.West };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.West, WinningTile = ConcealedTiles.Find(x => x.ToString()=="[O4]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(1 == actual.Count(x => x.Name == "Ikkitsuukan" && x.Value == 1));
            //Assert.AreEqual(1, actual.Count);
        }

        [TestMethod()]
        public void GetHanTest_M()
        {
            List<Tile> ConcealedTiles = Hand.FromString("O6O7O7O8O8O9C8C8RERERE");
            List<Meld> Exposed = Meld.FromString("B5B6B7", ' ');
            bool Riichi = false; // TODO: Initialize to an appropriate value
            bool Tsumo = false; // TODO: Initialize to an appropriate value
            bool Ippatsu = false; // TODO: Initialize to an appropriate value

            IGame game = new IGameStub { Doras =Hand.FromString(""), Uradoras = Hand.FromString(""), GameWind = TileHonors.West };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.West, WinningTile = ConcealedTiles.Find(x => x.ToString() == "[O6]") };

            List<Han> actual;
            var Concealed = Hand.GetBestHand(typeof(ScorerJapanese), ConcealedTiles, Exposed, game, player);
            actual = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);

            Assert.IsTrue(actual.Count> 0);
            //Assert.AreEqual(1, actual.Count);
        }
    }
}
