using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong.Async.Players
{
    public class AIShantenDelayed : AIShantenPlus
    {
        public TimeSpan Delay { get; set; }

        public AIShantenDelayed()
            : base()
        {
            this.Delay = TimeSpan.FromSeconds(0);
        }

        private System.Threading.Timer timer;

        private void DoDelay(Delegate Callback)
        {
            this.timer = new System.Threading.Timer(this.TimerCallback, Callback, (long)Delay.TotalMilliseconds, System.Threading.Timeout.Infinite);
        }

        private void TimerCallback(Object state)
        {
            if (state is Logic_ChooseDiscard_Delegate)
                base.Logic_ChooseDiscard(state as Logic_ChooseDiscard_Delegate);
            else if (state is Logic_WantsMeld_Delegate)
                base.Logic_WantsMeld(Logic_WantsMeld_Discarder, Logic_WantsMeld_DiscardedTile, Logic_WantsMeld_AvailableMelds, state as Logic_WantsMeld_Delegate);
            else if (state is Logic_WantsRiichi_Delegate)
                base.Logic_WantsRiichi(Logic_WantsRiichi_Discards, state as Logic_WantsRiichi_Delegate);
            else if (state is Logic_WantsRon_Delegate)
                base.Logic_WantsRon(Logic_WantsRon_Discarder, Logic_WantsRon_DiscardedTile, state as Logic_WantsRon_Delegate);
            else if (state is Logic_WantsTsumo_Delegate)
                base.Logic_WantsTsumo(Logic_WantsTsumo_ReceivedTile, state as Logic_WantsTsumo_Delegate);
        }

        protected override void Logic_ChooseDiscard(Logic_ChooseDiscard_Delegate Callback)
        {
            this.DoDelay(Callback);
        }

        Player Logic_WantsMeld_Discarder;
        Mahjong.Tile Logic_WantsMeld_DiscardedTile;
        List<Meld> Logic_WantsMeld_AvailableMelds;
        protected override void Logic_WantsMeld(Player Discarder, Tile DiscardedTile, List<Meld> AvailableMelds, Logic_WantsMeld_Delegate callback)
        {
            this.Logic_WantsMeld_Discarder = Discarder;
            this.Logic_WantsMeld_DiscardedTile = DiscardedTile;
            this.Logic_WantsMeld_AvailableMelds = AvailableMelds;
            this.DoDelay(callback);
            //base.Logic_WantsMeld(Discarder, DiscardedTile, AvailableMelds, callback);
        }

        List<Tile> Logic_WantsRiichi_Discards;
        protected override void Logic_WantsRiichi(List<Tile> Discards, Logic_WantsRiichi_Delegate callback)
        {
            //this.Logic_WantsRiichi_Discards = Discards;
            //this.DoDelay(callback);
            base.Logic_WantsRiichi(Discards, callback);
        }

        Player Logic_WantsRon_Discarder;
        Tile Logic_WantsRon_DiscardedTile;
        protected override void Logic_WantsRon(Player Discarder, Tile DiscardedTile, Logic_WantsRon_Delegate callback)
        {
            this.Logic_WantsRon_Discarder = Discarder;
            this.Logic_WantsRon_DiscardedTile = DiscardedTile;
            this.DoDelay(callback);
            //base.Logic_WantsRon(Discarder, DiscardedTile, callback);
        }

        Tile Logic_WantsTsumo_ReceivedTile;
        protected override void Logic_WantsTsumo(Tile ReceivedTile, Logic_WantsTsumo_Delegate callback)
        {
            this.Logic_WantsTsumo_ReceivedTile = ReceivedTile;
            this.DoDelay(callback);
            //base.Logic_WantsTsumo(ReceivedTile, callback);
        }
    }
}
