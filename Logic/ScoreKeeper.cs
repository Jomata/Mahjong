using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public class ScoreKeeper
    {
        IList<IDictionary<IPlayer, int>> roundPayments;
        private int StartingPoints {get;set;}
        public List<IPlayer> Players { get; private set; }

        public ScoreKeeper() : this(25000) { }
        public ScoreKeeper(int StartingPoints) : this(StartingPoints, new List<IPlayer>()) { }
        public ScoreKeeper(List<IPlayer> Players) : this(25000, Players) { }
        public ScoreKeeper(int StartingPoints, List<IPlayer> Players)
        {
            this.Players = Players;
            this.StartingPoints = StartingPoints;
            this.roundPayments = new List<IDictionary<IPlayer, int>>();
        }

        public bool RegisterPlayer(IPlayer Player)
        {
            if (this.Players.Count >= 4) return false;

            this.Players.Add(Player);
            return true;
        }

        public void RegisterRoundPayments(IDictionary<IPlayer, int> Payments)
        {
            this.roundPayments.Add(Payments);
        }

        public IList<int> GetPlayerPaymentsHistory(IPlayer Player)
        {
            var payments = new List<int>();
            foreach (var item in this.roundPayments)
            {
                var curr = item.Keys.FirstOrDefault(x => x.Name == Player.Name);
                if (null != curr)
                    payments.Add(item[curr]);
                else
                    payments.Add(0);
            }
            return payments;

            //if (this.roundPayments.Any(round => round.ContainsKey(Player)))
            //{
            //    var foo = this.roundPayments.Where(round => round.ContainsKey(Player)); //where is only returning one?
            //    var bar = foo.Select(round => round[Player]);
            //    return bar.ToList();
            //}
            //else 
            //    return new List<int>();
        }

        public IList<IDictionary<IPlayer, int>> AllPayments
        {
            get
            {
                return new List<IDictionary<IPlayer, int>>(this.roundPayments);
            }
        }

        public int GetPlayerCurrentTotal(IPlayer Player)
        {
            return this.StartingPoints + GetPlayerPaymentsHistory(Player).Sum();
        }
    }
}
