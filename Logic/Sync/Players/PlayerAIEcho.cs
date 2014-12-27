using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mahjong.Sync;

namespace Mahjong.Sync.Players
{
    public class PlayerAIEcho : Player
    {
        protected override Tile Logic_ChooseDiscard()
        {
            return this.LastDraw;
        }

        protected override bool Logic_WantsDiscard(Tile Tile, Player Discarder)
        {
            return false;
        }

        protected override List<Tile> Logic_ChooseExposedTilesAfterIntercept(List<List<Tile>> Melds)
        {
            throw new NotImplementedException();
        }

        protected override bool Logic_WantsRon(Tile Tile, Player Discarder)
        {
            return true;
        }
    }
}
