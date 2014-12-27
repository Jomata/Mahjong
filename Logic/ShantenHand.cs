using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public class ShantenHand
    {
        public List<List<TrackedTile>> Melds { get; set; }

        public ShantenHand()
        {
            this.Melds = new List<List<TrackedTile>>();
        }

        public int CountPlaceholderTiles()
        {
            return this.Melds.Sum(m => m.Count(t => t.IsShantenFiller));
        }

        public List<Tile> GetShantenTiles()
        {
            //var shantenFillers = this.Melds.Select(m => m.Where(t => t.IsShantenFiller));
            var shantenFillers = new List<Tile>();
            foreach (var meld in this.Melds)
            {
                foreach (var TT in meld)
                {
                    if (TT.IsShantenFiller)
                        shantenFillers.Add(TT.Tile);
                    else if (meld.Count == 2 && meld.Any(x => x.IsShantenFiller))
                        shantenFillers.Add(TT.Tile);
                }
            }
            shantenFillers.Sort();
            return shantenFillers;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ShantenHand)) return false;

            var otherHand = obj as ShantenHand;

            if (this.Melds.Count != otherHand.Melds.Count) return false;

            return this.GetShantenTiles().All(x => otherHand.GetShantenTiles().Contains(x));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var meld in this.Melds)
            {
                if(i > 0) sb.Append(",");
                foreach (var TTile in meld)
                {
                    if (TTile.IsShantenFiller) sb.Append("[??]");
                    else sb.Append(TTile.Tile.ToString());
                }
                i++;
            }

            return sb.ToString();
        }
    }
}
