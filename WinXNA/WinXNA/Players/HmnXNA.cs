using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong.Async.Players
{
    public enum HmnXNAStatus { Waiting, ChoosingDiscard, AskingTsumo, AskingRon, AskingRiichi, AskingIntercept, ChoosingMeld, AskingCompleteQuad }

    public class HmnXNA : Mahjong.Async.Player
    {
        public HmnXNA()
        {
            this.Status = HmnXNAStatus.Waiting;
        }

        public HmnXNAStatus Status 
        { get; 
            private set; }

        public Mahjong.Async.Player PlayerDiscardingTile { get; private set; }
        public Tile TileBeingDiscarded { get; private set; }

        public KeyValuePair<int,List<Tile>> GetShantenInfo()
        {
            return Hand.GetShantenTiles(MyHand, ExposedMelds);
            //if (shantenInfo.Key >= 0)
            //{
            //    if (shantenInfo.Key == 0) Console.Write("Tenpai.");
            //    else Console.Write("Shanten {0}.", shantenInfo.Key);

            //    if (shantenInfo.Value != null && shantenInfo.Value.Count > 0)
            //    {
            //        shantenInfo.Value.Sort();
            //        shantenInfo.Value.WriteColoredToConsole();
            //    }
            //}
        }

        public List<Tile> MyTiles
        {
            get
            {
                return this.MyHand;
            }
        }

        public Tile LastDraw { get { return base.LastDraw; } }

        //public bool HearTileClicks { get; private set; }

        public string Message
        {
            get;
            private set;
        }

        public void TileWasClicked(Tile ClickedTile)
        {
            if (this.Status == HmnXNAStatus.ChoosingDiscard && this.callback_ChooseDiscard != null)
            {
                this.Action_ChooseDiscard(ClickedTile);
            }
            else if (this.Status == HmnXNAStatus.AskingIntercept && this.callback_WantsMeld != null)
            {
                this.Action_WantsIntercept(ClickedTile);
            }
            else if (this.Status == HmnXNAStatus.AskingRon && this.callback_Ron != null)
            {
                this.Action_ChooseRon(ClickedTile == this.TileBeingDiscarded);
            }
        }

        private void Action_WantsIntercept(Tile ClickedTile)
        {
            if (ClickedTile == null)
            {
                this.Message = String.Empty;
                this.Status = HmnXNAStatus.Waiting;

                var tmp = this.callback_WantsMeld;
                this.callback_WantsMeld = null;
                tmp.Invoke(null);
            }
            else if (this.discardAvailableMelds != null && this.discardAvailableMelds.Count == 1)
            {
                this.Message = String.Empty;
                this.Status = HmnXNAStatus.Waiting;

                var tmp = this.callback_WantsMeld;
                this.callback_WantsMeld = null;
                tmp.Invoke(discardAvailableMelds.First());
            }
            else
            {
                this.Message = "Click on the meld to expose";
                this.Status = HmnXNAStatus.ChoosingMeld;
            }
        }
        public void AskForMeldCallback(Meld SelectedMeld)
        {
            if (this.Status == HmnXNAStatus.ChoosingMeld && this.discardAvailableMelds != null && this.discardAvailableMelds.Contains(SelectedMeld) && this.callback_WantsMeld != null)
            {
                this.Status = HmnXNAStatus.Waiting;

                var tmp = this.callback_WantsMeld;
                this.callback_WantsMeld = null;
                tmp.Invoke(SelectedMeld);
            }
        }

        private void Action_ChooseDiscard(Tile ClickedTile)
        {
            this.Message = String.Empty;
            this.Status = HmnXNAStatus.Waiting;

            var tmp = this.callback_ChooseDiscard;
            this.callback_ChooseDiscard = null;
            tmp.Invoke(ClickedTile);
        }

        Player.Logic_ChooseDiscard_Delegate callback_ChooseDiscard;
        protected override void Logic_ChooseDiscard(Player.Logic_ChooseDiscard_Delegate Callback)
        {
            this.Status = HmnXNAStatus.ChoosingDiscard;

            this.callback_ChooseDiscard = Callback;
            this.Message = "Click on a tile to discard";
            //this.HearTileClicks = true;

            //throw new NotImplementedException();
            //Callback.Invoke(this.LastDraw);
        }

        public List<Meld> discardAvailableMelds { get; private set; }
        Player.Logic_WantsMeld_Delegate callback_WantsMeld;
        protected override void Logic_WantsMeld(Player Discarder, Tile DiscardedTile, List<Meld> AvailableMelds, Player.Logic_WantsMeld_Delegate callback)
        {
            this.TileBeingDiscarded = DiscardedTile;
            this.PlayerDiscardingTile = Discarder;

            this.Status = HmnXNAStatus.AskingIntercept;
            this.discardAvailableMelds = AvailableMelds;
            this.callback_WantsMeld = callback;
            this.Message = String.Format("Click on {0}'s {1} to call it.\nClick anywhere else to skip it.",Discarder.Name,DiscardedTile.ToLongString());
        }

        #region Riichi
        public event EventHandler AskForRiichi;
        public List<Tile> riichiDiscardTiles { get; private set; }
        private Player.Logic_WantsRiichi_Delegate Logic_WantsRiichi_Callback;
        protected override void Logic_WantsRiichi(List<Tile> Discards, Player.Logic_WantsRiichi_Delegate callback)
        {
            this.Logic_WantsRiichi_Callback = callback;
            this.Status = HmnXNAStatus.AskingRiichi;
            this.Message = "Want to declare riichi?\nYou'd have to discard one of the highlighted tiles.";
            this.riichiDiscardTiles = Discards;

            if (null != this.AskForRiichi)
                this.AskForRiichi.Invoke(this, new EventArgs());
        }
        public void AskForRiichiCallback(bool WantsRiichi)
        {
            if (this.Status == HmnXNAStatus.AskingRiichi && null != this.Logic_WantsRiichi_Callback)
            {
                this.Status = HmnXNAStatus.Waiting;
                this.Logic_WantsRiichi_Callback.Invoke(WantsRiichi);
                this.Logic_WantsRiichi_Callback = null;
            }
        }
        #endregion

        #region Complete Quad
        private Player.Logic_Wants_CompleteQuad_Delegate Logic_WantsQuad_Callback;
        public event EventHandler<MahjongXNA.EventArg<Tile>> AskForCompleteQuad;
        public void AskForCompleteQuadCallback(bool WantsQuad)
        {
            if (this.Status == HmnXNAStatus.AskingCompleteQuad && null != this.Logic_WantsQuad_Callback)
            {
                this.Status = HmnXNAStatus.Waiting;
                this.Logic_WantsQuad_Callback.Invoke(WantsQuad);
                this.Logic_WantsQuad_Callback = null;
            }
        }
        protected override void Logic_Wants_CompleteQuad(Tile Bonus, Player.Logic_Wants_CompleteQuad_Delegate callback)
        {
            this.Logic_WantsQuad_Callback = callback;
            this.Status = HmnXNAStatus.AskingCompleteQuad;
            this.Message = String.Format("Do you want to complete your kan for {0}?",Bonus.ToLongString());

            if (null != this.AskForCompleteQuad)
                this.AskForCompleteQuad.Invoke(this, new MahjongXNA.EventArg<Tile>(Bonus));
        }
        #endregion

        #region Tsumo
        private Player.Logic_WantsTsumo_Delegate Logic_WantsTsumo_Callback;
        public event EventHandler<MahjongXNA.EventArg<Tile>> AskForTsumo;
        public void AskForTsumoCallback(bool WantsTsumo)
        {
            if (this.Status == HmnXNAStatus.AskingTsumo && null != this.Logic_WantsTsumo_Callback)
            {
                this.Status = HmnXNAStatus.Waiting;
                this.Logic_WantsTsumo_Callback.Invoke(WantsTsumo);
                this.Logic_WantsTsumo_Callback = null;
            }
        }
        protected override void Logic_WantsTsumo(Tile ReceivedTile, Player.Logic_WantsTsumo_Delegate callback)
        {
            this.Logic_WantsTsumo_Callback = callback;
            this.Status = HmnXNAStatus.AskingTsumo;
            this.Message = "Do you want to tsumo?";

            if (null != this.AskForTsumo)
                this.AskForTsumo.Invoke(this, new MahjongXNA.EventArg<Tile>(ReceivedTile));
        }
        #endregion

        #region Ron
        private Player.Logic_WantsRon_Delegate callback_Ron;
        protected override void Logic_WantsRon(Player Discarder, Tile DiscardedTile, Player.Logic_WantsRon_Delegate callback)
        {
            this.callback_Ron = callback;
            this.Status = HmnXNAStatus.AskingRon;
            this.PlayerDiscardingTile = Discarder;
            this.TileBeingDiscarded = DiscardedTile;
            this.Message = String.Format("Click on {0}'s {1} to ron him", Discarder.Name, DiscardedTile.ToLongString());
        }
        private void Action_ChooseRon(bool WantsRon)
        {
            this.Message = String.Empty;
            this.Status = HmnXNAStatus.Waiting;

            var tmp = this.callback_Ron;
            this.callback_Ron = null;
            tmp.Invoke(WantsRon);
        }
        #endregion
    }
}
