using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public class Hand
    {
        public static List<Tile> GetRandomHand()
        {
            var FullHand = Mahjong.Sync.Game.GetAllTiles();
            return FullHand.OrderBy(x => Guid.NewGuid()).Take(14).ToList();
        }

        public static List<Tile> FromString(string Tiles)
        {
            Tiles = Tiles.Replace(" ", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty);
            if (Tiles.Length % 2 != 0) throw new ArgumentException("Must have a string with two characters per tile");

            var Hand = new List<Tile>(Tiles.Length / 2);

            for (int x = 0; x + 1 < Tiles.Length; x += 2)
            {
                var code = Tiles.Substring(x, 2);
                //Console.Write("Evaluating {0}", code);

                Tile Tile;
                switch (code.ToUpper())
                {
                    case "RE": Tile = Tile.Instantiate(TileDragons.Red); break;
                    case "GR": Tile = Tile.Instantiate(TileDragons.Green); break;
                    case "WH": Tile = Tile.Instantiate(TileDragons.White); break;

                    case "NO": Tile = Tile.Instantiate(TileHonors.North); break;
                    case "SO": Tile = Tile.Instantiate(TileHonors.South); break;
                    case "EA": Tile = Tile.Instantiate(TileHonors.East); break;
                    case "WE": Tile = Tile.Instantiate(TileHonors.West); break;

                    default: Tile = Tile.Instantiate(code); break;
                }

                //Console.Write(" -> ");
                //Console.WriteLine(Tile);
                Hand.Add(Tile);
            }

            return Hand;
        }

        public static List<Tile> GetDiscardsForTenpai(Type ScorerType, List<Tile> ClosedTiles, List<Meld> ExposedMelds, IGame Game, IPlayer Player)
        {
            var discards = new List<Tile>();
            var newTilesToCheck = new List<Tile>();

            var shantenInfo = Hand.GetShantenTiles(ClosedTiles, ExposedMelds);

            if (shantenInfo.Key > 0) return discards;
            
            //We only need to check:
            // * Tiles with the same value of the ones in my hand
            newTilesToCheck.AddRange(ClosedTiles.Clone());
            // * If suited tile, tiles worth one less or one more
            foreach (SuitedTile closedTile in ClosedTiles.Where(x => x is SuitedTile))
            {
                if (closedTile.Rank < 9) newTilesToCheck.Add(new SuitedTile { Suit = closedTile.Suit, Rank = closedTile.Rank + 1 });
                if (closedTile.Rank > 1) newTilesToCheck.Add(new SuitedTile { Suit = closedTile.Suit, Rank = closedTile.Rank - 1 });
            }
            newTilesToCheck = newTilesToCheck.DistinctBy(x => x.Value()).ToList();
            
            //var myTilesToCheck = ClosedTiles.DistinctBy(x => x.Value()).ToList();
            var myTilesToCheck = shantenInfo.Value;

            foreach (var NewTile in newTilesToCheck)
            {
                foreach (var MyTile in myTilesToCheck)
                {
                    //We check if removing that tile and adding another one is a valid strat
                    //if (Hand.IsValidHand(ScorerType, ClosedTiles.Where(x => x != MyTile).Concat(NewTile).ToList(), ExposedMelds, Game, Player))
                    if (Hand.GetAllHands(ClosedTiles.Where(x => x != MyTile).Concat(NewTile).ToList(), ExposedMelds, Game, Player).Count > 0)
                    {
                        discards.Add(MyTile);
                    }
                }
            }
            return discards.DistinctBy(t => t.Value()).ToList();
        }

        //public static bool IsTenpai(Type ScorerType, List<Tile> ClosedTiles, List<Meld> ExposedMelds, IGame Game, IPlayer Player)
        //{
        //    var allTiles = Tile.GetAllTiles();
        //    foreach (var tile in allTiles)
        //    {
        //        if (Hand.IsValidHand(ScorerType, ClosedTiles.Concat(tile).ToList(), ExposedMelds, Game, Player))
        //            return true;
        //    }
        //    return false;
        //}

        public static List<Meld> GetBestHand(Type ScorerType, List<Tile> ClosedTiles, List<Meld> ExposedMelds, IGame Game, IPlayer Player)
        {
            if (!ScorerType.IsSubclassOf(typeof(Scorer))) throw new ArgumentException("Invalid scorer");
            var ScorerConstructor = ScorerType.GetConstructor(new Type[] { ExposedMelds.GetType(), ExposedMelds.GetType(), Game.GetType(), Player.GetType() });

            var AllHands = GetAllHands(ClosedTiles, ExposedMelds, Game, Player);

            List<Meld> bestHand = null;
            var bestScore = 0;
            foreach (var Hand in AllHands)
            {
                var handScorer = ScorerConstructor.Invoke(new object[] {Hand, ExposedMelds, Game, Player}) as Scorer;
                var newScore = handScorer.GetBasicValue();
                if (newScore > bestScore)
                {
                    bestHand = Hand;
                    bestScore = newScore;
                }
            }
            return bestHand; // GetAllHands(ClosedTiles, ExposedMelds, Game, Player).FirstOrDefault();
        }

        public static List<List<Meld>> GetAllHands(List<Tile> ClosedTiles, List<Meld> ExposedMelds, IGame Game, IPlayer Player)
        {
            var ValidHands = new List<List<Meld>>();
            if (ClosedTiles.Count + ExposedMelds.GetAllTiles().Count < 14) return ValidHands;
            
            /* Getting all hands possible */
            int Triplets, Pairs, Sequences;
            var WinningTiles = Hand.PrepareBacktracking(ClosedTiles, ExposedMelds, out Sequences, out Triplets, out Pairs);

            Mahjong.Hand.GetAllHands_Backtracking(WinningTiles, 0, 0, Triplets, Sequences, ValidHands);
            //remove duplicates
            return ValidHands.DistinctBy(x => x.ToMeldsString()).ToList();
        }

        ///// <summary>
        ///// http://stackoverflow.com/questions/4154960/algorithm-to-find-streets-and-same-kind-in-a-hand/4155177#4155177
        ///// </summary>
        ///// <param name="ClosedTiles"></param>
        ///// <returns></returns>
        //public static bool IsValidHand(List<Tile> ClosedTiles, List<Meld> ExposedMelds, IGame Game, IPlayer Player)
        //{
        //    if (ClosedTiles.Count() + ExposedMelds.GetAllTiles().Count < 14) return false;
        //    var ShantenHands = Hand.GetAllHands(ClosedTiles, ExposedMelds, Game, Player);
            
        //    // WinningTiles has them grouped by meldId
        //    //return IsValid;
        //    return ShantenHands.Count > 0;
        //}

        //Idea: Funcion recursiva como la de GetHands
        //En los parametros, añadir un diccionario de <string,int> con la representacion de la mano + los tiles que tendria que cambiar con esa
        //Probablemente reemplazando a validhands
        //Cada vez que entro a la recursiva añado una mano nueva, con los que ya tengo marcados como winning tiles y cuandos necesitaria
        //?? Como le hago con los XX{X} o los XY{Z} ?
        //A lo mejor debajo de donde termina de poner las manos que ya estan completas?

        private static List<TrackedTile> PrepareBacktracking(IList<Tile> ClosedTiles, IList<Meld> ExposedMelds, out int Sequences, out int Triplets, out int Pairs)
        {
            List<TrackedTile> WinningTiles = new List<TrackedTile>(ClosedTiles.Count);
            foreach (var Tile in ClosedTiles.OrderBy(t => t.Value())) WinningTiles.Add(new TrackedTile { Tile = Tile, IsUsed = false, MeldID = -1 });

            Triplets = 0;
            Sequences = 0;
            Pairs = 0;

            foreach (var Meld in ExposedMelds)
            {
                switch (Meld.Type)
                {
                    case MeldType.Sequence:
                        Sequences++;
                        break;
                    case MeldType.Triplet:
                        Triplets++;
                        break;
                    case MeldType.Pair:
                        //Wtf why do you have a pair on your open melds?
                        throw new InvalidOperationException("Can't have a pair in your exposed melds");
                    //case MeldType.None:
                    //    return -1;
                }

                foreach (var Tile in Meld.Tiles)
                {
                    WinningTiles.Add(new TrackedTile { Tile = Tile, IsUsed = true, MeldID = -1 });
                }
            }

            return WinningTiles;
        }

        public static KeyValuePair<int,List<Tile>> GetShantenTiles(IList<Tile> ClosedTiles, IList<Meld> ExposedMelds)
        {
            if (ClosedTiles.Count + ExposedMelds.GetAllTiles().Count < 14) return new KeyValuePair<int,List<Tile>>(-1,null);

            int Triplets, Pairs, Sequences;
            var WinningTiles = Hand.PrepareBacktracking(ClosedTiles, ExposedMelds, out Sequences, out Triplets, out Pairs);

            var ShantenTiles = new Dictionary<int, List<List<Tile>>>();
            Mahjong.Hand.GetShanten_Backtracing_v3(WinningTiles, 0, 0, Triplets, Sequences, ShantenTiles);
            var lowestShanten = ShantenTiles.Keys.Min();
            var lowestShantenTiles = (from listList in ShantenTiles[lowestShanten]
                                      from tile in listList
                                      select tile).ToList();
            var uniqueLowestShantenTiles = lowestShantenTiles.Distinct().ToList();

            return new KeyValuePair<int, List<Tile>>(lowestShanten-1,uniqueLowestShantenTiles); 
            //remember, I'm getting the tiles away from win, and shante = tiles away from tenpai, so -1
        }

        public static int GetShantenNumber(IList<Tile> ClosedTiles, IList<Meld> ExposedMelds)
        {
            if (ClosedTiles.Count + ExposedMelds.GetAllTiles().Count < 14) return -1;

            int Triplets, Pairs, Sequences;
            var WinningTiles = Hand.PrepareBacktracking(ClosedTiles,ExposedMelds,out Sequences,out Triplets,out Pairs);

            var ShantenTiles = new Dictionary<int, List<List<Tile>>>();
            Mahjong.Hand.GetShanten_Backtracing_v3(WinningTiles, 0, 0, Triplets, Sequences, ShantenTiles);
            var replacementsNeeded = ShantenTiles.Keys.Min();
            
            //var ShantenHands = new List<ShantenHand>();
            //var ShantenTiles = new List<Tile>();
            //int counter = 0;
            //Mahjong.Hand.GetShanten_Backtracing_v2(WinningTiles, 0, 0, Triplets, Sequences, ShantenHands, ref counter);
            //var replacementsNeeded = ShantenHands.Min(x => x.CountPlaceholderTiles());
            //#if DEBUG
            //var debugHelper = ShantenHands.Where(x => x.CountPlaceholderTiles() == replacementsNeeded).ToList();
            //#endif

            //var replacementsNeeded = 14;
            //var ShantenHands = new List<List<Meld>>();
            //Mahjong.Hand.GetShanten_Backtracing(WinningTiles, 0, 0, Triplets, Sequences, ShantenHands, ref replacementsNeeded);
            //var bestHands = ShantenHands.Where(x => replacementsNeeded == x.Count(y => y.Type == MeldType.None));

            //Count how many "unknown" tiles are on each hand, return minimum
            //var unknownMeldsCount = ShantenHands.Select(x => x.Count(y => y.Type == MeldType.None));
            //var fewestReplacements = unknownMeldsCount.Min();
            //var bestHands = ShantenHands.Where(x => fewestReplacements == x.Count(y => y.Type == MeldType.None));
            //#if DEBUG
            //var debugHelper = bestHands.ToList();
            //#endif

            return replacementsNeeded - 1;
            //return unknownMeldsCount.Min() - 1; //Count to tenpai, which needs 1 by itself
            //return ShantensDict.Values.Min();
        }
        
        private static void GetShanten_Backtracing_v3(List<TrackedTile> HandTiles, int Depth, int Pairs, int Triplets, int Sequences, IDictionary<int,List<List<Tile>>> ShantenTiles)
        {
            if ((Pairs == 7) || (Pairs == 1 && Triplets + Sequences == 4))
            {
                //var newHand = new List<Meld>();
                //var melds = HandTiles.Where(x => x.MeldID >= 0).GroupBy(x => x.MeldID);
                //foreach (var meld in melds)
                //{
                //    newHand.Add(new Meld(meld.Select(x => x.Tile).ToList()));
                //}

                //ValidHands.Add(newHand);

                return;
            }


            if (Triplets + Sequences < 4 && Pairs < 2) //If we have 2+ pairs, our only option is a 7 pairs, so we only check for sequences when we have 0 or 1 pairs
            {
                #region Sequences
                List<string> alreadyRejected = new List<string>();

                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    {
                        if (!NonWinningTile1.Tile.IsNextTile(NonWinningTile2.Tile)) continue;

                        var NonWinningTile3 = NonWinningTile2;
                        while (null != (NonWinningTile3 = HandTiles.GetNextAvailableTile(NonWinningTile3)))
                        {
                            if (!NonWinningTile2.Tile.IsNextTile(NonWinningTile3.Tile)) continue;

                            var stringified = String.Format("{0}{1}{2}", NonWinningTile1.Tile, NonWinningTile2.Tile, NonWinningTile3.Tile);

                            if (!alreadyRejected.Contains(stringified))
                            {

                                NonWinningTile1.IsUsed = true;
                                NonWinningTile2.IsUsed = true;
                                NonWinningTile3.IsUsed = true;

                                NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                                NonWinningTile2.MeldID = NonWinningTile1.MeldID;
                                NonWinningTile3.MeldID = NonWinningTile1.MeldID;

                                Hand.GetShanten_Backtracing_v3(HandTiles, Depth + 1, Pairs, Triplets, ++Sequences, ShantenTiles);

                                alreadyRejected.Add(stringified);
                                Sequences--;

                                NonWinningTile1.IsUsed = false;
                                NonWinningTile2.IsUsed = false;
                                NonWinningTile3.IsUsed = false;

                                NonWinningTile1.MeldID = -1;
                                NonWinningTile2.MeldID = -1;
                                NonWinningTile3.MeldID = -1;

                            }
                        }
                    }
                }
                #endregion
            }

            if (Triplets + Sequences < 4 && Pairs < 2) //If we have 2+ pairs, our only option is a 7 pairs, so we only check for sequences when we have 0 or 1 pairs
            {
                #region Triplets
                List<string> alreadyRejected = new List<string>();
                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    {
                        if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile2.Tile)) continue;

                        var NonWinningTile3 = NonWinningTile2;
                        while (null != (NonWinningTile3 = HandTiles.GetNextAvailableTile(NonWinningTile3)))
                        {
                            if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile3.Tile)) continue;
                            var stringified = String.Format("{0}{1}{2}", NonWinningTile1.Tile, NonWinningTile2.Tile, NonWinningTile3.Tile);

                            if (!alreadyRejected.Contains(stringified))
                            {
                                NonWinningTile1.IsUsed = true;
                                NonWinningTile2.IsUsed = true;
                                NonWinningTile3.IsUsed = true;

                                NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                                NonWinningTile2.MeldID = NonWinningTile1.MeldID;
                                NonWinningTile3.MeldID = NonWinningTile1.MeldID;

                                Hand.GetShanten_Backtracing_v3(HandTiles, Depth + 1, Pairs, ++Triplets, Sequences, ShantenTiles);

                                alreadyRejected.Add(stringified);
                                Triplets--;

                                NonWinningTile1.IsUsed = false;
                                NonWinningTile2.IsUsed = false;
                                NonWinningTile3.IsUsed = false;

                                NonWinningTile1.MeldID = -1;
                                NonWinningTile2.MeldID = -1;
                                NonWinningTile3.MeldID = -1;

                            }
                        }
                    }
                }
                #endregion
            }

            if ((Triplets + Sequences > 0 && Pairs < 1) //If we have at least one triplet/sequence, we only need 1 pair
                || Triplets + Sequences == 0) //If we don't have any sequences, we can go for 7 pairs
            {
                #region Pairs
                List<string> rejectedPairs = new List<string>();
                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    {
                        if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile2.Tile)) continue;
                        var stringified = NonWinningTile1.Tile.ToString() + NonWinningTile2.Tile.ToString();

                        if (!rejectedPairs.Contains(stringified))
                        {
                            NonWinningTile1.IsUsed = true;
                            NonWinningTile2.IsUsed = true;

                            NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                            NonWinningTile2.MeldID = NonWinningTile1.MeldID;

                            Hand.GetShanten_Backtracing_v3(HandTiles, Depth + 1, ++Pairs, Triplets, Sequences, ShantenTiles);

                            rejectedPairs.Add(stringified);
                            Pairs--;

                            NonWinningTile1.IsUsed = false;
                            NonWinningTile2.IsUsed = false;

                            NonWinningTile1.MeldID = -1;
                            NonWinningTile2.MeldID = -1;

                        }
                    }
                }
                #endregion
            }

            Hand.GetShanten_Backtracing_v3_CalcShantenWithRemains(HandTiles,Pairs,Triplets,Sequences,ShantenTiles,true,false);
            Hand.GetShanten_Backtracing_v3_CalcShantenWithRemains(HandTiles, Pairs, Triplets, Sequences, ShantenTiles, false,false);
            Hand.GetShanten_Backtracing_v3_CalcShantenWithRemains(HandTiles, Pairs, Triplets, Sequences, ShantenTiles, true,true);
            Hand.GetShanten_Backtracing_v3_CalcShantenWithRemains(HandTiles, Pairs, Triplets, Sequences, ShantenTiles, false,true);

            return;
        }

        private static void GetShanten_Backtracing_v3_CalcShantenWithRemains(List<TrackedTile> HandTiles, int Pairs, int Triplets, int Sequences, IDictionary<int, List<List<Tile>>> ShantenTiles, bool TripletsFirst, bool backwards)
        {
            /* Here we can check what to do with the leftover tiles */

            //do MATH here, with the remaining unused tiles, if they have not been checked (keep a recursive array for cache), check how many tiles you'd need
            //and if it's lower than what we have so far, store that + the tiles, plus clearing the current array of values higher than current

            var shanten = 0;
            var unusedHandTiles = HandTiles.Where(x => !x.IsUsed).ToList().Clone();
            var nUnusedTiles = unusedHandTiles.Count;
            foreach (var unusedTile in unusedHandTiles)
                unusedTile.IsShantenFiller = true;   //HAVE TO TAKE CARE OF THIS, I PROBABLY SHOULD CLONE THE ARRAY

            //if we have utterly nothing, our best bet is a 7 pairs, using anything
            if (false && nUnusedTiles == 14)
            {
                shanten = 7;
            }
            else
            {
                if (TripletsFirst)
                {
                    #region Triplets
                    //For the remaining ones, we check if we are only missing one tile or if we need two
                    var possibleTriplets = unusedHandTiles.GroupBy(tt => tt.Tile.Value()).ToList();
                    //foreach (var possibleTriplet in possibleTriplets)
                    //for(int i = 0; i < possibleTriplets.Count; i++)
                    for (int i = backwards ? possibleTriplets.Count - 1 : 0;
                        backwards ? i >= 0 : i < possibleTriplets.Count;
                        i += backwards ? -1 : +1)
                    {
                        var possibleTriplet = possibleTriplets[i];

                        if (Triplets + Sequences >= 4 || Pairs > 1) break;
                        if (possibleTriplet.Count() == 2)
                        {
                            foreach (var tt in possibleTriplet)
                            {
                                tt.IsUsed = true;
                                tt.IsShantenFiller = false;
                            }
                            nUnusedTiles -= 3; //minus 3, already accounting for the one I'm using
                            shanten++;
                            Triplets++;
                        }
                    }
                    #endregion
                }
                //Now we check for possible sequences
                //foreach (var unusedTileA in unusedHandTiles)
                for (int i = backwards ? unusedHandTiles.Count - 1 : 0;
                        backwards ? i >= 0 : i < unusedHandTiles.Count;
                        i += backwards ? -1 : +1)
                {
                    #region Sequences
                    var unusedTileA = unusedHandTiles[i];

                    if (Triplets + Sequences >= 4 || Pairs > 1) break;
                    if (unusedTileA.IsUsed) continue;

                    foreach (var unusedTileB in unusedHandTiles)
                    {
                        //if (Triplets + Sequences >= 4) break;
                        if (unusedTileB.IsUsed) continue;

                        if (unusedTileA.Tile.IsNextTile(unusedTileB.Tile) || unusedTileA.Tile.IsNextNextTile(unusedTileB.Tile))
                        {
                            unusedTileA.IsUsed = true;
                            unusedTileA.IsShantenFiller = false;
                            unusedTileB.IsUsed = true;
                            unusedTileB.IsShantenFiller = false;
                            nUnusedTiles -= 3; //minus 3, already accounting for the one I'm using
                            shanten++;
                            Sequences++;
                            break;
                        }
                    }
                    #endregion
                }

                if (!TripletsFirst)
                {
                    #region Triplets
                    //For the remaining ones, we check if we are only missing one tile or if we need two
                    var possibleTriplets = unusedHandTiles.GroupBy(tt => tt.Tile.Value()).ToList();
                    //foreach (var possibleTriplet in possibleTriplets)
                    for (int i = backwards ? possibleTriplets.Count - 1 : 0;
                        backwards ? i >= 0 : i < possibleTriplets.Count;
                        i += backwards ? -1 : +1)
                    {
                        var possibleTriplet = possibleTriplets[i];

                        if (Triplets + Sequences >= 4 || Pairs > 1) break;
                        if (possibleTriplet.Count() == 2)
                        {
                            foreach (var tt in possibleTriplet)
                            {
                                tt.IsUsed = true;
                                tt.IsShantenFiller = false;
                            }
                            nUnusedTiles -= 3; //minus 3, already accounting for the one I'm using
                            shanten++;
                            Triplets++;
                        }
                    }
                    #endregion
                }

                //We don't have the pair, we can use any of the remaining tiles for the pair and only need to wait on 1
                //Moved down below
                //And moved back up here
                if (Pairs == 0)
                {
                    nUnusedTiles -= 2;
                    shanten++;
                }

                //All the remaining tiles are single tiles. 
                //We use them for either 7 pairs as solos if we don't have any sequences/triplets (shanten+1 each)
                if (unusedHandTiles.Where(x => !x.IsUsed).Count() == 14) //we got nuthin
                {
                    shanten = 7;
                }
                //else {
                else 
                    if (Triplets + Sequences == 0)
                    {
                        //shanten += nUnusedTiles / 2;

                        var pairsNeeded = nUnusedTiles / 2;
                        var freeTiles = 0;
                        var existingPairTiles = HandTiles.Where(x => x.IsUsed).Select(tt => tt.Tile);
                        foreach (var potentialPair in unusedHandTiles.Where(x => !x.IsUsed).Select(tt => tt.Tile))
                        {
                            //if (existingPairTiles.Any(existing => existing.IsPairOrTriplet(potentialPair)))
                            //    shanten++;
                            if (!existingPairTiles.Any(existing => existing.IsPairOrTriplet(potentialPair)))
                                freeTiles++;
                        }
                        //every free tile is good for a pair, so the 2nd free tile does not affect us, hence, (freeTiles+1)/2
                        shanten += pairsNeeded * 2 - (freeTiles + 1) / 2;

                        //What we can possibly do, is mark them as placeholders
                    }
                    //Or for triplets/sequences if we only have one pair (shanten+2 each)
                    else
                    {
                        shanten += nUnusedTiles * 2 / 3;
                    }
                //}
            }

            if (ShantenTiles.Count == 0 || shanten <= ShantenTiles.Keys.Min())
            {
                //shanten = unusedHandTiles.Count(x => !x.IsUsed);
                if (!ShantenTiles.ContainsKey(shanten)) ShantenTiles.Add(shanten, new List<List<Tile>>());
                ShantenTiles[shanten].Add(unusedHandTiles.Where(x => !x.IsUsed).Select(tt => tt.Tile).ToList());
            }
        }

        public static void GetShanten_Backtracing_v2(List<TrackedTile> HandTiles, int Depth, int Pairs, int Triplets, int Sequences, List<ShantenHand> ShantenHands, ref int totalHandsCounter)
        {
            if ((Pairs == 7) || (Pairs == 1 && Triplets + Sequences == 4))
            {
                totalHandsCounter++;

                var newHand = new List<Meld>();
                var melds = HandTiles.Where(x => x.MeldID >= 0).GroupBy(x => x.MeldID);
                var newShantenHand = new ShantenHand();
                foreach (var meld in melds)
                {
                    newHand.Add(new Meld(meld.Select(x => x.Tile).ToList()));
                    newShantenHand.Melds.Add(meld.ToList().Clone().ToList());
                }

                //if we need more tiles than what we already do, we don't care
                //var newHandReplacementsNeeded = newHand.Count(x => x.Type == MeldType.None);
                var newHandReplacementsNeeded = newShantenHand.CountPlaceholderTiles();
                if (ShantenHands.Count > 0 && newHandReplacementsNeeded >= ShantenHands.Min(x => x.CountPlaceholderTiles())) //I NEED TO PUT BACK THE >= HERE
                {
                    return;
                }

                //double checking that we don't have a similar shanten hand already
                if (ShantenHands.Any(x => x.Equals(newShantenHand)))
                    return;

                //double checking that if we have many completed pairs, we do not try to wait on them again as incomplete pairs
                if (newHand.Count(x => x.Type == MeldType.Pair) > 1)
                {
                    var pairedTiles = newHand.Where(x => x.Type == MeldType.Pair).GetAllTiles();
                    //foreach (var shantenPair in newHand.Where(x => x.Type == MeldType.None && x.Tiles.Count == 2))
                    //{
                    //    if (pairedTiles.Any(pairedTile => pairedTile.IsPairOrTriplet(shantenPair.Tiles[0])) &&
                    //        pairedTiles.Any(pairedTile => pairedTile.IsPairOrTriplet(shantenPair.Tiles[1])))
                    //        return;
                    //}
                    
                    //foreach (var pairedTile in pairedTiles.DistinctBy(x => x.Value()))
                    //{
                    //    if (HandTiles.Any(TT => TT.IsShantenFiller && TT.Tile.Value() == pairedTile.Value()))
                    //        return;
                    //}
                    foreach (var pairedTile in pairedTiles.DistinctBy(x => x.Value()))
                    {
                        foreach (var shantenMeld in newShantenHand.Melds.Where(meld => meld.Count == 2 && meld.Any(tt => tt.IsShantenFiller) /*&& meld.Any(tt => tt.Tile.Value() == pairedTile.Value())*/))
                        {
                            //If the already paired tile is NOT marked as placeholder, that means it's trying to use it as a pair *again*, thus nonvalid
                            if (shantenMeld.Any(TTile => TTile.Tile.IsPairOrTriplet(pairedTile) && !TTile.IsShantenFiller))
                                return;
                        }
                    }
                }

                //If we got this far, it means we added it and it has less replacements needed than what we currently do have
                ShantenHands.Add(newShantenHand);
                return;
            }


            if (Triplets + Sequences < 4 && Pairs < 2) //If we have 2+ pairs, our only option is a 7 pairs, so we only check for sequences when we have 0 or 1 pairs
            {
                #region Sequences
                List<string> checkedSequences = new List<string>();

                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    {
                        if (!NonWinningTile1.Tile.IsNextTile(NonWinningTile2.Tile)) continue;

                        var NonWinningTile3 = NonWinningTile2;
                        while (null != (NonWinningTile3 = HandTiles.GetNextAvailableTile(NonWinningTile3)))
                        {
                            //var stringified = String.Format("{0}{1}{2}", NonWinningTile1.Tile, NonWinningTile2.Tile, NonWinningTile3.Tile);
                            var stringified = String.Format("{0}{1}", NonWinningTile1.Tile, NonWinningTile2.Tile);

                            if (!checkedSequences.Contains(stringified))
                            {
                                //We always mark it as winning, even if it isn't- So we can count how many invalid melds we have. Every invalid meld = +1 shanten
                                NonWinningTile1.IsUsed = true;
                                NonWinningTile2.IsUsed = true;
                                NonWinningTile3.IsUsed = true;

                                NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                                NonWinningTile2.MeldID = NonWinningTile1.MeldID;
                                NonWinningTile3.MeldID = NonWinningTile1.MeldID;

                                if (!NonWinningTile2.Tile.IsNextTile(NonWinningTile3.Tile)) NonWinningTile3.IsShantenFiller = true;
                                Hand.GetShanten_Backtracing_v2(HandTiles, Depth + 1, Pairs, Triplets, ++Sequences, ShantenHands, ref totalHandsCounter);

                                checkedSequences.Add(stringified);
                                Sequences--;

                                NonWinningTile3.IsShantenFiller = false;
                                NonWinningTile1.IsUsed = false;
                                NonWinningTile2.IsUsed = false;
                                NonWinningTile3.IsUsed = false;

                                NonWinningTile1.MeldID = -1;
                                NonWinningTile2.MeldID = -1;
                                NonWinningTile3.MeldID = -1;

                            }
                        }
                    }
                }
                #endregion
            }

            //If we have 2+ pairs, our only option is a 7 pairs, so we only check for triplets when we have 0 or 1 pairs
            if (Triplets + Sequences < 4 && Pairs < 2)
            {
                #region Triplets
                List<string> checkedTriplets = new List<string>();
                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    {
                        if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile2.Tile)) continue;

                        var NonWinningTile3 = NonWinningTile2;
                        while (null != (NonWinningTile3 = HandTiles.GetNextAvailableTile(NonWinningTile3)))
                        {
                            //if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile3.Tile)) continue;
                            //We always mark it as winning, even if it isn't- So we can count how many invalid melds we have. Every invalid meld = +1 shanten

                            //var stringified = String.Format("{0}{1}{2}", NonWinningTile1.Tile, NonWinningTile2.Tile, NonWinningTile3.Tile);
                            var stringified = String.Format("{0}{1}", NonWinningTile1.Tile, NonWinningTile2.Tile);

                            if (!checkedTriplets.Contains(stringified))
                            {
                                NonWinningTile1.IsUsed = true;
                                NonWinningTile2.IsUsed = true;
                                NonWinningTile3.IsUsed = true;

                                NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                                NonWinningTile2.MeldID = NonWinningTile1.MeldID;
                                NonWinningTile3.MeldID = NonWinningTile1.MeldID;

                                //var newHandShantenTiles = ShantenTiles.Clone().ToList();
                                if (!NonWinningTile2.Tile.IsPairOrTriplet(NonWinningTile3.Tile)) NonWinningTile3.IsShantenFiller = true;
                                Hand.GetShanten_Backtracing_v2(HandTiles, Depth + 1, Pairs, ++Triplets, Sequences, ShantenHands, ref totalHandsCounter);
                                //Hand.GetShanten_Backtracing(HandTiles, Depth + 1, Pairs, ++Triplets, Sequences, ShantenHands, ref CurrentReplacementsNeeded);

                                checkedTriplets.Add(stringified);
                                Triplets--;

                                NonWinningTile3.IsShantenFiller = false;
                                NonWinningTile1.IsUsed = false;
                                NonWinningTile2.IsUsed = false;
                                NonWinningTile3.IsUsed = false;

                                NonWinningTile1.MeldID = -1;
                                NonWinningTile2.MeldID = -1;
                                NonWinningTile3.MeldID = -1;

                            }
                        }
                    }
                }
                #endregion
            }

            if ((Triplets + Sequences > 0 && Pairs < 1) //If we have at least one triplet/sequence, we only need 1 pair
                || Triplets + Sequences == 0) //If we don't have any sequences, we can go for 7 pairs
            {
                #region Pairs
                List<string> checkedPairs = new List<string>();
                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    {
                        var stringifiedAB = NonWinningTile1.Tile.ToString() + NonWinningTile2.Tile.ToString();
                        var stringifiedBA = NonWinningTile1.Tile.ToString() + NonWinningTile2.Tile.ToString();

                        if (!checkedPairs.Contains(stringifiedAB))
                        {
                            NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                            NonWinningTile2.MeldID = NonWinningTile1.MeldID;

                            NonWinningTile1.IsUsed = true;
                            NonWinningTile2.IsUsed = true;

                            if (NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile2.Tile))
                            {
                                Hand.GetShanten_Backtracing_v2(HandTiles, Depth + 1, Pairs + 1, Triplets, Sequences, ShantenHands, ref totalHandsCounter);
                            }
                            else
                            {
                                NonWinningTile1.IsShantenFiller = false;
                                NonWinningTile2.IsShantenFiller = true;
                                Hand.GetShanten_Backtracing_v2(HandTiles, Depth + 1, Pairs + 1, Triplets, Sequences, ShantenHands, ref totalHandsCounter);

                                NonWinningTile1.IsShantenFiller = true;
                                NonWinningTile2.IsShantenFiller = false;
                                Hand.GetShanten_Backtracing_v2(HandTiles, Depth + 1, Pairs + 1, Triplets, Sequences, ShantenHands, ref totalHandsCounter);
                            }

                            checkedPairs.Add(stringifiedAB);
                            checkedPairs.Add(stringifiedBA);

                            NonWinningTile1.IsUsed = false;
                            NonWinningTile2.IsUsed = false;

                            NonWinningTile1.MeldID = -1;
                            NonWinningTile2.MeldID = -1;
                        }
                    }
                }
                #endregion
            }

            return;
        }


        public static void GetShanten_Backtracing_v1(List<TrackedTile> HandTiles, int Depth, int Pairs, int Triplets, int Sequences, List<List<Meld>> ShantenHands, ref int CurrentReplacementsNeeded)
        {
            if ((Pairs == 7) || (Pairs == 1 && Triplets + Sequences == 4))
            {
                var newHand = new List<Meld>();
                var melds = HandTiles.Where(x => x.MeldID >= 0).GroupBy(x => x.MeldID);
                foreach (var meld in melds)
                {
                    newHand.Add(new Meld(meld.Select(x => x.Tile).ToList()));
                }

                //if we need more tiles than what we already do, we don't care
                var newHandReplacementsNeeded = newHand.Count(x => x.Type == MeldType.None);
                if (newHandReplacementsNeeded >= CurrentReplacementsNeeded)
                {
                    return;
                }

                //double checking that if we have many completed pairs, we do not try to wait on them again as incomplete pairs
                if (newHand.Count(x => x.Type == MeldType.Pair) > 1)
                {
                    var pairedTiles = newHand.Where(x => x.Type == MeldType.Pair).GetAllTiles();
                    foreach (var shantenPair in newHand.Where(x => x.Type == MeldType.None && x.Tiles.Count == 2))
                    {
                        if (pairedTiles.Any(pairedTile => pairedTile.IsPairOrTriplet(shantenPair.Tiles[0])) && 
                            pairedTiles.Any(pairedTile => pairedTile.IsPairOrTriplet(shantenPair.Tiles[1]))) 
                            return;
                    }
                }

                //If we got this far, it means we added it and it has less replacements needed than what we currently do have
                CurrentReplacementsNeeded = newHandReplacementsNeeded;
                ShantenHands.Add(newHand);
                return;
            }


            if (Triplets + Sequences < 4 && Pairs < 2) //If we have 2+ pairs, our only option is a 7 pairs, so we only check for sequences when we have 0 or 1 pairs
            {
                #region Sequences
                List<string> checkedSequences = new List<string>();

                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    {
                        if (!NonWinningTile1.Tile.IsNextTile(NonWinningTile2.Tile)) continue;

                        var NonWinningTile3 = NonWinningTile2;
                        while (null != (NonWinningTile3 = HandTiles.GetNextAvailableTile(NonWinningTile3)))
                        {
                            //var stringified = String.Format("{0}{1}{2}", NonWinningTile1.Tile, NonWinningTile2.Tile, NonWinningTile3.Tile);
                            var stringified = String.Format("{0}{1}", NonWinningTile1.Tile, NonWinningTile2.Tile);

                            if (!checkedSequences.Contains(stringified))
                            {
                                //We always mark it as winning, even if it isn't- So we can count how many invalid melds we have. Every invalid meld = +1 shanten
                                NonWinningTile1.IsUsed = true;
                                NonWinningTile2.IsUsed = true;
                                NonWinningTile3.IsUsed = true;

                                NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                                NonWinningTile2.MeldID = NonWinningTile1.MeldID;
                                NonWinningTile3.MeldID = NonWinningTile1.MeldID;

                                Hand.GetShanten_Backtracing_v1(HandTiles, Depth + 1, Pairs, Triplets, ++Sequences, ShantenHands, ref CurrentReplacementsNeeded);
                                
                                checkedSequences.Add(stringified);
                                Sequences--;

                                NonWinningTile1.IsUsed = false;
                                NonWinningTile2.IsUsed = false;
                                NonWinningTile3.IsUsed = false;

                                NonWinningTile1.MeldID = -1;
                                NonWinningTile2.MeldID = -1;
                                NonWinningTile3.MeldID = -1;
                                
                            }
                        }
                    }
                }
                #endregion
            }

            //If we have 2+ pairs, our only option is a 7 pairs, so we only check for triplets when we have 0 or 1 pairs
            if (Triplets + Sequences < 4 && Pairs < 2)
            {
                #region Triplets
                List<string> checkedTriplets = new List<string>();
                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    {
                        if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile2.Tile)) continue;

                        var NonWinningTile3 = NonWinningTile2;
                        while (null != (NonWinningTile3 = HandTiles.GetNextAvailableTile(NonWinningTile3)))
                        {
                            //if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile3.Tile)) continue;
                            //We always mark it as winning, even if it isn't- So we can count how many invalid melds we have. Every invalid meld = +1 shanten

                            //var stringified = String.Format("{0}{1}{2}", NonWinningTile1.Tile, NonWinningTile2.Tile, NonWinningTile3.Tile);
                            var stringified = String.Format("{0}{1}", NonWinningTile1.Tile, NonWinningTile2.Tile);

                            if (!checkedTriplets.Contains(stringified))
                            {
                                NonWinningTile1.IsUsed = true;
                                NonWinningTile2.IsUsed = true;
                                NonWinningTile3.IsUsed = true;

                                NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                                NonWinningTile2.MeldID = NonWinningTile1.MeldID;
                                NonWinningTile3.MeldID = NonWinningTile1.MeldID;

                                Hand.GetShanten_Backtracing_v1(HandTiles, Depth + 1, Pairs, ++Triplets, Sequences, ShantenHands, ref CurrentReplacementsNeeded);

                                checkedTriplets.Add(stringified);
                                Triplets--;

                                NonWinningTile1.IsUsed = false;
                                NonWinningTile2.IsUsed = false;
                                NonWinningTile3.IsUsed = false;

                                NonWinningTile1.MeldID = -1;
                                NonWinningTile2.MeldID = -1;
                                NonWinningTile3.MeldID = -1;

                            }
                        }
                    }
                }
                #endregion
            }

            if ((Triplets + Sequences > 0 && Pairs < 1) //If we have at least one triplet/sequence, we only need 1 pair
                || Triplets + Sequences == 0) //If we don't have any sequences, we can go for 7 pairs
            {
                #region Pairs
                List<string> checkedPairs = new List<string>();
                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    {
                        //var stringified = NonWinningTile1.Tile.ToString() + NonWinningTile2.Tile.ToString();
                        var stringified = NonWinningTile1.Tile.ToString();

                        //We always mark it as winning, even if it isn't- So we can count how many invalid melds we have. Every invalid meld = +1 shanten
                        //if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile2.Tile)) continue;

                        if (!checkedPairs.Contains(stringified))
                        {
                            NonWinningTile1.IsUsed = true;
                            NonWinningTile2.IsUsed = true;
                                                    
                            NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                            NonWinningTile2.MeldID = NonWinningTile1.MeldID;

                            Hand.GetShanten_Backtracing_v1(HandTiles, Depth + 1, ++Pairs, Triplets, Sequences, ShantenHands, ref CurrentReplacementsNeeded);

                            checkedPairs.Add(stringified);
                            Pairs--;

                            NonWinningTile1.IsUsed = false;
                            NonWinningTile2.IsUsed = false;

                            NonWinningTile1.MeldID = -1;
                            NonWinningTile2.MeldID = -1;
                        }
                    }
                }
                #endregion
            }

            return;
        }

        public static bool IsValidHand(Type ScorerType, List<Tile> ClosedTiles, List<Meld> ExposedMelds, IGame Game, IPlayer Player)
        {
            if (!ScorerType.IsSubclassOf(typeof(Scorer))) throw new ArgumentException("Invalid scorer");
            var ScorerConstructor = ScorerType.GetConstructor(new Type[] { ExposedMelds.GetType(), ExposedMelds.GetType(), Game.GetType(), Player.GetType() });

            var allHands = Hand.GetAllHands(ClosedTiles, ExposedMelds, Game, Player);

            foreach (var possibleHand in allHands)
            {
                var handScorer = ScorerConstructor.Invoke(new object[] { possibleHand, ExposedMelds, Game, Player }) as Scorer;
                if (handScorer.IsCompleteHand()) return true;
            }

            return false;
        }


        private static void GetAllHands_Backtracking(List<TrackedTile> HandTiles, int Depth, int Pairs, int Triplets, int Sequences, List<List<Meld>> ValidHands)
        {
            if ((Pairs == 7) || (Pairs == 1 && Triplets + Sequences == 4))
            {
                var newHand = new List<Meld>();
                /*foreach (var TrackedTile in HandTiles.Where(x => x.MeldID >= 0).OrderBy(x => x.MeldID)) //MeldID -1 are the exposed melds
                {
                    if (null == newHand.ElementAtOrDefault(TrackedTile.MeldID))
                    {
                        //newHand[TrackedTile.MeldID] = new Meld();
                        //newHand.Insert(TrackedTile.MeldID, new Meld());
                        newHand.Add(new Meld());
                    }

                    newHand[TrackedTile.MeldID].Tiles.Add(TrackedTile.Tile);
                }*/
                var melds = HandTiles.Where(x => x.MeldID >= 0).GroupBy(x => x.MeldID);
                foreach (var meld in melds)
                {
                    newHand.Add(new Meld(meld.Select(x => x.Tile).ToList()));
                }

                ValidHands.Add(newHand);

                return;
            }


            if (Triplets + Sequences < 4 && Pairs < 2) //If we have 2+ pairs, our only option is a 7 pairs, so we only check for sequences when we have 0 or 1 pairs
            {
                List<string> alreadyRejected = new List<string>();

                //foreach (var NonWinningTile1 in HandTiles.Where(x => !x.IsUsed))
                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    //foreach (var NonWinningTile2 in HandTiles.Where(x => x != NonWinningTile1).Where(x => !x.IsUsed && NonWinningTile1.Tile.IsNextTile(x.Tile)))
                    {
                        if (!NonWinningTile1.Tile.IsNextTile(NonWinningTile2.Tile)) continue;

                        var NonWinningTile3 = NonWinningTile2;
                        while (null != (NonWinningTile3 = HandTiles.GetNextAvailableTile(NonWinningTile3)))
                        //foreach (var NonWinningTile3 in HandTiles.Where(x => x != NonWinningTile1 && x != NonWinningTile2).Where(x => !x.IsUsed && NonWinningTile2.Tile.IsNextTile(x.Tile)))
                        {
                            if (!NonWinningTile2.Tile.IsNextTile(NonWinningTile3.Tile)) continue;

                            var stringified = String.Format("{0}{1}{2}", NonWinningTile1.Tile, NonWinningTile2.Tile, NonWinningTile3.Tile);

                            if (!alreadyRejected.Contains(stringified))
                            {

                                NonWinningTile1.IsUsed = true;
                                NonWinningTile2.IsUsed = true;
                                NonWinningTile3.IsUsed = true;

                                NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                                NonWinningTile2.MeldID = NonWinningTile1.MeldID;
                                NonWinningTile3.MeldID = NonWinningTile1.MeldID;

                                Hand.GetAllHands_Backtracking(HandTiles, Depth + 1, Pairs, Triplets, ++Sequences, ValidHands);

                                alreadyRejected.Add(stringified);
                                Sequences--;

                                NonWinningTile1.IsUsed = false;
                                NonWinningTile2.IsUsed = false;
                                NonWinningTile3.IsUsed = false;

                                NonWinningTile1.MeldID = -1;
                                NonWinningTile2.MeldID = -1;
                                NonWinningTile3.MeldID = -1;

                            }
                        }
                    }
                }
            }

            if (Triplets + Sequences < 4 && Pairs < 2) //If we have 2+ pairs, our only option is a 7 pairs, so we only check for sequences when we have 0 or 1 pairs
            {
                List<string> alreadyRejected = new List<string>();
                //foreach (var NonWinningTile1 in HandTiles.Where(x => !x.IsUsed))
                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    //foreach (var NonWinningTile2 in HandTiles.Where(x => x != NonWinningTile1).Where(x => !x.IsUsed && NonWinningTile1.Tile.IsPairOrTriplet(x.Tile)))
                    {
                        if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile2.Tile)) continue;

                        var NonWinningTile3 = NonWinningTile2;
                        while (null != (NonWinningTile3 = HandTiles.GetNextAvailableTile(NonWinningTile3)))
                        //foreach (var NonWinningTile3 in HandTiles.Where(x => x != NonWinningTile1 && x != NonWinningTile2).Where(x => !x.IsUsed && NonWinningTile2.Tile.IsPairOrTriplet(x.Tile)))
                        {
                            if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile3.Tile)) continue;
                            var stringified = String.Format("{0}{1}{2}", NonWinningTile1.Tile, NonWinningTile2.Tile, NonWinningTile3.Tile);

                            if (!alreadyRejected.Contains(stringified))
                            {
                                NonWinningTile1.IsUsed = true;
                                NonWinningTile2.IsUsed = true;
                                NonWinningTile3.IsUsed = true;

                                NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                                NonWinningTile2.MeldID = NonWinningTile1.MeldID;
                                NonWinningTile3.MeldID = NonWinningTile1.MeldID;

                                Hand.GetAllHands_Backtracking(HandTiles, Depth + 1, Pairs, ++Triplets, Sequences, ValidHands);
                                
                                alreadyRejected.Add(stringified);
                                Triplets--;

                                NonWinningTile1.IsUsed = false;
                                NonWinningTile2.IsUsed = false;
                                NonWinningTile3.IsUsed = false;

                                NonWinningTile1.MeldID = -1;
                                NonWinningTile2.MeldID = -1;
                                NonWinningTile3.MeldID = -1;
                                
                            }
                        }
                    }
                }
            }

            if ((Triplets + Sequences > 0 && Pairs < 1) //If we have at least one triplet/sequence, we only need 1 pair
                || Triplets + Sequences == 0) //If we don't have any sequences, we can go for 7 pairs
            {
                List<string> rejectedPairs = new List<string>();
                TrackedTile NonWinningTile1 = null;
                while (null != (NonWinningTile1 = HandTiles.GetNextAvailableTile(NonWinningTile1)))
                {
                    var NonWinningTile2 = NonWinningTile1;
                    while (null != (NonWinningTile2 = HandTiles.GetNextAvailableTile(NonWinningTile2)))
                    //foreach (var NonWinningTile2 in HandTiles.Where(x => x != NonWinningTile1).Where(x => !x.IsUsed && NonWinningTile1.Tile.IsPairOrTriplet(x.Tile)))
                    {
                        if (!NonWinningTile1.Tile.IsPairOrTriplet(NonWinningTile2.Tile)) continue;
                        var stringified = NonWinningTile1.Tile.ToString() + NonWinningTile2.Tile.ToString();

                        if (!rejectedPairs.Contains(stringified))
                        {
                            NonWinningTile1.IsUsed = true;
                            NonWinningTile2.IsUsed = true;

                            NonWinningTile1.MeldID = 1 + HandTiles.Max(x => x.MeldID);
                            NonWinningTile2.MeldID = NonWinningTile1.MeldID;

                            Hand.GetAllHands_Backtracking(HandTiles, Depth + 1, ++Pairs, Triplets, Sequences, ValidHands);

                            rejectedPairs.Add(stringified);
                            Pairs--;

                            NonWinningTile1.IsUsed = false;
                            NonWinningTile2.IsUsed = false;

                            NonWinningTile1.MeldID = -1;
                            NonWinningTile2.MeldID = -1;
                            
                        }
                    }
                }
            }

            return;
        }

    }
}
