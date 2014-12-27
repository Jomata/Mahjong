using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    class Program
    {
        static void singlePlayer()
        {

            //var RNG = new Random();
            //var AllTiles = Mahjong.Game.GetAllTiles();
            //var MyHand = new List<Tile>(14);

            //while (MyHand.Count < 14)
            //{
            //    var draw = AllTiles.GetRandom(RNG);
            //    AllTiles.Remove(draw);
            //    MyHand.Add(draw);
            //}

            //for (int x = 0; x < 3; x++)
            //{
            //    for (int y = 0; y < 14; y++)
            //    {
            //        if (AllTiles.Count == 0) break;

            //        var otherDraw = AllTiles.GetRandom(RNG);
            //        AllTiles.Remove(otherDraw);
            //    }
            //}

            //while (!MyHand.IsWinningHand() && AllTiles.Count > 0)
            //{
            //    MyHand.Sort();

            //    Console.WriteLine("Your hand is:");
            //    //Console.WriteLine(MyHand.ToLine());
            //    MyHand.WriteColoredToConsole();
            //    Console.WriteLine();

            //    if (MyHand.IsWinningHand()) continue;

            //    bool validDiscardSelected = false;
            //    while (!validDiscardSelected)
            //    {
            //        Console.WriteLine("{0} tiles left in the wall", AllTiles.Count);

            //        Console.WriteLine("Select a tile to discard");
            //        var discardString = Console.ReadLine();
            //        discardString = discardString.TrimStart('[').TrimEnd(']').ToUpper();
            //        if (MyHand.Any(x => x.ToString().ToUpper() == "[" + discardString + "]"))
            //        {
            //            validDiscardSelected = true;

            //            var discarded = MyHand.First(x => x.ToString().ToUpper() == "[" + discardString + "]");
            //            MyHand.Remove(discarded);

            //            var draw = AllTiles.GetRandom(RNG);
            //            AllTiles.Remove(draw);
            //            MyHand.Add(draw);

            //            Console.Write("Discarded: ");
            //            discarded.WriteColoredToConsole();
            //            Console.WriteLine();

            //            Console.WriteLine("Other players drew tiles");
            //            for (int x = 0; x < 3; x++)
            //            {
            //                if (AllTiles.Count == 0) break;

            //                var otherDraw = AllTiles.GetRandom(RNG);
            //                AllTiles.Remove(otherDraw);
            //            }

            //            Console.Write("Obtained : ");
            //            draw.WriteColoredToConsole();
            //            Console.WriteLine();
            //        }
            //        else
            //        {
            //            Console.WriteLine("Invalid discard selected");
            //        }
            //    }

            //}

            //if (MyHand.IsWinningHand())
            //{
            //    Console.WriteLine("Win!");
            //}
            //else
            //{
            //    Console.WriteLine("Ran out of tiles!");
            //}

            //MyHand.WriteColoredToConsole();
            //Console.WriteLine();
        }

        static void multiPlayer()
        {
            var multiPlayer = new Mahjong.Sync.Game();

            Console.Write("Enter your name: ");
            var PCname = Console.ReadLine();

            multiPlayer.RegisterPlayer(new Mahjong.Sync.Players.PlayerHumanConsole { Name = PCname });
            multiPlayer.RegisterPlayer(new Mahjong.Sync.Players.PlayerAIEcho { Name = "Echo1" });
            multiPlayer.RegisterPlayer(new Mahjong.Sync.Players.PlayerAIEcho { Name = "Echo2" });
            multiPlayer.RegisterPlayer(new Mahjong.Sync.Players.PlayerAIEcho { Name = "Echo3" });

            multiPlayer.Play();
        }

        static void multiplayerAsync()
        {
            var multiPlayer = new Mahjong.Async.Game();

            Console.Write("Enter your name: ");
            var PCname = Console.ReadLine();

            multiPlayer.RegisterPlayer(new Mahjong.Async.Players.HmnConsole { Name = PCname });
            
            multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlus { Name = "Articuno" });
            multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlus { Name = "Zapdos" });
            multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlus { Name = "Moltres" });

            multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AINope { Name = "Nope1" });
            //multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AINope { Name = "Nope2" });
            //multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AINope { Name = "Nope3" });
            
            multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AIFirst { Name = "First1" });
            //multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AIFirst { Name = "First2" });

            multiPlayer.Play();
        }

        static int AIPlayerStatsWith = 20000;
        static Dictionary<Async.Players.AIShantenPlus, int> Scores = new Dictionary<Async.Players.AIShantenPlus, int>();

        static void multiplayerAsyncAIs()
        {
            var multiPlayerAIs = new Mahjong.Async.Game();
            
            //multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlusLog2File { Name = "Shanten1" });
            //multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlusLog2File { Name = "Shanten2" });
            //multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlusLog2File { Name = "Shanten3" });
            //multiPlayer.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlusLog2File { Name = "Shanten4" });

            //multiPlayerAIs.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlusLog2sqlite { Name = "Shanten1" });
            //multiPlayerAIs.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlusLog2sqlite { Name = "Shanten2" });
            //multiPlayerAIs.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlusLog2sqlite { Name = "Shanten3" });
            //multiPlayerAIs.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlusLog2sqlite { Name = "Shanten4" });

            multiPlayerAIs.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlus { Name = "Shanten1" });
            multiPlayerAIs.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlus { Name = "Shanten2" });
            multiPlayerAIs.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlus { Name = "Shanten3" });
            multiPlayerAIs.RegisterPlayer(new Mahjong.Async.Players.AIShantenPlus { Name = "Shanten4" });

            multiPlayerAIs.PlayerWin += new Async.PlayerWinEventHandler(multiPlayerAIs_PlayerWin);

            double variance = 999;

            do
            {
                //we stop when someone is busted (AKA goes into negatives)
                do
                {
                    multiPlayerAIs.Play();
                } while (Scores.Values.All(x => x > 0));
                //get best one
                var bestPlayer = Scores.First();
                foreach (var item in Scores)
                {
                    if (item.Value > bestPlayer.Value) bestPlayer = item;
                }
                //average the others with that one
                foreach (var item in multiPlayerAIs.Players)
                {
                    if (item is Async.Players.AIShantenPlus)
                    {
                        (item as Async.Players.AIShantenPlus).AverageWeightsWith(bestPlayer.Key);
                    }
                }
                //then clear scores
                Scores.Clear();

                variance = multiPlayerAIs.Players.Cast<Async.Players.AIShantenPlus>().Select(x => (double)x.WeightAdjacent).StandardDeviation();
                double tmp = variance;
                tmp = multiPlayerAIs.Players.Cast<Async.Players.AIShantenPlus>().Select(x => (double)x.WeightAnyDiscard).StandardDeviation();
                if (tmp > variance) variance = tmp;
                tmp = multiPlayerAIs.Players.Cast<Async.Players.AIShantenPlus>().Select(x => (double)x.WeightDora).StandardDeviation();
                if (tmp > variance) variance = tmp;
                tmp = multiPlayerAIs.Players.Cast<Async.Players.AIShantenPlus>().Select(x => (double)x.WeightHonorOrTerminal).StandardDeviation();
                if (tmp > variance) variance = tmp;
                tmp = multiPlayerAIs.Players.Cast<Async.Players.AIShantenPlus>().Select(x => (double)x.WeightMeld).StandardDeviation();
                if (tmp > variance) variance = tmp;
                tmp = multiPlayerAIs.Players.Cast<Async.Players.AIShantenPlus>().Select(x => (double)x.WeightMyDiscard).StandardDeviation();
                if (tmp > variance) variance = tmp;
                tmp = multiPlayerAIs.Players.Cast<Async.Players.AIShantenPlus>().Select(x => (double)x.WeightPair).StandardDeviation();
                if (tmp > variance) variance = tmp;
                tmp = multiPlayerAIs.Players.Cast<Async.Players.AIShantenPlus>().Select(x => (double)x.WeightPseudoAdjacent).StandardDeviation();
                if (tmp > variance) variance = tmp;
                tmp = multiPlayerAIs.Players.Cast<Async.Players.AIShantenPlus>().Select(x => (double)x.WeightSameSuit).StandardDeviation();
                if (tmp > variance) variance = tmp;

            } while (variance > 0.01);
        }

        static void multiPlayerAIs_PlayerWin(Async.Player Winner, List<Meld> Concealed, List<Meld> Exposed, Tile WinningTile, Async.Player PlayerHit, IDictionary<IPlayer, int> Payments)
        {
            foreach (var item in Payments)
            {
                if (item.Key is Async.Players.AIShantenPlus)
                {
                    var casted = item.Key as Async.Players.AIShantenPlus;
                    if (!Scores.ContainsKey(casted)) Scores.Add(casted, AIPlayerStatsWith);
                    Scores[casted] += item.Value;
                }
            }

            var thisGame = new EntityFramework.EntityHand();
            foreach (var payment in Payments)
            {
                switch (payment.Key.PlayerWind)
                {
                    case TileHonors.East:
                        thisGame.east_player_score = payment.Value;
                        break;
                    case TileHonors.South:
                        thisGame.south_player_score = payment.Value;
                        break;
                    case TileHonors.West:
                        thisGame.west_player_score = payment.Value;
                        break;
                    case TileHonors.North:
                        thisGame.north_player_score = payment.Value;
                        break;
                }
            }
            foreach (var player in Winner.Game.Players)
            {
                if (player is Mahjong.Async.Players.AIShantenPlus)
                {
                    var entity = (player as Mahjong.Async.Players.AIShantenPlusLog2sqlite).Entity;

                    switch (player.PlayerWind)
                    {
                        case TileHonors.East:
                            if (entity.id.HasValue) thisGame.east_player_id = entity.id.Value;
                            else thisGame.east_player = entity;
                            break;
                        case TileHonors.South:
                            if (entity.id.HasValue) thisGame.south_player_id = entity.id.Value;
                            else thisGame.south_player = entity;
                            break;
                        case TileHonors.West:
                            if (entity.id.HasValue) thisGame.west_player_id = entity.id.Value;
                            else thisGame.west_player = entity;
                            break;
                        case TileHonors.North:
                            if (entity.id.HasValue) thisGame.north_player_id = entity.id.Value;
                            else thisGame.north_player = entity;
                            break;
                    }
                }
            }

            StringBuilder tiles = new StringBuilder();
            
            var concealed = (String.Join(",", Concealed.Select(x => x.ToString()).ToArray()));
            var regex = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(WinningTile.ToString()));
            concealed = regex.Replace(concealed, WinningTile.ToString().Replace('[', '{').Replace(']', '}'), 1);
            tiles.Append(concealed);

            if (Exposed != null && Exposed.Count > 0)
            {
                tiles.Append(" + ");
                tiles.Append(String.Join(",", Exposed.Select(x => x.ToString()).ToArray()));
            }
            thisGame.winning_hand = tiles.ToString();

            bool saved = false;

            do
            {
                try
                {
                    Console.WriteLine("Saving to SQLite...");
                    using (var context = new EntityFramework.MahjongDBContext())
                    {
                        context.Hands.Add(thisGame);
                        context.SaveChanges();
                        saved = true;
                    }
                }
                catch
                {
                    saved = false;
                }
            } while (!saved);
        }

        static void Main(string[] args)
        {
            Console.SetWindowSize(90, 40);
            
            bool continuePlaying = false;
            do
            {
                //singlePlayer();
                multiplayerAsync();
                //multiplayerAsyncAIs();

                bool? continuePlayingInput = null;
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("Play again? (Y/N)");
                    
                    //var userInput = Console.ReadLine();
                    var userInput = "N";

                    switch (userInput.ToUpper())
                    {
                        case "Y": continuePlayingInput = true; break;
                        case "N": continuePlayingInput = false; break;
                    }
                } while (null == continuePlayingInput);
                continuePlaying = continuePlayingInput.Value;
            } while (continuePlaying);

            /*
            Console.WriteLine(DateTime.Now);
            if (Mahjong.Hand.IsWinningHand(Hand.FromString("c1c1c2c2c3c3c4c4c5c5c6c7c8c9")))
                Console.WriteLine("Winning Hand!!!");
            else Console.WriteLine("Not winning hand");
            Console.WriteLine(DateTime.Now);
            if (Mahjong.Hand.IsWinningHand(Hand.FromString("O1O2O2O3O3O3O4O4O5RERERENONO")))
                Console.WriteLine("Winning Hand!!!");
            else Console.WriteLine("Not winning hand");
            Console.WriteLine(DateTime.Now);
             **/

            /*
            int counter = 0;
            do
            {
                Console.WriteLine("Game #{0}", ++counter);
                Console.Title = String.Format("Game #{0}", counter);
            }
            while (!Mahjong.Hand.IsWinningHand(Hand.GetRandomHand()));
            Console.WriteLine("Winning Hand!!!");
             */
            

            //if (Mahjong.Hand.IsWinningHand(Hand.FromString("B1B2B3C4C5C6O1O2O3O1O2O3RERE")))
            //    Console.WriteLine("Winning Hand!!!");
            //else Console.WriteLine("Not winning hand");

            //if (Mahjong.Hand.IsWinningHand(Hand.FromString("B1B1B1B2B3C4C5C6O7O8O9RERERE")))
            //    Console.WriteLine("Winning Hand!!!");
            //else Console.WriteLine("Not winning hand");

            //var foo = Game.GetAllTiles();
            //foo.Sort();
            //foreach (var item in foo)
            //    Console.WriteLine(item.ToString() + " " + item.Value());

            //Console.WriteLine(DateTime.Now);
            //if (Mahjong.Hand.IsWinningHand(Hand.FromString("B1B1B1B2B2B2B3B3B3B4B4B4B5B5")))
            //    Console.WriteLine("Winning Hand!!!");
            //else Console.WriteLine("Not winning hand");

            //Console.WriteLine(DateTime.Now);
            //if (Mahjong.Hand.IsWinningHand(Hand.FromString("C1C2C3C4C5C5C5C5C6C7C8C9C9C9")))
            //    Console.WriteLine("Winning Hand!!!");
            //else Console.WriteLine("Not winning hand");

            //Console.WriteLine(DateTime.Now);
            //if (Mahjong.Hand.IsWinningHand(Hand.FromString("o1o1o2o2o3o3o7o8o8o8o8o9o9o9")))
            //    Console.WriteLine("Winning Hand!!!");
            //else Console.WriteLine("Not winning hand");
            
            /*
            List<Tile> MyHand = new List<Tile>(14);
            MyHand.Add(Tile.Instantiate("B1"));
            MyHand.Add(Tile.Instantiate("B2"));
            MyHand.Add(Tile.Instantiate("B3"));
            MyHand.Add(Tile.Instantiate("C4"));
            MyHand.Add(Tile.Instantiate("C5"));
            MyHand.Add(Tile.Instantiate("C6"));
            MyHand.Add(Tile.Instantiate("O1"));
            MyHand.Add(Tile.Instantiate("O2"));
            MyHand.Add(Tile.Instantiate("O3"));
            MyHand.Add(Tile.Instantiate("O1"));
            MyHand.Add(Tile.Instantiate("O2"));
            MyHand.Add(Tile.Instantiate("O3"));
            MyHand.Add(Tile.Instantiate(TileDragons.Red));
            MyHand.Add(Tile.Instantiate(TileDragons.Red));
            if(Mahjong.Hand.IsWinningHand(MyHand))
                Console.WriteLine("Winning Hand!!!");
            else
                Console.WriteLine("Not winning hand");
            */
            /*
            List<Tile> MyHand = new List<Tile>(14);

            MyHand.Add(new SuitedTile { Suit = TileSuits.Bamboo, Rank = 1 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Bamboo, Rank = 2 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Bamboo, Rank = 3 });

            MyHand.Add(new SuitedTile { Suit = TileSuits.Circle, Rank = 4 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Circle, Rank = 5 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Circle, Rank = 6 });

            MyHand.Add(new SuitedTile { Suit = TileSuits.Character, Rank = 7 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Character, Rank = 8 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Character, Rank = 9 });

            MyHand.Add(new DragonTile { Dragon = TileDragons.Red });
            MyHand.Add(new DragonTile { Dragon = TileDragons.Red });
            MyHand.Add(new DragonTile { Dragon = TileDragons.Red });

            MyHand.Add(new HonorTile { Honor = TileHonors.North });
            MyHand.Add(new HonorTile { Honor = TileHonors.North });

            if(Mahjong.Game.IsWinningHand(MyHand)) Console.WriteLine("Winning Hand!!!");

            MyHand.Clear();

            MyHand.Add(new SuitedTile { Suit = TileSuits.Bamboo, Rank = 1 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Bamboo, Rank = 1 });
            
            MyHand.Add(new SuitedTile { Suit = TileSuits.Circle, Rank = 5 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Circle, Rank = 5 });

            MyHand.Add(new SuitedTile { Suit = TileSuits.Character, Rank = 7 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Character, Rank = 8 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Character, Rank = 9 });
            
            MyHand.Add(new SuitedTile { Suit = TileSuits.Character, Rank = 7 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Character, Rank = 8 });
            MyHand.Add(new SuitedTile { Suit = TileSuits.Character, Rank = 9 });

            MyHand.Add(new DragonTile { Dragon = TileDragons.Red });
            MyHand.Add(new DragonTile { Dragon = TileDragons.Red });

            MyHand.Add(new HonorTile { Honor = TileHonors.North });
            MyHand.Add(new HonorTile { Honor = TileHonors.North });

            if (Mahjong.Game.IsWinningHand(MyHand)) Console.WriteLine("Winning Hand!!!");
            */

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
