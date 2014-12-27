using Mahjong;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tests
{
    /// <summary>
    ///This is a test class for HandTest and is intended
    ///to contain all HandTest Unit Tests
    ///http://saki.wikia.com/wiki/Mahjong_yaku
    ///</summary>
    [TestClass()]
    public class HandTest_Saki
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

        
        [TestMethod()]
        public void Tanyao_ClosedTest()
        {
            List<Meld> Concealed = Meld.FromString("b3b3b3 o5o6o7 c5c5c5 c6c7c8 b7b7", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 1;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Tanyao_OpenTest()
        {
            List<Meld> Concealed = Meld.FromString("b3b3b3 o5o6o7 c6c7c8 b7b7", ' ');
            List<Meld> Exposed = Meld.FromString("c5c5c5", ' ');
            int expected = 1;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void PinfuTest()
        {
            List<Meld> Concealed = Meld.FromString("b6b7b8 c2c3c4 o7o8o9 c4c5c6 b5b5", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 1;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// The hand receives one han per dragon triplet/kan, regardless of whether the meld is open or closed.
        /// </summary>
        [TestMethod()]
        public void DragonTriplet1Test()
        {
            List<Meld> Concealed = Meld.FromString("o2o2o2 grgrgr o2o3o4 c3c4c5 c7c7", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 1;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The hand receives one han per dragon triplet/kan, regardless of whether the meld is open or closed.
        /// </summary>
        [TestMethod()]
        public void DragonTriplet2Test()
        {
            List<Meld> Concealed = Meld.FromString("grgrgr o2o3o4 c3c4c5 c7c7", ' ');
            List<Meld> Exposed = Meld.FromString("rerere", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A hand that has the player's seat-wind or the prevailing-wind triplet/kan. 
        /// The hand receives one han per qualifying wind triplet/kan, regardless of whether the meld is open or closed. 
        /// The yaku is worth two han when both the seat and prevailing winds coincide.
        /// </summary>
        [TestMethod()]
        public void WindTriplet1Test()
        {
            List<Meld> Concealed = Meld.FromString("O7O8O9 C4C4C4 B5B6B7 O1O1", ' ');
            List<Meld> Exposed = Meld.FromString("WEWEWE", ' ');
            int expected = 1;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub{GameWind = TileHonors.East};
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.West };
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void WindTriplet2Test()
        {
            List<Meld> Concealed = Meld.FromString("O7O8O9 C4C4C4 B5B6B7 O1O1", ' ');
            List<Meld> Exposed = Meld.FromString("WEWEWE", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub { GameWind = TileHonors.West };
            IPlayer player = new IPlayerStub { PlayerWind = TileHonors.West };
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void TwoIdenticalRunsTest()
        {
            List<Meld> Concealed = Meld.FromString("O3O4O5 O3O4O5 B4B4B4 C1C2C3 NONO", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 1;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TwoIdenticalRunsFalseTest()
        {
            List<Meld> Concealed = Meld.FromString("O3O4O5 B4B4B4 C1C2C3 NONO", ' ');
            List<Meld> Exposed = Meld.FromString("O3O4O5", ' ');
            int expected = 0;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ThreeMatchingRunsClosedTest()
        {
            List<Meld> Concealed = Meld.FromString("B4B5B6 C4C5C6 O4O5O6 O2O2O2 O9O9", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ThreeMatchingRunsOpenTest()
        {
            List<Meld> Concealed = Meld.FromString("B4B5B6 C4C5C6 O2O2O2 O9O9", ' ');
            List<Meld> Exposed = Meld.FromString("O4O5O6", ' ');
            int expected = 1;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void PureStraightClosedTest()
        {
            List<Meld> Concealed = Meld.FromString("C1C2C3 C4C5C6 C7C8C9 B5B6B7 WHWH", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void PureStraightOpenTest()
        {
            List<Meld> Concealed = Meld.FromString("C1C2C3 C7C8C9 B5B6B7 WHWH", ' ');
            List<Meld> Exposed = Meld.FromString("C4C5C6", ' ');
            int expected = 1;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TerminalsOrHonorsOnEachMeldClosedTest()
        {
            List<Meld> Concealed = Meld.FromString("NONO C7C8C9 B1B2B3 O1O1O1 O7O8O9", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TerminalsOrHonorsOnEachMeldOPenTest()
        {
            List<Meld> Concealed = Meld.FromString("NONO C7C8C9 B1B2B3 O7O8O9", ' ');
            List<Meld> Exposed = Meld.FromString("O1O1O1", ' ');
            int expected = 1;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /* Two han yakus start here */
        
        [TestMethod()]
        public void OpenHalfFlushTest()
        {
            List<Meld> Concealed = Meld.FromString("O1O1O1 O4O5O6 O6O7O8 GRGR", ' ');
            List<Meld> Exposed = Meld.FromString("SOSOSO", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ClosedHalfFlushTest()
        {
            List<Meld> Concealed = Meld.FromString("O1O1O1 O4O5O6 SOSOSO O6O7O8 GRGR", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 3;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void OpenFourTripletsTest()
        {
            List<Meld> Concealed = Meld.FromString("B4B4B4 O7O7 NONONO", ' ');
            List<Meld> Exposed = Meld.FromString("C8C8C8 C3C3C3", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// San ankou / Suu ankou, a yakuman
        /// </summary>
        [TestMethod()]
        public void ClosedFourTripletsTest()
        {
            List<Meld> Concealed = Meld.FromString("B4B4B4 O7O7 NONONO C8C8C8 C3C3C3", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 13;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SevenPairsTest()
        {
            List<Meld> Concealed = Meld.FromString("B5B5 O1O1 B9B9 EAEA O4O4 C2C2 B6B6", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 3; //2 7Pairs + 1 NoPointsHand
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ThreeClosedTripletsTest()
        {
            List<Meld> Concealed = Meld.FromString("O9O9O9 B4B4B4 C6C6C6 C1C2C3 RERE", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ThreeOpenTripletsTest()
        {
            List<Meld> Concealed = Meld.FromString("O9O9O9 B4B4B4 C6C6C6 RERE", ' ');
            List<Meld> Exposed = Meld.FromString("C1C2C3", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A hand that has all of the melds and the pair contain terminal tiles and there is at least one run. 
        /// This hand is similar to chanta, except that no honor tiles are included.
        /// +1 han when the hand is closed.
        /// </summary>
        [TestMethod()]
        public void ClosedTerminalInAllMeldsTest()
        {
            List<Meld> Concealed = Meld.FromString("B9B9 O1O2O3 B7B8B9 C1C2C3 B1B1B1", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 3;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void OpenTerminalInAllMeldsTest()
        {
            List<Meld> Concealed = Meld.FromString("B9B9 B7B8B9 B1B1B1", ' ');
            List<Meld> Exposed = Meld.FromString("O1O2O3 C1C2C3", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A hand that has a triplet/kan in each of the three suits with one number in common.
        /// </summary>
        [TestMethod()]
        public void ThreeMatchingTripletsTest()
        {
            List<Meld> Concealed = Meld.FromString("O5O5O5 B5B5B5 C5C5C5 B1B2B3 EAEA", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 2;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /* Hands worth 3 han start here */

        [TestMethod()]
        public void TwoPairsOfIdenticalRunsTest()
        {
            List<Meld> Concealed = Meld.FromString("B3B4B5 B3B4B5 C7C8C9 C7C8C9 WHWH", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 3;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /* Hands worth 4 han start here */

        [TestMethod()]
        public void Small3DragonsTest()
        {
            List<Meld> Concealed = Meld.FromString("WHWHWH O6O7O8 GRGRGR RERE", ' ');
            List<Meld> Exposed = Meld.FromString("C3C3C3", ' ');
            int expected = 4;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A hand that has only honor and terminal tiles. 
        /// Consequently, this hand must contain four triplets/kans or seven pairs. 
        /// If it contains seven pairs, the fu value of this hand is 25. 
        /// The two han for the four triplets or the seven pairs are already included in the han value.
        /// </summary>
        [TestMethod()]
        public void AllHonorsAndTerminalsTest()
        {
            List<Meld> Concealed = Meld.FromString("O1O1O1 WEWE B1B1B1 C9C9C9", ' ');
            List<Meld> Exposed = Meld.FromString("NONONO", ' ');
            int expected = 6; //2 from all triplets, 2 from 3 closed triplets, 2 from all terminals and honors
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AllHonorsAndTerminals7PairsTest()
        {
            List<Meld> Concealed = Meld.FromString("O1O1 NONO WEWE B1B1 C9C9 EAEA C1C1", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 4;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGameStub game = new IGameStub();
            game.GameWind = TileHonors.West; //If the player or game winds are not West, then we also have a no-points hand bonus for one additional Han
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /* 5 han */

        [TestMethod()]
        public void OpenFullFlushTest()
        {
            List<Meld> Concealed = Meld.FromString("B1B1B1 B3B4B5 B6B6B6 B8B8", ' ');
            List<Meld> Exposed = Meld.FromString("B9B9B9", ' ');
            int expected = 5; 
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ClosedFullFlushTest()
        {
            List<Meld> Concealed = Meld.FromString("B1B1B1 B3B4B5 B6B6B6 B8B8 B2B3B4", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 6; //5 flush + 1 closedflush
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        /* Yakuman */

        [TestMethod()]
        public void FourSelfDrawnTripletsTest()
        {
            List<Meld> Concealed = Meld.FromString("C7C7C7 RERERE O6O6O6 O8O8 C2C2C2", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 13;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ThirteenOrphansTest()
        {
            List<Meld> Concealed = Meld.FromString("O1O1 O9B1B9C1C9EASOWENOWHGRRE", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 13;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Big3DragonsTest()
        {
            List<Meld> Concealed = Meld.FromString("WHWHWH grgrgr rerere o4o5o6 c6c6", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 13;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Little4WindsTest()
        {
            List<Meld> Concealed = Meld.FromString("eaeaea soso wewewe nonono c3c4c5", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 13;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Big4WindsTest()
        {
            List<Meld> Concealed = Meld.FromString("eaeaea sososo nonono eaeaea c3c3", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 13;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AllHonorsTest()
        {
            List<Meld> Concealed = Meld.FromString("whwh eaeaea sososo rerere nonono", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 13;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AllTerminalsTest()
        {
            List<Meld> Concealed = Meld.FromString("b1b1b1 c9c9c9 o1o1o1 b9b9 c1c1c1", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 13;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AllGreenTest()
        {
            List<Meld> Concealed = Meld.FromString("b2b3b4 b4b4 b6b6b6 b8b8b8 grgrgr", ' ');
            List<Meld> Exposed = Meld.FromString("", ' ');
            int expected = 13;
            int actual;
            /////////////////////
            bool Riichi = false;
            bool Tsumo = false;
            bool Ippatsu = false;
            IGame game = new IGameStub();
            IPlayer player = new IPlayerStub();
            actual = ScorerJapanese.CountHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            Assert.AreEqual(expected, actual);
        }
    }
}
