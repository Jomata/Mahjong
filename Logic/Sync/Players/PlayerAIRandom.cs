using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mahjong.Sync;

namespace Mahjong.Sync.Players
{
    public class PlayerAIRandom : Player
    {
        private Random Brain;

        public PlayerAIRandom() : base()
        {
            this.Brain = new Random();
        }

        protected override Tile Logic_ChooseDiscard()
        {
            return this.MyHand.GetRandom(this.Brain);
        }

        protected override bool Logic_WantsDiscard(Tile Tile, Player Discarder)
        {
            //return this.Brain.Next(0,2) == 0; //returns 0 or 1
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
