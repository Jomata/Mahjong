using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public enum MeldType { Sequence, Triplet, Pair, None };

    public class Meld : ICloneable
    {
        public bool IsKan()
        {
            return this.Tiles != null && this.Tiles.Count == 4 && this.Type == MeldType.Triplet;
        }

        public static List<Meld> FromString(string Tiles,char Separator)
        {
            List<Meld> list = new List<Meld>();

            if (String.IsNullOrEmpty(Tiles)) return list;

            foreach (var meld in Tiles.Split(Separator))
            {
                list.Add(Meld.FromString(meld));
            }
            return list;
        }

        public static Meld FromString(string Tiles)
        {
            Meld m = new Meld(Hand.FromString(Tiles));
            return m;
        }

        public List<Tile> Tiles { get; set; }

        public Meld()
        {
            this.Tiles = new List<Tile>();
        }

        public Meld(List<Tile> Tiles)
        {
            this.Tiles = Tiles;
        }

        public override string ToString()
        {
            StringBuilder SB = new StringBuilder(this.Tiles.Count * 4);
            foreach (var tile in this.Tiles)
            {
                SB.Append(tile.ToString());
            }
            return SB.ToString();
        }

        public MeldType Type
        {
            get
            {
                Tiles.Sort();

                if (Tiles.Count == 0)
                    return MeldType.None;

                if (Tiles.Count == 2 && Tiles[0].IsPairOrTriplet(Tiles[1]))
                    return MeldType.Pair;

                if (Tiles.Count >= 3 && Tiles[0].IsPairOrTriplet(Tiles[1]) && Tiles[1].IsPairOrTriplet(Tiles[2]))
                    return MeldType.Triplet;

                if (Tiles.Count == 3 && Tiles[0].IsNextTile(Tiles[1]) && Tiles[1].IsNextTile(Tiles[2]))
                    return MeldType.Sequence;

                return MeldType.None;
            }
        }

        public object Clone()
        {
            var clone = new Meld(this.Tiles.Clone().ToList());
            return clone;
        }
    }
}
