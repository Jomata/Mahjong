using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong.Async.Players
{
    public class AIShantenPlus : Player
    {
        Random rng = new Random();

        //Test runs 1 & 2 have both eventually arrived at these values
        protected static float Pair = -1.008309841f;
        protected static float Adjacent = -0.819516659f;
        protected static float PseudoAdjacent = -0.450917661f;
        protected static float SameSuit = -0.158805847f;
        protected static float Dora = -1.432615995f;
        protected static float AnyDiscard = 0.446508229f;
        protected static float MyDiscard = 0.270123482f; //Se le suma al de AnyDiscard
        protected static float Meld = 0.500935912f;
        protected static float HonorOrTerminal = 0.446309865f;

        //public static float WeightPair { get { return AIShantenPlus.Pair; } }
        //public static float WeightAdjacent { get { return AIShantenPlus.Adjacent; } }
        //public static float WeightPseudoAdjacent { get { return AIShantenPlus.PseudoAdjacent; } }
        //public static float WeightSameSuit { get { return AIShantenPlus.SameSuit; } }
        //public static float WeightDora { get { return AIShantenPlus.Dora; } }
        //public static float WeightAnyDiscard { get { return AIShantenPlus.AnyDiscard; } }
        //public static float WeightMyDiscard { get { return AIShantenPlus.MyDiscard; } }
        //public static float WeightMeld { get { return AIShantenPlus.Meld; } }
        //public static float WeightHonorOrTerminal { get { return AIShantenPlus.HonorOrTerminal; } }

        //public float WeightPair { get { return this.Pair; } protected set { this.Pair = value; } }
        //public float WeightAdjacent { get { return this.Adjacent; } protected set { this.Adjacent = value; } }
        //public float WeightPseudoAdjacent { get { return this.PseudoAdjacent; } protected set { this.PseudoAdjacent = value; } }
        //public float WeightSameSuit { get { return this.SameSuit; } protected set { this.SameSuit = value; } }
        //public float WeightDora { get { return this.Dora; } protected set { this.Dora = value; } }
        //public float WeightAnyDiscard { get { return this.AnyDiscard; } protected set { this.AnyDiscard = value; } }
        //public float WeightMyDiscard { get { return this.MyDiscard; } protected set { this.MyDiscard = value; } }
        //public float WeightMeld { get { return this.Meld; } protected set { this.Meld = value; } }
        //public float WeightHonorOrTerminal { get { return this.HonorOrTerminal; } protected set { this.HonorOrTerminal = value; } }

        public virtual void AverageWeightsWith(AIShantenPlus Another)
        {
            //this.WeightPair = (this.WeightPair + Another.WeightPair) / 2;
            //this.WeightAdjacent = (this.WeightAdjacent + Another.WeightAdjacent) / 2;
            //this.WeightPseudoAdjacent = (this.WeightPseudoAdjacent + Another.WeightPseudoAdjacent) / 2;
            //this.WeightSameSuit = (this.WeightSameSuit + Another.WeightSameSuit) / 2;
            //this.WeightDora = (this.WeightDora + Another.WeightDora) / 2;
            //this.WeightAnyDiscard = (this.WeightAnyDiscard + Another.WeightAnyDiscard) / 2;
            //this.WeightMyDiscard = (this.WeightMyDiscard + Another.WeightMyDiscard) / 2;
            //this.WeightMeld = (this.WeightMeld + Another.WeightMeld) / 2;
            //this.WeightHonorOrTerminal = (this.WeightHonorOrTerminal + Another.WeightHonorOrTerminal) / 2;
        }

        public AIShantenPlus()
        {
            //float variance = 1f;

            //Pair += rng.Next(-1 * variance, variance);
            //Adjacent += rng.Next(-1 * variance, variance);
            //PseudoAdjacent += rng.Next(-1 * variance, variance);
            //SameSuit += rng.Next(-1 * variance, variance);
            //Dora += rng.Next(-1 * variance, variance);
            //AnyDiscard += rng.Next(-1 * variance, variance);
            //MyDiscard += rng.Next(-1 * variance, variance);
            //Meld += rng.Next(-1 * variance, variance);
            //HonorOrTerminal += rng.Next(-1 * variance, variance);

            //Just for lulz, we're gonna set random 0-10 to all of them, and see where they end up
            //Pair = rng.Next(0, 10);
            //Adjacent = rng.Next(0, 10);
            //PseudoAdjacent = rng.Next(0, 10);
            //SameSuit = rng.Next(0, 10);
            //Dora = rng.Next(0, 10);
            //AnyDiscard = rng.Next(0, 10);
            //MyDiscard = rng.Next(0, 10);
            //Meld = rng.Next(0, 10);
            //HonorOrTerminal = rng.Next(0, 10);
        }

        protected override void Logic_WantsTsumo(Tile ReceivedTile, Player.Logic_WantsTsumo_Delegate callback)
        {
            callback.Invoke(true);
        }

        public static Dictionary<Tile, float> GetUndesirability(IList<Tile> MyHand, out int? ShantenNumber)
        {
            var shantenTiles = Hand.GetShantenTiles(MyHand, new List<Meld>());

            ShantenNumber = shantenTiles.Key;

            List<KeyValuePair<Tile, string>> log = new List<KeyValuePair<Tile, string>>();

            //Let's write a dictionary of Tile/undesireability
            var undesiredScore = new Dictionary<Tile, float>();

            if (shantenTiles.Value != null && shantenTiles.Value.Count > 0)
            {
                foreach (var myTile in shantenTiles.Value)
                {
                    float undesirability = 0;
                    //Honors/terminals go away
                    if (myTile.IsHonorOrTerminal())
                    {
                        undesirability += AIShantenPlus.HonorOrTerminal;
                        log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{0}: Honor or Terminal", HonorOrTerminal)));
                    }
                    //Discarded tiles go away
                    //var nDiscarded = allDiscardedTiles.Count(discarded => discarded.Value() == myTile.Value());
                    //undesirability += nDiscarded * AnyDiscard;
                    //if (nDiscarded > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} discarded instances", nDiscarded, nDiscarded * AnyDiscard)));
                    //Exposed tiles in melds go away
                    //if (allExposedMeldTiles.Any(discarded => discarded.Value() == myTile.Value())) undesirability += 1;
                    //var nExposed = allExposedMeldTiles.Count(discarded => discarded.Value() == myTile.Value());
                    //undesirability += nExposed * Meld;
                    //if (nExposed > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} exposed instances", nExposed, nExposed * Meld)));
                    //Tiles I discarded are doubly discardable
                    //var nMyDiscards = this.Discards.Count(discarded => discarded.Value() == myTile.Value());
                    //undesirability += nMyDiscards * MyDiscard;
                    //if (nMyDiscards > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} in my discards", nMyDiscards, nMyDiscards * MyDiscard)));
                    //We try to keep as many doras as we can
                    //var nDoras = this.Game.Doras.Count(dora => dora.Value() == myTile.Value());
                    //undesirability += nDoras * Dora;
                    //if (nDoras > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} doras", nDoras, nDoras * Dora)));
                    //Try to keep em if we have pairs
                    var nPairTiles = MyHand.Count(handTile => handTile.Value() == myTile.Value()) - 1; //we -1 because we have the current tile already
                    undesirability += nPairTiles * Pair;
                    if (nPairTiles > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} identical tiles", nPairTiles, nPairTiles * Pair)));
                    //Try to keep em if we have any adjacent tiles
                    var nAdjacentTiles = MyHand.Count(handTile => handTile.IsNextTile(myTile) || myTile.IsNextTile(handTile));
                    undesirability += nAdjacentTiles * Adjacent;
                    if (nAdjacentTiles > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} adjacent tiles", nAdjacentTiles, nAdjacentTiles * Adjacent)));
                    //Try to keep em if we have any pseudo-adjacent tiles
                    var nPseudoAdjacentTiles = MyHand.Count(handTile => handTile.IsNextNextTile(myTile) || myTile.IsNextNextTile(handTile));
                    undesirability += nPseudoAdjacentTiles * PseudoAdjacent;
                    if (nPseudoAdjacentTiles > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} pseudo-adjacent tiles", nPseudoAdjacentTiles, nPseudoAdjacentTiles * PseudoAdjacent)));
                    //Try to keep em if we have of the same suit for easier melding
                    if (myTile is SuitedTile)
                    {
                        //same, -1 to discount the current tile
                        var nSameSuitTiles = MyHand.Count(handT => (handT is SuitedTile) && (handT as SuitedTile).Suit == (myTile as SuitedTile).Suit) - 1;
                        undesirability += nSameSuitTiles * SameSuit;
                        if (nSameSuitTiles > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} same suit tiles", nSameSuitTiles, nSameSuitTiles * SameSuit)));
                    }
                    undesiredScore.Add(myTile, undesirability);
                }
            }

            return undesiredScore;
        }

        protected override void Logic_ChooseDiscard(Player.Logic_ChooseDiscard_Delegate Callback)
        {
            if (this.Riichi && !this.Ippatsu)
            {
                if (this.LastDraw == null) 
                    throw new InvalidOperationException("Riichi declared and last draw tile is null");

                Callback.Invoke(this.LastDraw);
            }
            else
            {
                var shantenTiles = Hand.GetShantenTiles(this.MyHand, this.ExposedMelds);

                var allDiscardedTiles = new List<Tile>();
                var allExposedMeldTiles = new List<Tile>();
                foreach (var player in this.Game.Players)
                {
                    allDiscardedTiles.AddRange(player.Discards);
                    allExposedMeldTiles.AddRange(player.ExposedMelds.GetAllTiles());
                }

                List<KeyValuePair<Tile, string>> log = new List<KeyValuePair<Tile, string>>();

                if (shantenTiles.Value != null && shantenTiles.Value.Count > 1)
                {
                    Tile chosenDiscard = null;

                    //Let's write a dictionary of Tile/undesireability
                    var undesiredScore = new Dictionary<Tile, float>();
                    foreach (var myTile in shantenTiles.Value)
                    {
                        float undesirability = 0;
                        //Honors/terminals go away
                        if (myTile.IsHonorOrTerminal())
                        {
                            undesirability += HonorOrTerminal;
                            log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{0}: Honor or Terminal",HonorOrTerminal)));
                        }
                        //Discarded tiles go away
                        var nDiscarded = allDiscardedTiles.Count(discarded => discarded.Value() == myTile.Value());
                        undesirability += nDiscarded * AnyDiscard;
                        if (nDiscarded > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} discarded instances",nDiscarded, nDiscarded * AnyDiscard)));
                        //Exposed tiles in melds go away
                        //if (allExposedMeldTiles.Any(discarded => discarded.Value() == myTile.Value())) undesirability += 1;
                        var nExposed = allExposedMeldTiles.Count(discarded => discarded.Value() == myTile.Value());
                        undesirability += nExposed * Meld;
                        if (nExposed > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} exposed instances", nExposed, nExposed * Meld)));
                        //Tiles I discarded are doubly discardable
                        var nMyDiscards = this.Discards.Count(discarded => discarded.Value() == myTile.Value());
                        undesirability += nMyDiscards * MyDiscard;
                        if (nMyDiscards > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} in my discards", nMyDiscards, nMyDiscards * MyDiscard)));
                        //We try to keep as many doras as we can
                        var nDoras = this.Game.Doras.Count(dora => dora.Value() == myTile.Value());
                        undesirability += nDoras * Dora;
                        if (nDoras > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} doras", nDoras, nDoras * Dora)));
                        //Try to keep em if we have pairs
                        var nPairTiles = this.MyHand.Count(handTile => handTile.Value() == myTile.Value()) - 1; //we -1 because we have the current tile already
                        undesirability += nPairTiles * Pair;
                        if (nPairTiles > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} identical tiles", nPairTiles, nPairTiles * Pair)));
                        //Try to keep em if we have any adjacent tiles
                        var nAdjacentTiles = this.MyHand.Count(handTile => handTile.IsNextTile (myTile) || myTile.IsNextTile(handTile));
                        undesirability += nAdjacentTiles * Adjacent;
                        if (nAdjacentTiles > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} adjacent tiles", nAdjacentTiles, nAdjacentTiles * Adjacent)));
                        //Try to keep em if we have any pseudo-adjacent tiles
                        var nPseudoAdjacentTiles = this.MyHand.Count(handTile => handTile.IsNextNextTile(myTile) || myTile.IsNextNextTile(handTile));
                        undesirability += nPseudoAdjacentTiles * PseudoAdjacent;
                        if (nPseudoAdjacentTiles > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} pseudo-adjacent tiles", nPseudoAdjacentTiles, nPseudoAdjacentTiles * PseudoAdjacent)));
                        //Try to keep em if we have of the same suit for easier melding
                        if (myTile is SuitedTile)
                        {
                            //same, -1 to discount the current tile
                            var nSameSuitTiles = this.MyHand.Count(handT => (handT is SuitedTile) && (handT as SuitedTile).Suit == (myTile as SuitedTile).Suit) - 1;
                            undesirability += nSameSuitTiles * SameSuit;
                            if (nSameSuitTiles > 0) log.Add(new KeyValuePair<Tile, string>(myTile, String.Format("{1}: {0} same suit tiles", nSameSuitTiles, nSameSuitTiles * SameSuit)));
                        }
                        undesiredScore.Add(myTile, undesirability);
                    }

                    //Pick least desired one
                    var mostUndesiredScore = undesiredScore.Values.Max();
                    var mostUndesiredTiles = undesiredScore.Where(x => x.Value == mostUndesiredScore).Select(x => x.Key);

                    chosenDiscard = mostUndesiredTiles.GetRandom(rng);

                    this.DiscardSelectionResults(undesiredScore, chosenDiscard, log);

                    Callback.Invoke(chosenDiscard);
                }
                else if (shantenTiles.Value != null && shantenTiles.Value.Count == 1)
                {
                    Callback.Invoke(shantenTiles.Value.First());
                }
                else if (this.LastDraw != null)
                {
                    Callback.Invoke(this.LastDraw);
                }
                else
                {
                    //Something went real bad
                    throw new InvalidOperationException("No tile to discard");
                }
            }
        }

        protected virtual void DiscardSelectionResults(Dictionary<Tile, float> UndesiredScores, Tile ChosenDiscard, List<KeyValuePair<Tile, string>> Log)
        {
        }

        protected override void Logic_WantsMeld(Player Discarder, Tile DiscardedTile, List<Meld> AvailableMelds, Player.Logic_WantsMeld_Delegate callback)
        {
            var honorTriplet = AvailableMelds.FirstOrDefault(meld => meld.Type == MeldType.Triplet && meld.Tiles.All(tile => tile.IsHonor()));
            //debe ser null si no hay ninguno
            callback.Invoke(honorTriplet);
        }

        protected override void Logic_WantsRon(Player Discarder, Tile DiscardedTile, Player.Logic_WantsRon_Delegate callback)
        {
            callback.Invoke(true);
        }

        protected override void Logic_WantsRiichi(List<Tile> Discards, Player.Logic_WantsRiichi_Delegate callback)
        {
            callback.Invoke(true);
        }

        protected override void Logic_Wants_CompleteQuad(Tile Bonus, Player.Logic_Wants_CompleteQuad_Delegate callback)
        {
            callback.Invoke(true);
        }
    }
}
