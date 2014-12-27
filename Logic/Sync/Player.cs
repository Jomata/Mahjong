using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong.Sync
{
    public abstract class Player
    {
        public string Name { get; set; }
        protected List<Tile> MyHand = new List<Tile>();
        //public List<Tile> ExposedMelds = new List<Tile>();
        //public List<Tile> Discards = new List<Tile>();
        public List<Meld> ExposedMelds { get; private set; }
        public List<Tile> Discards { get; private set; }
        public Game Game;

        private bool HandRevealed = false;
        public List<Tile> RevealedHand 
        {
            get
            {
                if (this.HandRevealed)
                {
                    this.MyHand.Sort();
                    return this.MyHand;
                }
                else return new List<Tile>();
            }
        }

        protected Player()
        {
            this.ExposedMelds = new List<Meld>();
            this.Discards = new List<Tile>();
            this.HandRevealed = false;
        }

        public int HandTilesCount
        {
            get { return MyHand.Count; }
        }

        protected Tile LastDraw { private set; get; }

        /// <summary>
        /// Adds a tile to the player's hand, either by self draw, or another player's discard
        /// </summary>
        /// <param name="Tile">The received tile</param>
        /// <returns>True if the player wins with the obtained tile, false if not</returns>
        public bool Draw(Tile Tile)
        {
            MyHand.Sort();
            this.LastDraw = Tile;
            MyHand.Add(Tile);
            //return MyHand.Concat(ExposedMelds).IsWinningHand();
            if (this.CheckWinningHand())
            {
                this.RevealHand();
                return true;
            }
            else
            {
                return false;
            }
        }
        public Tile Discard()
        {
            var discardedTile = this.Logic_ChooseDiscard();
            MyHand.Remove(discardedTile);
            this.LastDraw = null;
            //Discards.Add(discardedTile);
            return discardedTile;
        }

        private bool CheckWinningHand()
        {
            //return Mahjong.Hand.IsValidHand(MyHand, ExposedMelds, this.Game, this);
            return Mahjong.Hand.IsValidHand(typeof(ScorerJapanese), MyHand, ExposedMelds, null, null);
        }

        private List<List<Tile>> getMatchingTriplets(Tile ToTest)
        {
            var results = new List<List<Tile>>();

            var matchingTiles = MyHand.Where(x => x.IsPairOrTriplet(ToTest));
            if (matchingTiles.Count() >= 2) results.Add(matchingTiles.Take(2).ToList()); //Pon
            if (matchingTiles.Count() >= 3) results.Add(matchingTiles.Take(3).ToList()); //Kan

            return results;
        }

        private List<List<Tile>> getMatchingSequences(Tile ToTest)
        {
            var results = new List<List<Tile>>();

            var Minus2 = MyHand.Find(x => x.Value() - 2 == ToTest.Value());
            var Minus1 = MyHand.Find(x => x.Value() - 1 == ToTest.Value());
            var Plus1 = MyHand.Find(x => x.Value() + 1 == ToTest.Value());
            var Plus2 = MyHand.Find(x => x.Value() + 2 == ToTest.Value());

            if (null != Minus2 && null != Minus1) results.Add(new List<Tile> { Minus2, Minus1 });
            if (null != Minus1 && null != Plus1) results.Add(new List<Tile> { Minus1, Plus1 });
            if (null != Plus1 && null != Plus2) results.Add(new List<Tile> { Plus1, Plus2 });
            
            return results;
        }

        private void RevealHand()
        {
            this.HandRevealed = true;
        }

        public bool WantsDiscard(Tile DiscardedTile, Player Discarder)
        {
            if (CheckRon(DiscardedTile))
            {
                if (this.Logic_WantsRon(DiscardedTile, Discarder))
                {
                    this.AllMyTiles.Add(DiscardedTile);
                    this.RevealHand();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //For a triplet, we want at least 2 of the same kind in the hand
            //if (MyHand.Count(x => x.IsPairOrTriplet(DiscardedTile)) >= 2)
            if(getMatchingTriplets(DiscardedTile).Count > 0)
            {
                return this.Logic_WantsDiscard(DiscardedTile, Discarder);
            }
            //For a pair, we need either X-2,X-1; X-1,X+1; X+1,X+2
            if (getMatchingSequences(DiscardedTile).Count > 0)
            {
                return this.Logic_WantsDiscard(DiscardedTile, Discarder);
            }

            return false;
        }

        private bool CheckRon(Tile ToCheck)
        {
            //return MyHand.Concat(ExposedMelds).Concat(new List<Tile>{ToCheck}).IsWinningHand();
            //return AllMyTiles.Concat(new List<Tile> { ToCheck }).IsWinningHand();
            
            //return Hand.IsValidHand(MyHand.Concat(ToCheck).ToList(), ExposedMelds);
            return Hand.IsValidHand(typeof(ScorerJapanese), MyHand.Concat(ToCheck).ToList(), ExposedMelds, null, null);
        }

        private List<Tile> AllMyTiles
        {
            get
            {
                var allExposedTiles = from exposed in ExposedMelds
                          from exposedTile in exposed.Tiles
                          select exposedTile;

                return MyHand.Concat(allExposedTiles).ToList();
            }
        }

        /// <summary>
        /// Adds intercepted tile to hand, exposed the meld it was used for
        /// </summary>
        /// <param name="InterceptedTile">The tile that was intercepted</param>
        /// <returns>True if the player wins with the obtained tile, false if not</returns>
        public bool ProcessInterception(Tile InterceptedTile)
        {
            //If I've already exposed my hand, it means it was a Ron
            if (this.HandRevealed)
            {
                this.MyHand.Add(InterceptedTile);
                return true;
            }

            var triplets = getMatchingTriplets(InterceptedTile);
            var sequences = getMatchingSequences(InterceptedTile);
            var melds = triplets.Concat(sequences).ToList();

            List<Tile> exposed;

            if (melds.Count == 1)
            {
                exposed = melds.First();
            }
            else
            {
                exposed = Logic_ChooseExposedTilesAfterIntercept(melds);

                //If the meld selected by the logic is not in the available melds, we select the first one. Cheaters.
                if (!melds.Contains(exposed)) exposed = melds.First();
            }

            MyHand.RemoveAll(x => exposed.Contains(x));

            var newMeld = new Meld(exposed);
            newMeld.Tiles.Add(InterceptedTile);
            ExposedMelds.Add(newMeld);

            //ExposedMelds.AddRange(exposed);
            //ExposedMelds.Add(InterceptedTile);
            //return MyHand.Concat(this.ExposedMelds).IsWinningHand();
            return this.CheckWinningHand();
        }

        protected abstract Tile Logic_ChooseDiscard();
        protected abstract bool Logic_WantsDiscard(Tile Tile, Player Discarder);
        protected abstract List<Tile> Logic_ChooseExposedTilesAfterIntercept(List<List<Tile>> Melds);
        protected abstract bool Logic_WantsRon(Tile Tile, Player Discarder);
    }
}
