using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public class ScorerJapanese : Scorer
    {
        public ScorerJapanese(List<Meld> Concealed, List<Meld> Exposed, IGame Game, IPlayer Player) : base(Concealed,Exposed,Game,Player)
        {
        }

        public static int GetBasicValue(List<Meld> Concealed, List<Meld> Exposed, IGame Game, IPlayer Winner)
        {
            /**
            The payment to the winner of a hand is calculated as follows:
            1. Counting han (飜)
            2. If it is five han or more, it is mangan (満貫) or more and the calculation of basic points is omitted
            3. Counting fu (符)
            4. If it is clear that the han and fu yield more than mangan, the calculation of basic points is omitted
            5. Calculating the basic points based on the fu and han
            6. Multiplying the basic points depending on whether the winner is the dealer or non-dealer, and whether the hand is won by tsumo or ron
            7. Adding bonuses based on the number of counters
            (8. Adjusting the payment by the wareme rule)
            */

            //return 0;
            var han = ScorerJapanese.CountHan(Concealed, Exposed, Winner.Riichi, Winner.Tsumo, Winner.Tsumo, Game, Winner);
            var basicPoints = 0;
            if (han < 5)
            {
                var fu = 0;
                fu = ScorerJapanese.CountFu(Concealed, Exposed, Game, Winner);

                //basic points = fu × 2^(2+han) ]
                //When a non-dealer (ko, 子: child) goes out by self-draw, the dealer (oya, 親: parent) pays the winner 2 × basic points, and the other two non-dealers pay the winner 1 × basic points.
                //When a non-dealer goes out by discard, the discarding player pays the winner 4 × basic points.
                //When the dealer goes out by self-drawn, all the three non-dealers pay the winner 2 × basic points.
                //When the dealer goes out by discard, the discarding non-dealer pays the winner 6 × basic points.
                //The actual points given are rounded up to the nearest 100. Even if the values of han and fu are the same, the points received for self-draw wins often slightly deviate from those received for discard wins because of rounding.

                basicPoints = (int)(fu * Math.Pow(2, 2 + han));
            }
            if (basicPoints > 2000) basicPoints = 2000;

            if (han >= 13) basicPoints = 8000;
            else if (han >= 11) basicPoints = 6000;
            else if (han >= 8) basicPoints = 4000;
            else if (han >= 6) basicPoints = 3000;
            else if (han >= 5) basicPoints = 2000;

            return basicPoints;
        }

        /// <summary>
        /// http://en.wikipedia.org/wiki/Japanese_Mahjong_scoring_rules
        /// http://en.wikipedia.org/wiki/Japanese_Mahjong_yaku
        /// </summary>
        /// <param name="Concealed"></param>
        /// <param name="Exposed"></param>
        /// <param name="Doras"></param>
        /// <param name="Dealer"></param>
        /// <returns></returns>
        public static IDictionary<IPlayer,int> GetPayments(List<Meld> Concealed, List<Meld> Exposed, IGame Game, IPlayer Winner)
        {
            var basicPoints = ScorerJapanese.GetBasicValue(Concealed, Exposed, Game, Winner);

            Dictionary<IPlayer, int> payments = new Dictionary<IPlayer, int>();
            var OtherPlayers = Game.Players.Where(x => x != Winner);
            if (Winner.Tsumo)
            {
                if (Winner.IsDealer)
                {
                    foreach (var OtherPlayer in OtherPlayers)
                    {
                        payments.Add(OtherPlayer, (2 * basicPoints).ToNext100());
                    }
                }
                else
                {
                    foreach (var OtherPlayer in OtherPlayers)
                    {
                        if(OtherPlayer.IsDealer)
                            payments.Add(OtherPlayer, (2 * basicPoints).ToNext100());
                        else
                            payments.Add(OtherPlayer, (1 * basicPoints).ToNext100());
                    }
                }
            }
            else //ron
            {
                if (Winner.IsDealer)
                {
                    payments.Add(Game.CurrentPlayer, (6 * basicPoints).ToNext100());
                }
                else
                {
                    payments.Add(Game.CurrentPlayer, (4 * basicPoints).ToNext100());
                }

            }

            //Let's make this actual payments, winner gets +XXXXX, payers get -XXXXX
            var earned = payments.Values.Sum();

            Dictionary<IPlayer, int> finalPayments = new Dictionary<IPlayer, int>();
            foreach (var payment in payments)
            {
                finalPayments.Add(payment.Key, -1 * payment.Value);
            }
            finalPayments[Winner] = earned;

            //return payments;
            return finalPayments;
        }

        public static int CountFu(List<Meld> Concealed, List<Meld> Exposed, IGame Game, IPlayer Player)
        {
            var Fus = ScorerJapanese.GetFu(Concealed, Exposed, Player.Riichi, Player.Tsumo, Player.Ippatsu, Game, Player);
            int fusValue = Fus.Sum(x => x.Value);

            ////Round the fu up to the nearest 10 (except for the 25 fu of a seven pairs hand).
            if (fusValue != 25)
            {
                fusValue = (int)(Math.Ceiling(Decimal.Divide(fusValue, 10)) * 10);
            }

            return fusValue;
        }

        public static bool IsSevenPairs(List<Meld> Melds)
        {
            return Melds.Where(x => x.Type == MeldType.Pair).Count() == 7;
        }

        /// <summary>
        /// No-points hand
        /// [wiki]
        /// The hand must have no triplets or quads, and must not contain any pair of dragons, player’s own wind, or wind of the round which is worth points
        /// Furthermore, the hand must be waiting for multiple winning tiles that can make a sequence,[6] such as having number 2 and 3 and waiting for 1 or 4
        /// [saki]
        /// Thus, the four melds must be runs, the hand must be won on a two-sided wait, and it must have a valueless pair. 
        /// Value pairs include dragons, the player's seat wind, and the prevailing wind. 
        /// </summary>
        /// <param name="Melds"></param>
        /// <param name="PlayerWind"></param>
        /// <param name="GameWind"></param>
        /// <returns></returns>
        public static bool IsNoPointsHand(List<Meld> Melds, TileHonors PlayerWind, TileHonors GameWind, Tile WinningTile)
        {
            var winningMeld = Melds.Find(x => x.Tiles.Contains(WinningTile));
            if (null != winningMeld)
            {
                if (winningMeld.Type == MeldType.Sequence)
                {
                    winningMeld.Tiles.Sort();
                    //If the winning tile is the middle tile of a run, 
                    if (winningMeld.Tiles.IndexOf(WinningTile) == 1) //0>1>2
                        return false; //Wait on middle tile of a run
                    //is the number 3 tile of a 1-2-3 run, 
                    else if ((winningMeld.Tiles.First() as SuitedTile).Rank == 1 && winningMeld.Tiles.IndexOf(WinningTile) == 2)
                        return false; //Waiting on 3 for 1-2-3
                    //or the number 7 tile of a 7-8-9 run,
                    else if ((winningMeld.Tiles.Last() as SuitedTile).Rank == 9 && winningMeld.Tiles.IndexOf(WinningTile) == 0)
                        return false; //Waiting on 7 for 7-8-9
                }
                else return false;
            }

            return Melds.GetAllTiles().Count > 0
            && false == Melds.Any(x => x.Type == MeldType.Triplet)
            && false == Melds.Any(x => x.Type == MeldType.Pair && x.Tiles[0] is DragonTile)
            && false == Melds.GetAllTiles().Any(x => x is HonorTile && (x as HonorTile).Honor == PlayerWind)
            && false == Melds.GetAllTiles().Any(x => x is HonorTile && (x as HonorTile).Honor == GameWind);
        }

        /// <summary>
        /// Two sequences of the same numbers in the same suit. 
        /// "One set of identical sequences" in http://en.wikipedia.org/wiki/Japanese_Mahjong_yaku
        /// Ex: B1B2B3 + B1B2B3 + XXX + YYY + ZZ
        /// </summary>
        /// <returns></returns>
        public static bool IsPairOfIdenticalSequences(List<Meld> Melds)
        {
            var seqBam = Melds.Where(x => x.Type == MeldType.Sequence && x.Tiles[0] is SuitedTile && (x.Tiles[0] as SuitedTile).Suit == TileSuits.Bamboo);
            var seqCir = Melds.Where(x => x.Type == MeldType.Sequence && x.Tiles[0] is SuitedTile && (x.Tiles[0] as SuitedTile).Suit == TileSuits.Circle);
            var seqChr = Melds.Where(x => x.Type == MeldType.Sequence && x.Tiles[0] is SuitedTile && (x.Tiles[0] as SuitedTile).Suit == TileSuits.Character);

            if (seqBam.Count() == 2 && seqBam.First().Tiles[0].Value() == seqBam.Last().Tiles[0].Value()) return true;
            if (seqCir.Count() == 2 && seqCir.First().Tiles[0].Value() == seqCir.Last().Tiles[0].Value()) return true;
            if (seqChr.Count() == 2 && seqChr.First().Tiles[0].Value() == seqChr.Last().Tiles[0].Value()) return true;

            return false;
        }

        public static bool IsTwoPairsOfIdenticalSequences(List<Meld> Melds)
        {
            var sequences = Melds.Where(x => x.Type == MeldType.Sequence).ToList();
            if (sequences.Count < 4) return false;

            //4 sequences, combos of two are: 01,02,03,12,13,23
            if (ScorerJapanese.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[0], sequences[1] }) && ScorerJapanese.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[2], sequences[3] }))
                return true;
            if (ScorerJapanese.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[0], sequences[2] }) && ScorerJapanese.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[1], sequences[3] }))
                return true;
            if (ScorerJapanese.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[0], sequences[3] }) && ScorerJapanese.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[1], sequences[2] }))
                return true;
            //same as 03+12
            //if (Hand.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[1], sequences[2] }) && Hand.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[0], sequences[3] }))
            //    return true;
            //same as 02+13
            //if (Hand.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[1], sequences[3] }) && Hand.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[0], sequences[2] }))
            //    return true;
            //same as 01+23
            //if (Hand.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[2], sequences[3] }) && Hand.IsPairOfIdenticalSequences(new List<Meld>(2) { sequences[0], sequences[1] }))
            //    return true;

            return false;
        }

        public static bool IsFullStraight(List<Meld> Melds)
        {
            var seqs = Melds.Where(x => x.Type == MeldType.Sequence && x.Tiles[0] is SuitedTile && (x.Tiles[0] as SuitedTile).Suit == TileSuits.Bamboo);
            if(seqs.Count() < 3)
                seqs = Melds.Where(x => x.Type == MeldType.Sequence && x.Tiles[0] is SuitedTile && (x.Tiles[0] as SuitedTile).Suit == TileSuits.Circle);
            if (seqs.Count() < 3)
                seqs = Melds.Where(x => x.Type == MeldType.Sequence && x.Tiles[0] is SuitedTile && (x.Tiles[0] as SuitedTile).Suit == TileSuits.Character);
            if (seqs.Count() < 3)
                return false;

            var seqsList = seqs.OrderBy(x => x.Tiles.First()).ToList();
            seqsList.Sort((x,y) => x.Tiles.First().Value().CompareTo(y.Tiles.First().Value()));

            if (                      seqsList[0].Tiles.Last().IsNextTile(seqsList[1].Tiles.First()) && seqsList[1].Tiles.Last().IsNextTile(seqsList[2].Tiles.First()))
                return true;
            if (seqsList.Count > 3 && seqsList[1].Tiles.Last().IsNextTile(seqsList[2].Tiles.First()) && seqsList[2].Tiles.Last().IsNextTile(seqsList[3].Tiles.First()))
                return true;

            return false;
        }

        public static bool IsThreeTriplets(List<Meld> Melds)
        {
            return Melds.Count(x => x.Type == MeldType.Triplet) == 3;
        }

        /// <summary>
        /// Three colour straight
        /// Three sequences of the same numbers in all three suits.
        /// O1O2O3 + C1C2C3 + B1B2B3 + XXX + YY
        /// </summary>
        /// <param name="Melds"></param>
        /// <returns></returns>
        public static bool IsThreeColorSequences(List<Meld> Melds)
        {
            var sequences = Melds.Where(x => x.Type == MeldType.Sequence);
            if (sequences.Count() < 3) return false;

            var seqBam = Melds.Where(x => x.Type == MeldType.Sequence && x.Tiles[0] is SuitedTile && (x.Tiles[0] as SuitedTile).Suit == TileSuits.Bamboo);
            var seqCir = Melds.Where(x => x.Type == MeldType.Sequence && x.Tiles[0] is SuitedTile && (x.Tiles[0] as SuitedTile).Suit == TileSuits.Circle);
            var seqChr = Melds.Where(x => x.Type == MeldType.Sequence && x.Tiles[0] is SuitedTile && (x.Tiles[0] as SuitedTile).Suit == TileSuits.Character);

            foreach (var Bam in seqBam)
            {
                foreach (var Cir in seqCir)
                {
                    if ((Bam.Tiles.First() as SuitedTile).Rank == (Cir.Tiles.First() as SuitedTile).Rank)
                    {
                        foreach (var Chr in seqChr)
                        {
                            if ((Bam.Tiles.First() as SuitedTile).Rank == (Chr.Tiles.First() as SuitedTile).Rank)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsAllTriplets(List<Meld> Melds)
        {
            return Melds.Count(x => x.Type == MeldType.Triplet) == 4;
        }

        //Only numbered tiles 2-8
        public static bool IsAllSimples(List<Meld> Melds)
        {
            if (Melds.Count == 0) return false;
            if (Melds.GetAllTiles().Count == 0) return false;
            return !Melds.GetAllTiles().Any(x => !(x is SuitedTile) || (x as SuitedTile).Rank == 1 || (x as SuitedTile).Rank == 9);
        }

        public static int CountHonorTriplets(List<Meld> Melds, TileHonors PlayerWind, TileHonors GameWind)
        {
            int count = 0;
            count += Melds.Count(x => x.Type == MeldType.Triplet && x.Tiles[0] is DragonTile); //dragon tiles
            //If player wind == game wind, it counts double, so no need to make special exceptions
            count += Melds.Count(x => x.Type == MeldType.Triplet && x.Tiles[0] is HonorTile && (x.Tiles[0] as HonorTile).Honor == PlayerWind);
            count += Melds.Count(x => x.Type == MeldType.Triplet && x.Tiles[0] is HonorTile && (x.Tiles[0] as HonorTile).Honor == GameWind);

            return count;
        }

        /// <summary>
        /// Terminal or honor in each set
        /// The hand contains at least one sequence.
        /// The sequences in the hand must be 1-2-3 and 7-8-9
        /// Triplets and the pair must be 1’s, 9’s and honor tiles.
        /// </summary>
        /// <returns></returns>
        public static bool IsTerminalOrHonorOnAllSets(List<Meld> Melds)
        {
            if (Melds.Count(x => x.Type == MeldType.Sequence) == 0) return false;

            foreach (var meld in Melds)
            {
                if (meld.Tiles.Count(x => x is SuitedTile && (x as SuitedTile).Rank == 1) > 0) continue;
                if (meld.Tiles.Count(x => x is SuitedTile && (x as SuitedTile).Rank == 9) > 0) continue;
                if (meld.Tiles.Count(x => x is HonorTile) > 0) continue;
                if (meld.Tiles.Count(x => x is DragonTile) > 0) continue;

                //If I get here, it means it doesn't have any of the previous ones, so breaking
                return false;
            }

            return true;
        }

        /// <summary>
        /// "Pure Hand"
        /// No honor tiles are included
        /// The sequences must be 1-2-3 and 7-8-9
        /// Triplets and the pair must be 1’s and 9’s
        /// The hand has at least one sequence.
        /// </summary>
        /// <param name="Melds"></param>
        /// <returns></returns>
        public static bool IsTerminalsOnAllSets(List<Meld> Melds)
        {
            if (Melds.Count(x => x.Type == MeldType.Sequence) == 0) return false;

            foreach (var meld in Melds)
            {
                if (meld.Tiles.Count(x => x is SuitedTile && (x as SuitedTile).Rank == 1) > 0) continue;
                if (meld.Tiles.Count(x => x is SuitedTile && (x as SuitedTile).Rank == 9) > 0) continue;
                
                //I think these 2 are not needed
                if (meld.Tiles.Count(x => x is HonorTile) > 0) return false;
                if (meld.Tiles.Count(x => x is DragonTile) > 0) return false;

                //If I get here, it means it doesn't have any of the previous ones, so breaking
                return false;
            }

            return true;
        }
        
        public static bool IsAllTerminalsAndHonors(List<Meld> Melds)
        {
            foreach (var meld in Melds)
            {
                if (meld.Tiles.Count(x => x is SuitedTile) > 0)
                {
                    if (meld.Tiles.Count(x => (x is SuitedTile) && (x as SuitedTile).Rank != 1 && (x as SuitedTile).Rank != 9) > 0)
                        return false;
                }
            }

            return Melds.GetAllTiles().Count > 0;
        }

        /// <summary>
        /// Two triplets or quads of dragons, plus a pair of the third.
        /// </summary>
        /// <param name="Melds"></param>
        /// <returns></returns>
        public static bool IsLittleThreeDragons(List<Meld> Melds)
        {
            //if (Melds.Count(x => x.Type == MeldType.Pair) > 1) return false;
            //if (Melds.Count(x => x.Type == MeldType.Triplet) < 2) return false;

            if (Melds.Count(x => x.Type == MeldType.Triplet && x.Tiles[0] is DragonTile) < 2) return false;
            if (Melds.Count(x => x.Type == MeldType.Pair && x.Tiles[0] is DragonTile) != 1) return false; //exactly 1, less than 1 = no dragon pair, more than 1 = 7 pairs

            return true;
        }

        /// <summary>
        /// Three triplets/quads of winds and a pair of the fourth wind.
        /// </summary>
        /// <param name="Melds"></param>
        /// <returns></returns>
        public static bool IsLittleFourWinds(List<Meld> Melds)
        {
            if (Melds.Count(x => x.Type == MeldType.Triplet && x.Tiles[0] is HonorTile) < 3) return false;
            if (Melds.Count(x => x.Type == MeldType.Pair && x.Tiles[0] is HonorTile) != 1) return false; //exactly 1, less than 1 => no pair, more than 1 => 7 pairs

            return true;
        }

        /// <summary>
        /// A triplet or quad of each type of dragon tile.
        /// </summary>
        /// <param name="Melds"></param>
        /// <returns></returns>
        public static bool IsBigThreeDragons(List<Meld> Melds)
        {
            if (Melds.Count(x => x.Type == MeldType.Triplet && x.Tiles[0] is DragonTile) < 3) return false;

            return true;
        }

        public static bool IsBigFourWinds(List<Meld> Melds)
        {
            if (Melds.Count(x => x.Type == MeldType.Triplet && x.Tiles[0] is HonorTile) < 4) return false;

            return true;
        }

        /// <summary>
        /// The hand contains tiles from one suit and honors. The honors can be two or more sets. The hand can be seven pairs.
        /// </summary>
        /// <param name="Melds"></param>
        /// <returns></returns>
        public static bool IsHalfFlush(List<Meld> Melds)
        {
            var suitedMelds = Melds.Where(x => x.Tiles.Count > 0 && x.Tiles[0] is SuitedTile).ToList();

            if (suitedMelds.Count == 0) return false;

            TileSuits currentSuit = (suitedMelds[0].Tiles[0] as SuitedTile).Suit;
            for (int x = 1; x < suitedMelds.Count; x++)
                if (currentSuit != (suitedMelds[x].Tiles[0] as SuitedTile).Suit)
                    return false;

            return true;
        }

        public static bool IsFlush(List<Meld> Melds)
        {
            if (Melds.Count < 1) return false;

            var allTiles = Melds.GetAllTiles();

            if (allTiles.Count < 1) return false;

            if (allTiles.Count(x => !(x is SuitedTile)) > 0) return false;

            if (allTiles.Any(x => (x as SuitedTile).Suit != (Melds[0].Tiles[0] as SuitedTile).Suit)) return false;

            return true;
        }

        public static bool IsAllHonors(List<Meld> Melds)
        {
            return Melds.GetAllTiles().Count > 0 && Melds.GetAllTiles().Count(x => x is SuitedTile) == 0;
        }

        /// <summary>
        /// All 1s or 9s
        /// </summary>
        /// <param name="Melds"></param>
        /// <returns></returns>
        public static bool IsAllTerminals(List<Meld> Melds)
        {
            var allTiles = Melds.GetAllTiles();

            if (allTiles.Count == 0) return false;
            if (allTiles.Count(x => !(x is SuitedTile)) > 0) return false;
            if (allTiles.Count(x => (x as SuitedTile).Rank != 1 && (x as SuitedTile).Rank != 9) > 0) return false;

            return true;
        }

        /// <summary>
        /// Green tiles are: 2, 3, 4, 6, 8 of bamboo, and green dragons. 
        /// The other bamboo tiles of 1, 5, 7, and 9 have red paint on them, thereby not making them all green.
        /// </summary>
        /// <param name="Melds"></param>
        /// <returns></returns>
        public static bool IsAllGreen(List<Meld> Melds)
        {
            int greenCount = 0;

            foreach (var tile in Melds.GetAllTiles())
            {
                if (tile is DragonTile && (tile as DragonTile).Dragon != TileDragons.Green) return false;
                if (tile is HonorTile) return false;
                if (tile is SuitedTile)
                {
                    switch ((tile as SuitedTile).Rank)
                    {
                        case 1:
                        case 5:
                        case 7: 
                        case 9:
                            return false;
                    }
                }

                greenCount++;
            }

            return greenCount > 0 && Melds.GetAllTiles().Count == greenCount;
        }

        /// <summary>
        /// One of each dragon tile, one of each wind tile, a 1 and a 9 (terminal) from each suit, plus any tile that matches anything else in the hand.
        /// If a player wins in a 13-way wait for the pair, the hand is worth two yakuman, which is called daburu (double) yakuman.
        /// </summary>
        /// <param name="Melds"></param>
        /// <returns></returns>
        //public static bool Is13Orphans(List<Meld> Melds)
        public static bool Is13Orphans(List<Tile> AllTiles)
        {
            if (AllTiles == null || AllTiles.Count == 0) return false;
            //var AllTiles = Melds.GetAllTiles().ToList();

            if (AllTiles.Count(x => x is HonorTile && (x as HonorTile).Honor == TileHonors.East) < 1) return false;
            if (AllTiles.Count(x => x is HonorTile && (x as HonorTile).Honor == TileHonors.North) < 1) return false;
            if (AllTiles.Count(x => x is HonorTile && (x as HonorTile).Honor == TileHonors.South) < 1) return false;
            if (AllTiles.Count(x => x is HonorTile && (x as HonorTile).Honor == TileHonors.West) < 1) return false;

            if (AllTiles.Count(x => x is DragonTile && (x as DragonTile).Dragon == TileDragons.Green) < 1) return false;
            if (AllTiles.Count(x => x is DragonTile && (x as DragonTile).Dragon == TileDragons.Red) < 1) return false;
            if (AllTiles.Count(x => x is DragonTile && (x as DragonTile).Dragon == TileDragons.White) < 1) return false;

            if (AllTiles.Count(x => x is SuitedTile && (x as SuitedTile).Suit == TileSuits.Bamboo && (x as SuitedTile).Rank == 1) < 1) return false;
            if (AllTiles.Count(x => x is SuitedTile && (x as SuitedTile).Suit == TileSuits.Character && (x as SuitedTile).Rank == 1) < 1) return false;
            if (AllTiles.Count(x => x is SuitedTile && (x as SuitedTile).Suit == TileSuits.Circle && (x as SuitedTile).Rank == 1) < 1) return false;

            if (AllTiles.Count(x => x is SuitedTile && (x as SuitedTile).Suit == TileSuits.Bamboo && (x as SuitedTile).Rank == 9) < 1) return false;
            if (AllTiles.Count(x => x is SuitedTile && (x as SuitedTile).Suit == TileSuits.Character && (x as SuitedTile).Rank == 9) < 1) return false;
            if (AllTiles.Count(x => x is SuitedTile && (x as SuitedTile).Suit == TileSuits.Circle && (x as SuitedTile).Rank == 9) < 1) return false;

            foreach (var tile in AllTiles)
            {
                if (AllTiles.Where(x => x != tile).Count(x => x.Value() == tile.Value()) == 1) 
                    return true;
            }

            return false;
        }

        public static int CountHan(List<Meld> Concealed, List<Meld> Exposed, bool Riichi, bool Tsumo, bool Ippatsu, IGame game, IPlayer player)
        {
            var Hans = ScorerJapanese.GetHan(Concealed, Exposed, Riichi, Tsumo, Ippatsu, game, player);
            //int nHan = 0;
            //foreach (var Han in Hans)
            //{
            //    nHan += Han.Value;
            //}
            int nHan = Hans.Sum(x => x.Value);
            return Math.Min(13,nHan);
        }

        /// <summary>
        /// http://saki.wikia.com/wiki/Scoring_in_mahjong
        /// </summary>
        /// <param name="Concealed"></param>
        /// <param name="Exposed"></param>
        /// <param name="Riichi"></param>
        /// <param name="Tsumo"></param>
        /// <param name="Ippatsu"></param>
        /// <param name="game"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static List<Fu> GetFu(List<Meld> Concealed, List<Meld> Exposed, bool Riichi, bool Tsumo, bool Ippatsu, IGame game, IPlayer player)
        {
            var fus = new List<Fu>();

            //For a claim of a seven pairs hand yaku, start with 25 fu and stop further fu calculations.
            if (ScorerJapanese.IsSevenPairs(Concealed))
            {
                fus.Add(new Fu("7 Pairs","7 Pairs",25));
                return fus;
            }

            //For a tsumo win or an open hand, start with 20 fu. A hand that contains called tiles is an open hand.
            if (Tsumo) fus.Add(new Fu("Tsumo", "", 20));
            else if (Exposed.Count > 0) fus.Add(new Fu("Open Hand", "", 20));

            //For a ron with a closed hand, start with 30 fu. This is fu for a menzen ron, which is partial compensation compared to a menzen tsumo.
            if (!Tsumo && Exposed.Count == 0) fus.Add(new Fu("Closed han ron", "", 20));

            #region Triplets
            //For each triplet of simples, add 2 fu for an open meld or 4 fu for a closed meld.
            foreach (var tripletOfSimples in Concealed.Where(x => x.Type == MeldType.Triplet && x.Tiles.Count==3 && !x.Tiles.Any(y => y.IsHonorOrTerminal())))
            {
                fus.Add(new Fu("Ankou", "Triplet of Simples, Concealed" /*+ ", " + tripletOfSimples.ToString()*/, 4));
            }
            foreach (var tripletOfSimples in Exposed.Where(x => x.Type == MeldType.Triplet && x.Tiles.Count == 3 && !x.Tiles.Any(y => y.IsHonorOrTerminal())))
            {
                fus.Add(new Fu("Minkou", "Triplet of Simples, Open" /*+ ", " + tripletOfSimples.ToString()*/, 2));
            }
            //For each triplet of honors or terminals, add 4 fu for an open meld or 8 fu for a closed meld.
            foreach (var tripletOfSimples in Concealed.Where(x => x.Type == MeldType.Triplet && x.Tiles.Count == 3 && x.Tiles.Any(y => y.IsHonorOrTerminal())))
            {
                fus.Add(new Fu("Ankou", "Triplet of Honors/Terminals, Concealed" /*+ ", " + tripletOfSimples.ToString()*/, 8));
            }
            foreach (var tripletOfSimples in Exposed.Where(x => x.Type == MeldType.Triplet && x.Tiles.Count == 3 && x.Tiles.Any(y => y.IsHonorOrTerminal())))
            {
                fus.Add(new Fu("Minkou", "Triplet of Honors/Terminals, Open" /*+ ", " + tripletOfSimples.ToString()*/, 4));
            }
            #endregion

            #region Quads
            //For each quad of simples, add 8 fu for an open meld or 16 fu for a closed meld.
            foreach (var tripletOfSimples in Concealed.Where(x => x.Type == MeldType.Triplet && x.Tiles.Count == 4 && !x.Tiles.Any(y => y.IsHonorOrTerminal())))
            {
                fus.Add(new Fu("Ankan", "Quad of Simples, Concealed" /*+ ", " + tripletOfSimples.ToString()*/, 16));
            }
            foreach (var tripletOfSimples in Exposed.Where(x => x.Type == MeldType.Triplet && x.Tiles.Count == 4 && !x.Tiles.Any(y => y.IsHonorOrTerminal())))
            {
                fus.Add(new Fu("Minkan", "Quad of Simples, Open" /*+ ", " + tripletOfSimples.ToString()*/, 8));
            }
            //For each quad of honors or terminals, add 16 fu for an open meld or 32 fu for a closed meld.
            foreach (var tripletOfSimples in Concealed.Where(x => x.Type == MeldType.Triplet && x.Tiles.Count == 4 && x.Tiles.Any(y => y.IsHonorOrTerminal())))
            {
                fus.Add(new Fu("Ankan", "Quad of Honors/Terminals" /*+ ", Concealed" + tripletOfSimples.ToString()*/, 32));
            }
            foreach (var tripletOfSimples in Exposed.Where(x => x.Type == MeldType.Triplet && x.Tiles.Count == 4 && x.Tiles.Any(y => y.IsHonorOrTerminal())))
            {
                fus.Add(new Fu("Minkan", "Quad of Honors/Terminals" /*+ ", Open, " + tripletOfSimples.ToString()*/, 16));
            }
            #endregion

            #region Hell Wait
            //This is fu for more difficult waits, add 2 fu
            var winningMeld = Concealed.Find(x => x.Tiles.Contains(player.WinningTile));
            if (winningMeld != null)
            {
                if (winningMeld.Type == MeldType.Sequence)
                {
                    winningMeld.Tiles.Sort();
                    //If the winning tile is the middle tile of a run, 
                    if (winningMeld.Tiles.IndexOf(player.WinningTile) == 1) //0>1>2
                        fus.Add(new Fu("Kanchan-machi", "Wait on middle tile of run", 2));
                    //is the number 3 tile of a 1-2-3 run, 
                    else if ((winningMeld.Tiles.First() as SuitedTile).Rank == 1 && winningMeld.Tiles.IndexOf(player.WinningTile) == 2)
                        fus.Add(new Fu("Penchan-machi", "Waiting on 3 for 1-2-3", 2));
                    //or the number 7 tile of a 7-8-9 run,
                    else if ((winningMeld.Tiles.Last() as SuitedTile).Rank == 9 && winningMeld.Tiles.IndexOf(player.WinningTile) == 0)
                        fus.Add(new Fu("Penchan-machi", "Waiting on 7 for 7-8-9", 2));
                }
                //or completes the pair
                else if (winningMeld.Type == MeldType.Pair)
                {
                    fus.Add(new Fu("Tanki-machi", "Pair", 2));
                }
            }
            #endregion

            #region Honor Pairs
            //For a pair of seat wind,
            //prevailing wind, 
            //or dragon tiles, add 2 fu. 
            //If the pair of tiles is both the seat and prevailing wind, 2 fu is added for each one.
            var pairMeld = Concealed.Find(x => x.Type == MeldType.Pair);
            if(pairMeld.Tiles.Any(x => x is HonorTile && (x as HonorTile).Honor == player.PlayerWind))
                fus.Add(new Fu("Toitsu", "Pair of seat wind", 2));
            if (pairMeld.Tiles.Any(x => x is HonorTile && (x as HonorTile).Honor == game.GameWind))
                fus.Add(new Fu("Toitsu", "Pair of prevailing wind", 2));
            if (pairMeld.Tiles.Any(x => x is DragonTile))
                fus.Add(new Fu("Toitsu", "Pair of dragons", 2));
            #endregion

            //If it's a tsumo win and if the total fu at this point isn't 20, add 2 fu. This is fu for a tsumo win.
            if(Tsumo && fus.Sum(x => x.Value) != 20)
                fus.Add(new Fu("Tsumo win bonus", "", 2));
            //If it's an open hand and if the total fu at this point is 20, add 2 fu. This is fu for an open pinfu.
            if (Exposed.Count > 0 && fus.Sum(x => x.Value) == 20) 
                fus.Add(new Fu("Open Pinfu", "Open hand, less than 20 fus", 2));

            return fus;
        }

        /// <summary>
        /// TODO: Count the doras
        /// </summary>
        /// <param name="Concealed"></param>
        /// <param name="Exposed"></param>
        /// <param name="Riichi"></param>
        /// <param name="Tsumo"></param>
        /// <param name="Ippatsu"></param>
        /// <param name="game"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static List<Han> GetHan(List<Meld> Concealed, List<Meld> Exposed, bool Riichi, bool Tsumo, bool Ippatsu, IGame game, IPlayer player)
        {
            List<Han> Hans = new List<Han>();

            List<Tile> ConcealedTiles = new List<Tile>();
            foreach (var concealedMeld in Concealed)
	        { ConcealedTiles.AddRange(concealedMeld.Tiles); }

            List<Tile> ExposedTiles = new List<Tile>();
            foreach (var exposedMeld in Exposed)
	        { ExposedTiles.AddRange(exposedMeld.Tiles); }

            var AllMelds = Concealed.Concat(Exposed).ToList();

            #region Closed only hands
            if (Exposed.Count == 0)
            {
                if (Riichi) Hans.Add(new Han("Riichi","Riichi",1));
                //Double-ready
                //If a player can declare ready within the first go-around of a hand, they can call "daburu riichi" to declare a double ready for two han instead of one.

                if (Tsumo) 
                    Hans.Add(new Han("Tsumo", "Tsumo", 1));

                if (Riichi && Ippatsu) 
                    Hans.Add(new Han("Ippatsu", "Ippatsu", 1));

                if (ScorerJapanese.IsSevenPairs(Concealed)) 
                    Hans.Add(new Han("Chiitoitsu", "Seven Pairs", 2));

                if (ScorerJapanese.IsNoPointsHand(Concealed, player.PlayerWind, game.GameWind,player.WinningTile)) 
                    Hans.Add(new Han("Pinfu", "No-points hand", 1));

                if (ScorerJapanese.IsTwoPairsOfIdenticalSequences(Concealed)) Hans.Add(new Han("Ryanpeikou", "Two pairs of identical sequences", 3));
                else if (ScorerJapanese.IsPairOfIdenticalSequences(AllMelds)) Hans.Add(new Han("Iipeikou", "Pair of identical sequences", 1));
            }
            #endregion

            #region Open/Closed hands
            //Nagashi mangan
            ////I am so NOT doing this

            //Last tile from the wall
            //If the last self-drawn tile that the last player draws before reaching the dead wall completes that player's hand, the hand’s value increases by one han.
            if(game.TilesLeft == 0 && Tsumo)
                Hans.Add(new Han("Haitei Raoyue", "Last tile from the wall", 1));

            //Last discard
            //One han is added if a player wins on the last discard, that is, the tile discarded by the last player that drew the last tile from the wall. 
            if(game.TilesLeft == 0 && !Tsumo)
                Hans.Add(new Han("Houtei Raoyui", "Last tile from the wall", 1));

            //Dead wall draw
            //When a player declares a quad, they must draw a supplemental tile from the dead wall to keep the number of tiles in the hand consistent. 

            //Robbing a quad
            //If a player has an open triplet and draws the fourth tile, they can add it to the triplet to make a quad. At the time, another player can win on the tile, namely, they can "rob" that tile.

            if (Exposed.Count == 0 && ScorerJapanese.IsAllTriplets(Concealed)) 
                Hans.Add(new Han("Suu Ankou","Four concealed triplets",13)); //4 closed triplets = suu ankou = Yakuman
            else if (ScorerJapanese.IsAllTriplets(AllMelds)) 
                Hans.Add(new Han("Toi Toi", "Four triplets", 2));

            if (Exposed.Count == 0 && ScorerJapanese.IsAllSimples(Concealed))
                Hans.Add(new Han("Tanyao", "No terminals or honors", 1));
            else if (ScorerJapanese.IsAllSimples(AllMelds)) //Some rulesets limit this to closed only
                Hans.Add(new Han("Tanyao", "No terminals or honors", 1));

            if (Exposed.Count == 0 && ScorerJapanese.IsThreeColorSequences(Concealed))
                Hans.Add(new Han("Sanshoku Doujun", "Three colour straight, closed", 2));
            else if(ScorerJapanese.IsThreeColorSequences(AllMelds))
                Hans.Add(new Han("Sanshoku Doujun", "Three colour straight, open", 1));

            //3 closed triplets, regardless of the rest of the hand
            if (ScorerJapanese.IsThreeTriplets(Concealed))
                Hans.Add(new Han("San Ankou", "Three concealed triplets", 2));

            if (Exposed.Count == 0 && ScorerJapanese.IsFullStraight(Concealed))
                Hans.Add(new Han("Ikkitsuukan", "Straight 1 to 9 from a single suit, closed", 2));
            else if(ScorerJapanese.IsFullStraight(AllMelds))
                Hans.Add(new Han("Ikkitsuukan", "Straight 1 to 9 from a single suit, open", 1));

            if (Exposed.Count == 0 && ScorerJapanese.IsTerminalsOnAllSets(Concealed))
                Hans.Add(new Han("Jun Chantaiyao", "Terminals on each set, concealed", 3));
            else if (ScorerJapanese.IsTerminalsOnAllSets(AllMelds))
                Hans.Add(new Han("Junchan", "Terminals on each set, open", 2));
            else if (Exposed.Count == 0 && ScorerJapanese.IsTerminalOrHonorOnAllSets(Concealed))
                Hans.Add(new Han("Chantaiyao", "Terminal or Honor on each set, concealed", 2));
            else if (ScorerJapanese.IsTerminalOrHonorOnAllSets(AllMelds))
                Hans.Add(new Han("Chanta", "Terminal or Honor on each set, open", 1));

            if (ScorerJapanese.IsAllTerminalsAndHonors(AllMelds)) 
                Hans.Add(new Han("Honroutou", "All terminals and honors", 2));
            
            if (Exposed.Count == 0 && ScorerJapanese.IsFlush(Concealed))
                Hans.Add(new Han("Chin'itsu", "Tiles from one single suit, concealed", 6));
            else if (ScorerJapanese.IsFlush(AllMelds))
                Hans.Add(new Han("Chin'itsu", "Tiles from one single suit, open", 5));
            else if (Exposed.Count == 0 && ScorerJapanese.IsHalfFlush(Concealed))
                Hans.Add(new Han("Hon'itsu", "Tiles from one single suit and honors, concealed", 3));
            else if (ScorerJapanese.IsHalfFlush(AllMelds))
                Hans.Add(new Han("Hon'itsu", "Tiles from one single suit and honors, open", 2));

            if (ScorerJapanese.Is13Orphans(Concealed.Concat(Exposed).GetAllTiles()))
                Hans.Add(new Han("Kokushi Musou", "13 orphans", 13)); //Cap, or double cap

            if (ScorerJapanese.IsBigThreeDragons(AllMelds))
                Hans.Add(new Han("Daisangen", "Triplet or Quad of each dragon tile", 13));
            else if (ScorerJapanese.IsLittleThreeDragons(AllMelds))
                Hans.Add(new Han("Shousangen", "Two triplets or quads of dragons, plus a pair of the third", 2));

            if (ScorerJapanese.IsBigFourWinds(AllMelds))
                Hans.Add(new Han("Daisuushii", "Four triplets or quads of winds", 13)); //double limit? wtf!
            else if (ScorerJapanese.IsLittleFourWinds(AllMelds))
                Hans.Add(new Han("Shousuushii", "Three triplets or quads of winds, plus a pair of the third", 13));

            if (ScorerJapanese.IsAllHonors(AllMelds))
                Hans.Add(new Han("Tsuuiisou", "All honors", 13));
            else if (ScorerJapanese.IsAllTerminals(AllMelds))
                Hans.Add(new Han("Chinroutou", "All terminals", 13));

            if (ScorerJapanese.IsAllGreen(AllMelds))
                Hans.Add(new Han("Ryuuiisou", "All green", 13));
            
            #endregion

            #region Counting Stuff
            int nHonorTriplets = ScorerJapanese.CountHonorTriplets(AllMelds, player.PlayerWind, game.GameWind);
            if (nHonorTriplets > 0)
            {
                Hans.Add(new Han("Yakuhai",
                    String.Format("{0} honor triplet{1}/quad{1}", nHonorTriplets, nHonorTriplets > 1 ? "s" : ""),
                    nHonorTriplets));
            }

            int nDoras = ScorerJapanese.CountDoras(AllMelds, game.Doras);
            if (nDoras > 0)
            {
                Hans.Add(new Han("Dora",
                    String.Format("{0} dora{1}", nDoras, nDoras > 1 ? "s" : ""),
                    nDoras));
            }

            int nUraDoras = ScorerJapanese.CountDoras(AllMelds, game.Uradoras);
            if (nUraDoras > 0)
            {
                Hans.Add(new Han("Uradora",
                    String.Format("{0} dora{1}", nUraDoras, nUraDoras > 1 ? "s" : ""),
                    nUraDoras));
            }
            #endregion


            return Hans;
        }

        public static int CountDoras(List<Meld> Melds, List<Tile> Doras)
        {
            int n = 0;
            foreach (var Dora in Doras)
            {
                n += Melds.GetAllTiles().Count(x => x.Value() == Dora.Value());
            }
            return n;
        }
        
        public override IDictionary<IPlayer,int> GetPayments()
        {
            return ScorerJapanese.GetPayments(this.Concealed, this.Exposed, this.Game, this.Player);
        }

        public override int CountFu()
        {
            return ScorerJapanese.CountFu(this.Concealed, this.Exposed, this.Game, this.Player);
        }

        public override int CountHan()
        {
            return ScorerJapanese.CountHan(this.Concealed, this.Exposed, this.Player.Riichi, this.Player.Tsumo, this.Player.Ippatsu, this.Game, this.Player);
        }

        public override List<Han> GetHan()
        {
            return ScorerJapanese.GetHan(this.Concealed, this.Exposed, this.Player.Riichi, this.Player.Tsumo, this.Player.Ippatsu, this.Game, this.Player);
        }

        public override List<Fu> GetFu()
        {
            return ScorerJapanese.GetFu(this.Concealed, this.Exposed, this.Player.Riichi, this.Player.Tsumo, this.Player.Ippatsu, this.Game, this.Player);
        }

        public override bool IsCompleteHand()
        {
            var hans = this.GetHan();
            return hans.Any(x => x.Name.ToUpper() != "DORA" && x.Name.ToUpper() != "URADORA");
        }

        public override int GetBasicValue()
        {
            return ScorerJapanese.GetBasicValue(Concealed, Exposed, Game, Player);
        }
    }
}
