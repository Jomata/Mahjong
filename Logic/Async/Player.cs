using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong.Async
{
    //public delegate void WantsDiscardEventHandler(Tile SelectedTile, Player Player);
    //public delegate void WantsRonEventHandler(Tile SelectedTile, Player Player);
    //public delegate void WantsTsumoEventHandler(Tile ReceivedTile, Player Player);
    //public delegate void DiscardDecidedEventHandler(Tile SelectedTile, Player Player);

    public abstract class Player : IPlayer
    {
        public virtual string Name { get; set; }
        private Game _Game;

        public Game Game
        {
            get { return _Game; }
            set { _Game = value; this.privateOnGameSet(value); }
        }

        public TileHonors PlayerWind { get; private set; }
        

        protected List<Tile> MyHand = new List<Tile>();
        public List<Meld> ExposedMelds { get; private set; }
        public List<Tile> Discards { get; private set; }
        protected Tile LastDraw { get; private set; }

        public List<Meld> RevealedHand {private set;get;}

        public Tile WinningTile { get; private set; }

        private void privateOnGameSet(Game NewGame)
        {
            this.PlayerWind = NewGame.GetPlayerWind(this);
            NewGame.GameStart += new EventHandler(Game_GameStart);

            this.OnGameSet(NewGame);
        }

        protected virtual void OnGameSet(Game NewGame){}

        void Game_GameStart(object sender, EventArgs e)
        {
            this.MyHand.Clear();
            this.ExposedMelds.Clear();
            this.Discards.Clear();
            this.Riichi = false;
            this.Ippatsu = false;
            this.PlayerWind = this.Game.GetPlayerWind(this);
            this.RevealedHand = new List<Meld>();
            this.WinningTile = null;
            this.LastDraw = null;
            this.Tsumo = false;
        }

        public int HandTilesCount
        {
            get { return this.MyHand.Count; }
        }

        public Player()
        {
            this.MyHand = new List<Tile>();
            this.ExposedMelds = new List<Meld>();
            this.Discards = new List<Tile>();
        }

        private void RevealHand(Tile WinningTile)
        {
            this.WinningTile = WinningTile;
            this.RevealedHand = Hand.GetBestHand(typeof(ScorerJapanese), this.MyHand, this.ExposedMelds, this.Game, this);
        }

        #region Kan replacement
        public void ReceiveKanReplacement(Tile Rinshan, Player OriginalDiscarder,Mahjong.Async.PlayerWantsRon callback)
        {
            MyHand.Add(Rinshan);
            this.LastDraw = Rinshan;

            if (CheckRon(Rinshan))
            {
                this.discardTileBeingChecked = Rinshan;
                this.WantsRon_callback = callback;
                this.Logic_WantsRon(OriginalDiscarder, Rinshan, this.Logic_WantsRon_Callback);
            }
            else
            {
                callback.Invoke(this, false);
            }
        }
        #endregion

        #region Receive Tile
        private PlayerDraw PlayerDraw_callback;
        public void ReceiveTile(Tile tile, PlayerDraw callback)
        {
            this.MyHand.Add(tile);
            this.LastDraw = tile;
            this.PlayerDraw_callback = callback;

            //Can we tsumo?
            if (Hand.IsValidHand(typeof(ScorerJapanese), MyHand, ExposedMelds, this.Game, this))
            {
                this.Logic_WantsTsumo(tile, this.Logic_WantsTsumo_Callback);
            }
            //Can we complete a triplet as a quad?
            else if (this.ExposedMelds.Any(x => x.Type == MeldType.Triplet && x.Tiles.Any(y => y.IsPairOrTriplet(tile))))
            {
                this.Logic_Wants_CompleteQuad(tile, this.Logic_Wants_CompleteQuad_Callback);
            }
            else
            {
                callback.Invoke(this, PlayerDrawResponse.Discard);
            }            
        }
        protected abstract void Logic_WantsTsumo(Tile ReceivedTile, Logic_WantsTsumo_Delegate callback);
        protected delegate void Logic_WantsTsumo_Delegate(bool WantsTsumo);
        private void Logic_WantsTsumo_Callback(bool WantsTsumo)
        {
            if (WantsTsumo)
            {
                this.Tsumo = true;
                this.RevealHand(this.LastDraw);
                this.PlayerDraw_callback.Invoke(this, PlayerDrawResponse.Tsumo);
            }
            else
            {
                this.PlayerDraw_callback.Invoke(this, PlayerDrawResponse.Discard);
            }
        }
        #endregion

        #region Choose Discard
        private PlayerDiscardSelected DiscardSelected_callback;
        public void DiscardTile(PlayerDiscardSelected callback)
        {
            bool askedForRiichi = false;
            this.DiscardSelected_callback = callback;

            //If it's my turn again, and I already riichi'd, it's no longer ippatsu
            //I get here after the tsumo check, so no chance of making this accidentally remove my ippatsu
            if (this.Ippatsu && this.Riichi && this.Game.CurrentPlayer == this)
            {
                this.Ippatsu = false;
            }

            if (!this.Riichi && this.ExposedMelds.Count == 0) //riichi only on closed hands, and when I haven't riichi'd already
            {
                var riichiDiscards = Hand.GetDiscardsForTenpai(typeof(ScorerJapanese), this.MyHand, this.ExposedMelds, this.Game, this);

                if (riichiDiscards.Count > 0) 
                {
                    askedForRiichi = true;
                    this.originalRiichiDiscards = riichiDiscards;
                    this.Logic_WantsRiichi(riichiDiscards, this.Logic_WantsRiichi_Callback);
                }
            }

            if (!askedForRiichi)
            {
                this.Logic_ChooseDiscard(this.Logic_ChooseDiscard_Callback);
            }
        }
        protected abstract void Logic_ChooseDiscard(Logic_ChooseDiscard_Delegate Callback);
        protected delegate void Logic_ChooseDiscard_Delegate(Tile SelectedTile);
        private void Logic_ChooseDiscard_Callback(Tile SelectedTile)
        {
            //We check that the selected tile is valid
            if (!MyHand.Contains(SelectedTile)) 
                this.Logic_ChooseDiscard(this.Logic_ChooseDiscard_Callback);
            //If we just riichi'd, we can only discard one of those tiles
            if (this.Riichi && this.originalRiichiDiscards.Count > 0 && this.originalRiichiDiscards.Count(riichiDiscard => riichiDiscard.Value() == SelectedTile.Value()) == 0) 
                this.Logic_ChooseDiscard(this.Logic_ChooseDiscard_Callback);
            //On subsequent riichi turns, we can only discard the tile we just received
            if (this.Riichi && this.originalRiichiDiscards.Count == 0 && SelectedTile != this.LastDraw) 
                this.Logic_ChooseDiscard(this.Logic_ChooseDiscard_Callback);

            MyHand.Remove(SelectedTile);
            this.originalRiichiDiscards = new List<Tile>();
            this.LastDraw = null;
            this.DiscardSelected_callback.Invoke(this, SelectedTile);
        }
        #endregion

        #region Wants Meld
        private PlayerWantsMeld WantsMeld_callback;
        private Tile InterceptedTile = null;
        private Player InterceptedPlayer = null;
        public void WantsMeld(Player Discarder, Tile DiscardedTile, PlayerWantsMeld callback)
        {
            this.InterceptedTile = DiscardedTile;
            this.InterceptedPlayer = Discarder;

            if (this.Riichi) //if we're in riichi, we can only Ron
            {
                callback.Invoke(this, null);
            }
            else
            {
                var triplets = getMatchingTriplets(DiscardedTile);
                var sequences = getMatchingSequences(DiscardedTile);

                if (triplets.Count > 0 || sequences.Count > 0)
                {
                    this.WantsMeld_callback = callback;
                    this.Logic_WantsMeld(Discarder, DiscardedTile, triplets.Concat(sequences).ToList(), this.Logic_WantsMeld_Callback);
                }
                else
                {
                    callback.Invoke(this, null);
                }
            }
        }
        protected abstract void Logic_WantsMeld(Player Discarder, Tile DiscardedTile, List<Meld> AvailableMelds, Logic_WantsMeld_Delegate callback);
        protected delegate void Logic_WantsMeld_Delegate(Meld ChosenMeld);
        private void Logic_WantsMeld_Callback(Meld ChosenMeld)
        {
            //do not want
            if (ChosenMeld == null)
            {
                this.WantsMeld_callback.Invoke(this, null);
            }
            //check that the meld is actually viable
            else if (MyHand.Concat(this.InterceptedTile).Count(x => ChosenMeld.Tiles.Contains(x)) == ChosenMeld.Tiles.Count)
            {
                MyHand.RemoveAll(x => ChosenMeld.Tiles.Contains(x));
                this.ExposedMelds.Add(ChosenMeld);
                this.WantsMeld_callback.Invoke(this, ChosenMeld);
            }
            //Don't be a freaking cheater
            else
            {
                var triplets = getMatchingTriplets(this.InterceptedTile);
                var sequences = getMatchingSequences(this.InterceptedTile);
                this.Logic_WantsMeld(this.InterceptedPlayer, this.InterceptedTile, triplets.Concat(sequences).ToList(), this.Logic_WantsMeld_Callback);
            }
        }
        #endregion
        
        #region Wants Ron
        private Tile discardTileBeingChecked;
        private PlayerWantsRon WantsRon_callback;
        public void WantsRon(Player Discarder, Tile DiscardedTile, PlayerWantsRon callback)
        {
            if (CheckRon(DiscardedTile))
            {
                this.discardTileBeingChecked = DiscardedTile;
                this.WantsRon_callback = callback;
                this.Logic_WantsRon(Discarder, DiscardedTile, this.Logic_WantsRon_Callback);
            }
            else
            {
                callback.Invoke(this, false);
            }
        }
        protected abstract void Logic_WantsRon(Player Discarder, Tile DiscardedTile, Logic_WantsRon_Delegate callback);
        protected delegate void Logic_WantsRon_Delegate(bool WantsRon);
        private void Logic_WantsRon_Callback(bool WantsRon)
        {
            if (WantsRon)
            {
                this.MyHand.Add(this.discardTileBeingChecked);
                this.RevealHand(this.discardTileBeingChecked);
            }

            this.WantsRon_callback.Invoke(this, WantsRon);
        }
        #endregion

        #region Wants Riichi
        List<Tile> originalRiichiDiscards = new List<Tile>();
        protected abstract void Logic_WantsRiichi(List<Tile> Discards, Logic_WantsRiichi_Delegate callback);
        protected delegate void Logic_WantsRiichi_Delegate(bool WantsRiichi);
        private void Logic_WantsRiichi_Callback(bool DeclareRiichi)
        {
            if (DeclareRiichi)
            {   
                this.Riichi = true;
                this.Ippatsu = true;
            }
            else
            {
                this.originalRiichiDiscards = new List<Tile>();
            }

            this.Logic_ChooseDiscard(this.Logic_ChooseDiscard_Callback);
        }
        #endregion

        /// <summary>
        /// Closed quad: Players can make a closed quad by calling out "kan" using the same four tiles in their hand. 
        /// They reveal the meld on the table usually with the two inside tiles faced up and the two outside tiles faced down. 
        /// A closed quad doesn't use another player's discard, but a player must declare and reveal a quad if they wish to draw a supplemental tile from the dead wall. 
        /// Declaring a closed quad doesn't open a hand.
        /// </summary>
        #region Wants closed Kan
        
        #endregion

        #region Wants to complete Kan
        /// <summary>
        ///  Players can make an added open quad (kakan; 加槓) by calling out "kan." 
        ///  They can add a self-drawn tile or a tile already in their hand to an open meld of the same three tiles.[8] 
        ///  The tile is usually added sideways on top of the sideways tile in the open meld.
        /// </summary>
        protected abstract void Logic_Wants_CompleteQuad(Tile Bonus, Logic_Wants_CompleteQuad_Delegate callback);
        protected delegate void Logic_Wants_CompleteQuad_Delegate(bool CompleteQuad);
        private void Logic_Wants_CompleteQuad_Callback(bool CompleteQuad)
        {
            //if we do, we need to draw again/etc
            if (CompleteQuad)
            {
                this.MyHand.Remove(this.LastDraw);
                this.ExposedMelds.First(x => x.Type == MeldType.Triplet && x.Tiles.Any(y => y.IsPairOrTriplet(LastDraw))).Tiles.Add(this.LastDraw);

                this.PlayerDraw_callback.Invoke(this, PlayerDrawResponse.CompleteQuad);
            }
            //if we don't, just discard
            else
            {
                this.PlayerDraw_callback.Invoke(this, PlayerDrawResponse.Discard);
            }
        }
        #endregion

        private bool CheckRon(Tile ToCheck)
        {
            return Hand.IsValidHand(typeof(ScorerJapanese), MyHand.Concat(ToCheck).ToList(), ExposedMelds, this.Game, this);
        }

        private List<Meld> getMatchingTriplets(Tile ToTest)
        {
            var results = new List<Meld>();

            var matchingTiles = MyHand.Where(x => x.IsPairOrTriplet(ToTest));
            if (matchingTiles.Count() >= 2) results.Add(new Meld(matchingTiles.Take(2).Concat(ToTest).ToList())); //Pon
            if (matchingTiles.Count() >= 3) results.Add(new Meld(matchingTiles.Take(3).Concat(ToTest).ToList())); //Kan

            return results;
        }

        private List<Meld> getMatchingSequences(Tile ToTest)
        {
            var results = new List<Meld>();

            var Minus2 = MyHand.Find(x => x.Value() == ToTest.Value() - 2);
            var Minus1 = MyHand.Find(x => x.Value() == ToTest.Value() - 1);
            var Plus1 =  MyHand.Find(x => x.Value() == ToTest.Value() + 1);
            var Plus2 =  MyHand.Find(x => x.Value() == ToTest.Value() + 2);

            if (null != Minus2 && null != Minus1) results.Add(new Meld(new List<Tile> { Minus2, Minus1,ToTest }));
            if (null != Minus1 && null != Plus1) results.Add(new Meld(new List<Tile> { Minus1, ToTest, Plus1 }));
            if (null != Plus1 && null != Plus2) results.Add(new Meld(new List<Tile> { ToTest, Plus1, Plus2 }));

            return results;
        }


        public bool Tsumo
        {
            get;
            private set;
        }

        public bool Ippatsu
        {
            get;
            private set;
        }

        public bool Riichi
        {
            get;
            private set;
        }


        public bool IsDealer
        {
            get { return this.PlayerWind == TileHonors.East; }
        }
    }
}
