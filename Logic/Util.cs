using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    //public enum DiscardReaction
    //{
    //    No, Triplet, Sequence, Ron
    //}

    public static class Util
    {
        public static float Next(this Random rng, float Min, float Max)
        {
            var result = (float)rng.NextDouble();
            var range = Max - Min;
            return Min + result * range;
        }

        //public static TrackedTile ToTrackedTile(this Tile tile)
        //{
        //    return new TrackedTile { Tile = tile, IsShantenFiller = false, IsUsed = false, MeldID = -1 };
        //}

        //public static List<TrackedTile> ToTrackedTiles(this IEnumerable<Tile> Tiles)
        //{
        //    //var TrackedTiles = new List<TrackedTile>(Tiles.Count());
        //    //foreach (var tile in Tiles)
        //    //{
        //    //    TrackedTiles.Add(tile);
        //    //}
        //    //return TrackedTiles;

        //    return Tiles.Select(t => t.ToTrackedTile()).ToList();
        //}

        public static int ToNext100(this int Number)
        {
            var hundredth = decimal.Divide(Number, 100);
            return (int)(Math.Ceiling(hundredth) * 100);
        }

        public static string ToMeldsString(this List<Meld> Melds)
        {
            //Melds.Sort();
            return String.Join("|", Melds.Select(x => x.ToString()).ToArray());
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static List<Tile> GetAllTiles(this IEnumerable<Meld> melds)
        {
            return (from meld in melds
                   from tile in meld.Tiles
                   select tile).ToList();
        }

        public static IEnumerable<T> Last<T>(this IEnumerable<T> list, int Count)
        {
            return list.Skip(Math.Max(0, list.Count() - Count));
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        /// <summary>
        /// Removes and returns the first element of the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T PopFirst<T>(this IList<T> list)
        {
            //if (list == null || !list.Any())
            //    return default(T);
            if (list == null || list.Count == 0)
                return default(T);

            T element = list.First();
            list.Remove(element);
            return element;
            //T element = list[0];
            //list.RemoveAt(0);
            //return element;
        }

        public static T GetRandom<T>(this IEnumerable<T> list, Random rng)
        {
            return list.ElementAt(rng.Next(0, list.Count()));
        }

        public static TrackedTile GetNextAvailableTile(this List<TrackedTile> Tiles, TrackedTile CurrentTile)
        {
            int CurrentTileIndex = Tiles.IndexOf(CurrentTile);
            if (CurrentTileIndex + 1 < Tiles.Count)
            {
                var NextTiles = Tiles.GetRange(CurrentTileIndex + 1, Tiles.Count - CurrentTileIndex - 1);
                if(NextTiles.Exists(Tile => !Tile.IsUsed))
                    return NextTiles.First(Tile => !Tile.IsUsed);
            }
            return null;
        }

        public static string ToLine(this IEnumerable<TrackedTile> Hand)
        {
            StringBuilder SB = new StringBuilder(2 * Hand.Count());
            foreach (var Tile in Hand)
            {
                SB.Append(Tile.Tile);
            }
            return SB.ToString();
        }

        public static string ToLine(this IEnumerable<Tile> Hand)
        {
            StringBuilder SB = new StringBuilder(2 * Hand.Count());
            foreach (var Tile in Hand.OrderBy(x => x.Value()))
            {
                SB.Append(Tile);
            }
            return SB.ToString();
        }

        /*public static bool IsWinningHand(this IEnumerable<Tile> Hand)
        {
            return Mahjong.Hand.IsWinningHand(Hand.ToList());
        }*/

        public static void WriteColoredTileToConsole(IEnumerable<Tile> Hand)
        {
            foreach (var Tile in Hand)
            {
                WriteColoredTileToConsole(Tile);
            }
        }
        public static void WriteColoredToConsole(this IEnumerable<Tile> Hand)
        {
            WriteColoredTileToConsole(Hand);
        }

        public static void WriteColoredToConsole(this Tile tile)
        {
            WriteColoredTileToConsole(tile);
        }
        public static void WriteColoredTileToConsole(Tile Tile)
        {
            if (Tile is SuitedTile)
            {
                switch ((Tile as SuitedTile).Suit)
                {
                    case TileSuits.Bamboo:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;

                    case TileSuits.Character:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case TileSuits.Circle:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                }
            }
            else if (Tile is HonorTile)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else if (Tile is DragonTile)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;

                switch ((Tile as DragonTile).Dragon)
                {
                    case TileDragons.Red:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;

                    case TileDragons.White:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case TileDragons.Green:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                }
            }

            Console.Write(Tile);
            Console.ResetColor();
        }

        public static void WriteColoredToConsole(this List<Meld> Melds)
        {
            for (int x = 0; x < Melds.Count; x++)
            {
                if (x > 0) Console.Write("|");

                Melds[x].Tiles.Sort();
                Melds[x].Tiles.WriteColoredToConsole();
            }
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T item)
        {
            return enumerable.Concat(new List<T>() { item });
        }
    }
}
