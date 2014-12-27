using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong.Async.Players
{
    public class AIFirst : Mahjong.Async.Player
    {
        protected override void Logic_WantsTsumo(Tile ReceivedTile, Player.Logic_WantsTsumo_Delegate callback)
        {
            callback.Invoke(true);
        }

        protected override void Logic_ChooseDiscard(Player.Logic_ChooseDiscard_Delegate Callback)
        {
            if (this.Riichi)
                Callback.Invoke(this.LastDraw);
            else
                Callback.Invoke(this.MyHand.First());
        }

        protected override void Logic_WantsRon(Player Discarder, Tile DiscardedTile, Player.Logic_WantsRon_Delegate callback)
        {
            callback.Invoke(true);
        }

        protected override void Logic_WantsMeld(Player Discarder, Tile DiscardedTile, List<Meld> AvailableMelds, Player.Logic_WantsMeld_Delegate callback)
        {
            //AvailableMelds.First().Tiles.RemoveAt(0);
            //AvailableMelds.First().Tiles.Add(Tile.Unknown());

            callback.Invoke(AvailableMelds.First());
        }

        protected override void Logic_WantsRiichi(List<Tile> Discards, Player.Logic_WantsRiichi_Delegate callback)
        {
            callback.Invoke(true);
        }

        protected override void Logic_Wants_CompleteQuad(Tile Bonus, Player.Logic_Wants_CompleteQuad_Delegate callback)
        {
            callback.Invoke(true);
        }
    }
}
