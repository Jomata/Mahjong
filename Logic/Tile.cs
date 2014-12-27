using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public abstract class Tile : IComparable, ICloneable
    {
        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is Tile)
            {
                return this.Value().CompareTo((obj as Tile).Value());

                //if (this.GetType() == obj.GetType())
                //{
                //    if (this is HonorTile)
                //    {
                //        return (this as HonorTile).Honor.CompareTo((obj as HonorTile).Honor);
                //    }
                //    else if (this is DragonTile)
                //    {
                //        return (this as DragonTile).Dragon.CompareTo((obj as DragonTile).Dragon);
                //    }
                //    else if (this is SuitedTile)
                //    {
                //        if ((this as SuitedTile).Suit == (obj as SuitedTile).Suit) return (this as SuitedTile).Rank.CompareTo((obj as SuitedTile).Rank);
                //        else return (this as SuitedTile).Suit.CompareTo((obj as SuitedTile).Suit);
                //    }
                //}
                //else
                //{
                //    //Honors > Dragons > Suits
                //    /** Possible cases:
                //     * this|obj
                //     * Honor|Dragon
                //     * Honor|Suit
                //     * Dragon|Suit
                //     * Suit|Dragon
                //     * Suit|Honor
                //     * Dragon|Honor
                //     */
                //    if (this is HonorTile) return 1; //Honors are always at the top
                //    else if (this is SuitedTile) return -1; //Suits are always at the bottom
                //    else if (obj is HonorTile) return -1; //We're Dragon, we go under Honor
                //    else return 1; //We're Dragon, we go above Suits
                //}
            }
            else throw new ArgumentException("Both objects must inherit from Tile to be compared");
        }

        #endregion

        public abstract bool IsNextTile(Tile Tile);
        public abstract bool IsNextNextTile(Tile Tile);
        public abstract bool IsPairOrTriplet(Tile Tile);

        //public bool IsNextTile(Tile Tile)
        //{
        //    return this.Value() + 1 == Tile.Value();
        //}

        //public bool IsNextNextTile(Tile Tile)
        //{
        //    return this is SuitedTile && this.Value() + 2 == Tile.Value();
        //}

        //public bool IsPairOrTriplet(Tile Tile)
        //{
        //    return this.ToString() == Tile.ToString();
        //}

        public static List<Tile> GetAllTiles()
        {
            var allTiles = new List<Tile>();
            for (int x = 0; x < 4; x++)
            {

                foreach (var Tile in SuitedTile.GetAll()) allTiles.Add(Tile);
                foreach (var Tile in DragonTile.GetAll()) allTiles.Add(Tile);
                foreach (var Tile in HonorTile.GetAll()) allTiles.Add(Tile);
            }
            return allTiles;
        }

        public bool IsHonor()
        {
            return (this is DragonTile || this is HonorTile);
        }

        public bool IsHonorOrTerminal()
        {
            if (this.IsHonor()) return true;
            if (this is SuitedTile && (this as SuitedTile).Rank == 1) return true;
            if (this is SuitedTile && (this as SuitedTile).Rank == 9) return true;

            return false;
        }

        public abstract int Value();
        public abstract string ToLongString();

        public static Tile Unknown()
        {
            return new UnknownTile();
        }

        public static Tile Instantiate(string Suit)
        {
            var Tile = new SuitedTile();
            if (Suit.Length == 2)
            {
                Tile.Rank = int.Parse(Suit.Substring(1, 1));
                switch (Suit.ToUpper()[0])
                {
                    case 'O':
                        Tile.Suit = TileSuits.Circle;
                        break;

                    case 'C':
                        Tile.Suit = TileSuits.Character;
                        break;

                    case 'B':
                        Tile.Suit = TileSuits.Bamboo;
                        break;
                }
            }

            return Tile;
        }

        public static Tile Instantiate(TileHonors Honor)
        {
            return new HonorTile { Honor = Honor };
        }

        public static Tile Instantiate(TileDragons Dragon)
        {
            return new DragonTile { Dragon = Dragon };
        }

        public static Tile Instantiate(Random rng)
        {
            var AllTiles = Mahjong.Sync.Game.GetAllTiles();
            return AllTiles[rng.Next(0,AllTiles.Count)];
        }

        public abstract Tile IsDoraIndicatorOf();

        /// <summary>
        /// http://www.fileformat.info/info/unicode/block/mahjong_tiles/index.htm
        /// </summary>
        /// <returns></returns>
        public string ToUTF8()
        {
            //int charBlockOffset = Convert.ToInt32("f09f8080", 16);
            //int charBlockOffset = 126976;
            //uint charBlockOffset = 4036984960;
            int charBlockOffset = 0;

            if (this is SuitedTile)
            {
                switch ((this as SuitedTile).Suit)
                {
                    case TileSuits.Character:
                        charBlockOffset += 6;
                        break;
                    case TileSuits.Bamboo:
                        charBlockOffset += 15;
                        break;
                    case TileSuits.Circle:
                        charBlockOffset += 24;
                        break;
                    default:
                        charBlockOffset = 43;
                        break;
                }

                if((this as SuitedTile).Rank > 0 && (this as SuitedTile).Rank < 10)
                    charBlockOffset += (this as SuitedTile).Rank;
                else
                    charBlockOffset = 43;
            }
            else if (this is HonorTile)
            {
                charBlockOffset += 0;

                switch ((this as HonorTile).Honor)
                {
                    case TileHonors.East:
                        charBlockOffset += 0;
                        break;
                    case TileHonors.South:
                        charBlockOffset += 1;
                        break;
                    case TileHonors.West:
                        charBlockOffset += 2;
                        break;
                    case TileHonors.North:
                        charBlockOffset += 3;
                        break;
                    default:
                        charBlockOffset = 43;
                        break;
                }
            }
            else if (this is DragonTile)
            {
                charBlockOffset += 4;

                switch ((this as DragonTile).Dragon)
                {
                    case TileDragons.Red:
                        charBlockOffset += 0;
                        break;
                    case TileDragons.Green:
                        charBlockOffset += 1;
                        break;
                    case TileDragons.White:
                        charBlockOffset += 2;
                        break;
                    default:
                        charBlockOffset = 43;
                        break;
                }
            }
            else
            {
                charBlockOffset = 43;
            }

            var byteArray = new byte[] { 0xF0, 0x9F, 0x80, 0x80 };
            byteArray[3] = (byte)(128 + charBlockOffset); //0x80 = 128
            return Encoding.UTF8.GetString(byteArray);
            //return Encoding.UTF32.GetString(BitConverter.GetBytes(charBlockOffset));
            //return Encoding.UTF8.GetString(BitConverter.GetBytes(charBlockOffset));
        }

        public abstract object Clone();
    }

    public class UnknownTile : Tile
    {
        public override int Value()
        {
            return 0;
        }

        public override string ToString()
        {
            return "[??]";
        }

        public override object Clone()
        {
            return Tile.Unknown();
        }

        public override Tile IsDoraIndicatorOf()
        {
            return Tile.Unknown();
        }

        public override string ToLongString()
        {
            return "Unknown";
        }

        public override bool IsNextTile(Tile Tile)
        {
            return false;
        }

        public override bool IsNextNextTile(Tile Tile)
        {
            return false;
        }

        public override bool IsPairOrTriplet(Tile Tile)
        {
            return false;
        }
    }
 

    public enum TileSuits { Character, Circle, Bamboo }
    public class SuitedTile : Tile
    {
        public int Rank { get; set; }
        public TileSuits Suit { get; set; }

        public override string ToString()
        {
            string suitKey = this.Suit == TileSuits.Circle ? "O" : this.Suit.ToString().Substring(0, 1);

            return "[" + suitKey + this.Rank.ToString() + "]";
        }

        public static List<SuitedTile> GetAllFromSuit(TileSuits Suit)
        {
            var result = new List<SuitedTile>(9);
            for(int x=1; x<=9; x++)
                result.Add(new SuitedTile{Rank=x,Suit=Suit});
            return result;
        }

        public static List<SuitedTile> GetAll()
        {
            var result = new List<SuitedTile>(9 * 4);

            foreach (var Suit in (TileSuits[])Enum.GetValues(typeof(TileSuits)))
            {
                result.AddRange(SuitedTile.GetAllFromSuit(Suit));
            }

            return result;
        }

        //public override bool IsNextTile(Tile Tile)
        //{
        //    if (Tile is SuitedTile)
        //    {
        //        if (this.Suit == (Tile as SuitedTile).Suit)
        //        {
        //            return this.Rank == ((Tile as SuitedTile).Rank - 1);
        //        }
        //        else return false;
        //    }
        //    else return false;
        //}

        public override int Value()
        {
            int value = this.Rank;
            switch (this.Suit)
            {
                case TileSuits.Character:
                    value += 0;
                    break;
                case TileSuits.Circle:
                    value += 10;
                    break;
                case TileSuits.Bamboo:
                    value += 20;
                    break;
                default:
                    break;
            }
            return value;
        }

        public override object Clone()
        {
            return Tile.Instantiate(this.ToString().Trim('[', ']'));
        }

        public override Tile IsDoraIndicatorOf()
        {
            var doraRank = this.Rank + 1;
            if (this.Rank == 9) doraRank = 1;

            return new SuitedTile { Suit = this.Suit, Rank = doraRank };
        }

        public override string ToLongString()
        {
            return String.Format("{0} of {1}s", this.Rank, this.Suit);
        }

        public override bool IsNextTile(Tile Tile)
        {
            if (!(Tile is SuitedTile)) return false;
            var suitedTile = Tile as SuitedTile;
            if (this.Suit != suitedTile.Suit) return false;

            return this.Rank + 1 == suitedTile.Rank;
        }

        public override bool IsNextNextTile(Tile Tile)
        {
            if (!(Tile is SuitedTile)) return false;
            var suitedTile = Tile as SuitedTile;
            if (this.Suit != suitedTile.Suit) return false;

            return this.Rank + 2 == suitedTile.Rank;
        }

        public override bool IsPairOrTriplet(Tile Tile)
        {
            if (!(Tile is SuitedTile)) return false;
            var suitedTile = Tile as SuitedTile;

            return this.Rank == suitedTile.Rank && this.Suit == suitedTile.Suit;
        }
    }

    public enum TileHonors { East=1, South, West, North }
    public class HonorTile : Tile
    {
        public TileHonors Honor {get;set;}

        public override string ToString()
        {
            return "[" + this.Honor.ToString().Substring(0, 2) + "]";
        }

        public static List<HonorTile> GetAll()
        {
            var Honors = (TileHonors[])Enum.GetValues(typeof(TileHonors));
            var result = new List<HonorTile>(Honors.Length);

            foreach (var Honor in Honors)
            {
                result.Add(new HonorTile { Honor = Honor });
            }

            return result;
        }

        //public override bool IsNextTile(Tile Tile)
        //{
        //    return false;
        //}

        public override int Value()
        {
            return 30 + 2*(int)this.Honor;
        }

        public override object Clone()
        {
            return Tile.Instantiate(this.Honor);
        }

        public override Tile IsDoraIndicatorOf()
        {
            switch (this.Honor)
            {
                case TileHonors.East:
                    return Tile.Instantiate(TileHonors.South);
                case TileHonors.South:
                    return Tile.Instantiate(TileHonors.West);
                case TileHonors.West:
                    return Tile.Instantiate(TileHonors.North);
                case TileHonors.North:
                    return Tile.Instantiate(TileHonors.East);
                default:
                    return Tile.Unknown();
            }
        }

        public override string ToLongString()
        {
            return string.Format("{0} wind", this.Honor);
        }

        public override bool IsNextTile(Tile Tile)
        {
            return false;
        }

        public override bool IsNextNextTile(Tile Tile)
        {
            return false;
        }

        public override bool IsPairOrTriplet(Tile Tile)
        {
            if (!(Tile is HonorTile)) return false;
            return this.Honor == (Tile as HonorTile).Honor;
        }
    }

    public enum TileDragons { White, Green, Red }
    public class DragonTile : Tile
    {
        public TileDragons Dragon { get; set; }

        public override string ToString()
        {
            return "[" + this.Dragon.ToString().Substring(0, 2) + "]";
        }

        public static List<DragonTile> GetAll()
        {
            var Dragons = (TileDragons[])Enum.GetValues(typeof(TileDragons));
            var result = new List<DragonTile>(Dragons.Length);

            foreach (var Dragon in Dragons)
            {
                result.Add(new DragonTile { Dragon = Dragon });
            }

            return result;
        }

        //public override bool IsNextTile(Tile Tile)
        //{
        //    return false;
        //}

        public override int Value()
        {
            return 40 + 2*(int)this.Dragon;
        }

        public override object Clone()
        {
            return Tile.Instantiate(this.Dragon);
        }

        public override Tile IsDoraIndicatorOf()
        {
            switch (this.Dragon)
            {
                case TileDragons.Red:
                    return Tile.Instantiate(TileDragons.White);
                case TileDragons.Green:
                    return Tile.Instantiate(TileDragons.Red);
                case TileDragons.White:
                    return Tile.Instantiate(TileDragons.Green);
                default:
                    return Tile.Unknown();
            }
        }

        public override string ToLongString()
        {
            return String.Format("{0} dragon", this.Dragon);
        }

        public override bool IsNextTile(Tile Tile)
        {
            return false;
        }

        public override bool IsNextNextTile(Tile Tile)
        {
            return false;
        }

        public override bool IsPairOrTriplet(Tile Tile)
        {
            if (!(Tile is DragonTile)) return false;
            return this.Dragon == (Tile as DragonTile).Dragon;
        }
    }
}
