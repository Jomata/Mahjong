using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public abstract class Scorer
    {
        protected List<Meld> Concealed { get; private set; }
        protected List<Meld> Exposed { get; private set; }
        protected IGame Game { get; private set; }
        protected IPlayer Player { get; private set; }
        public Scorer(List<Meld> Concealed, List<Meld> Exposed, IGame Game, IPlayer Player)
        {
            this.Concealed = Concealed;
            this.Exposed = Exposed;
            this.Game = Game;
            this.Player = Player;
        }

        //Maybe I should add some convenience properties: Riichi, Ippatsu, Tsumo, last-draw, etc.

        public abstract int GetBasicValue();
        public abstract IDictionary<IPlayer,int> GetPayments();
        public abstract int CountFu();
        public abstract int CountHan();
        public abstract List<Han> GetHan();
        public abstract List<Fu> GetFu();
        public abstract bool IsCompleteHand();
    }
}
