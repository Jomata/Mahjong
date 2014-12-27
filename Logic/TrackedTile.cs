using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public class TrackedTile : ICloneable
    {
        public Tile Tile { get; set; }
        public bool IsUsed { get; set; }
        public bool IsShantenFiller { get; set; }
        public int MeldID { get; set; }

        public TrackedTile()
        {
            this.MeldID = -1;
        }

        public override string ToString()
        {
            StringBuilder SB = new StringBuilder();

            SB.Append(this.Tile.ToString());
            SB.Append(": ");
            if (this.IsUsed) SB.Append("Used");
            else SB.Append("Available");

            if (this.IsShantenFiller) SB.Append(", placeholder");

            if (MeldID >= 0) SB.AppendFormat(" (Meld {0}", MeldID);

            return SB.ToString();
        }

        public object Clone()
        {
            var clone = new TrackedTile();
            clone.IsShantenFiller = this.IsShantenFiller;
            clone.IsUsed = this.IsUsed;
            clone.MeldID = this.MeldID;
            clone.Tile = this.Tile;
            return clone;
        }
    }
}
