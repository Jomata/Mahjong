using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong.Async.Players
{
    public class HmnConsole : Mahjong.Async.Player
    {
        //To keep using the color format thingie, we'll format the messages with [] as placeholders for the tiles, and store the tiles in another list
        private const string MessageTilePlaceholder = "[]";
        List<string> MessagesStrings { get; set; }
        List<List<Tile>> MessagesTiles {get;set;}

        public HmnConsole()
        {
            this.MessagesStrings = new List<string>();
            this.MessagesTiles = new List<List<Tile>>();
        }

        protected override void OnGameSet(Game NewGame)
        {
            NewGame.DiscardIntercepted += new DiscardInterceptedEventHandler(Game_DiscardIntercepted);
            NewGame.DiscardCompleted += new DiscardCompletedEventHandler(Game_DiscardCompleted);
            NewGame.PlayerWin += new PlayerWinEventHandler(Game_PlayerWin);
            NewGame.PlayerRiichi += new PlayerRiichi(Game_PlayerRiichi);

            base.OnGameSet(NewGame);
        }

        void Game_PlayerRiichi(Player sender)
        {
            this.MessagesStrings.Add(String.Format("{0} declares riichi",sender.Name));
            this.MessagesTiles.Add(new List<Tile>());
        }

        void Game_PlayerWin(Player Winner, List<Meld> Concealed, List<Meld> Exposed, Tile WinningTile, Player PlayerHit, IDictionary<IPlayer,int> Payments)
        {
            this.MessagesStrings.Add(String.Format("Player {0} wins!", Winner.Name));
            this.MessagesTiles.Add(new List<Tile>());

            if (PlayerHit == null)
            {
                this.MessagesStrings.Add("Tsumo with " + MessageTilePlaceholder);
                this.MessagesTiles.Add(new List<Tile>(1) { WinningTile });
            }
            else
            {
                this.MessagesStrings.Add(string.Format("Ron on {0}'s {1}", PlayerHit.Name, MessageTilePlaceholder));
                this.MessagesTiles.Add(new List<Tile>(1) { WinningTile });
            }

            string winningHand = "Winning Hand: ";
            foreach(var meld in Concealed)
            foreach (var tile in meld.Tiles)
            {
                winningHand += MessageTilePlaceholder;
            }
            this.MessagesTiles.Add(Concealed.GetAllTiles());

            if (Exposed != null && Exposed.Count > 0)
            {
                winningHand += " + ";
                for (int x = 0; x < Exposed.Count; x++)
                {
                    if (x > 0) winningHand += " | ";

                    foreach (var tile in Exposed[x].Tiles)
                    {
                        winningHand += MessageTilePlaceholder;
                        this.MessagesTiles.Last().Add(tile);
                    }
                }
            }
            this.MessagesStrings.Add(winningHand);

            var scorer = new Mahjong.ScorerJapanese(Concealed, Exposed, this.Game, Winner);

            var hans = scorer.GetHan();
            foreach (var han in hans)
            {
                //this.MessagesStrings.Add(String.Format("> {0} ({1}): {2} han{3}", han.Name, han.Description, han.Value, han.Value > 1 ? "s":""));
                this.MessagesStrings.Add(String.Format("> {0}",han));
                this.MessagesTiles.Add(new List<Tile>());
            }
            var fus = scorer.GetFu();
            foreach (var fu in fus)
            {
                //this.MessagesStrings.Add(String.Format(">> {0} ({1}): {2} fu{3}", fu.Name, fu.Description, fu.Value, fu.Value > 1 ? "s" : ""));
                this.MessagesStrings.Add(String.Format(">> {0}",fu));
                this.MessagesTiles.Add(new List<Tile>());
            }

            foreach (var payer in Payments)
            {
                if (payer.Value < 0)
                {
                    this.MessagesStrings.Add(String.Format("{0} pays {1}", payer.Key.Name, -1 * payer.Value));
                }
                else if (payer.Value > 0)
                {
                    this.MessagesStrings.Add(String.Format("{0} gets {1}", payer.Key.Name, payer.Value));
                }
                this.MessagesTiles.Add(new List<Tile>());
            }

            this.DrawBoard();
        }

        void Game_DiscardCompleted(Player Discarder, Tile Discarded)
        {
            this.MessagesStrings.Add(string.Format("{0} discarded a {1}", Discarder.Name, MessageTilePlaceholder));
            this.MessagesTiles.Add(new List<Tile>(){Discarded});
        }

        void Game_DiscardIntercepted(Player InterceptedPlayer, Tile InterceptedTile, Player IntercepterPlayer, Meld RevealedMeld)
        {
            var SB = new StringBuilder();
            SB.AppendFormat("{0}'s {1} was intercepted by {2} for a ", InterceptedPlayer.Name, MessageTilePlaceholder, IntercepterPlayer.Name);

            foreach (var item in RevealedMeld.Tiles)
            {
                SB.Append(MessageTilePlaceholder);		        
            }

            this.MessagesStrings.Add(SB.ToString());
            RevealedMeld.Tiles.Insert(0,InterceptedTile);
            this.MessagesTiles.Add(RevealedMeld.Tiles);
        }

        protected override void Logic_WantsTsumo(Tile ReceivedTile, Player.Logic_WantsTsumo_Delegate callback)
        {
            DrawBoard();

            bool? WantsTsumo = null;
            while (WantsTsumo == null)
            {
                Console.Write("Do you want to tsumo on ");
                ReceivedTile.WriteColoredToConsole();
                Console.Write("? (Y/N)");

                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case "Y": WantsTsumo = true; break;
                    case "N": WantsTsumo = false; break;
                    default: Console.WriteLine("Y/N"); break;
                }
            }
            callback.Invoke(WantsTsumo.Value);
        }

        protected override void Logic_ChooseDiscard(Player.Logic_ChooseDiscard_Delegate Callback)
        {
            this.DrawBoard();

            if (this.LastDraw != null)
            {
                Console.Write("Obtained : ");
                this.LastDraw.WriteColoredToConsole();
                Console.WriteLine();
            }

            Tile discarded = null;
            while (discarded == null)
            {
                Console.WriteLine("Select a tile to discard");
                var discardString = Console.ReadLine();
                discardString = discardString.TrimStart('[').TrimEnd(']').ToUpper();
                if (MyHand.Any(x => x.ToString().ToUpper() == "[" + discardString + "]"))
                {
                    discarded = MyHand.First(x => x.ToString().ToUpper() == "[" + discardString + "]");

                    //Console.Write("Discarded: ");
                    //discarded.WriteColoredToConsole();
                    //Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Invalid discard selected");
                }
            }

            Callback.Invoke(discarded);
        }

        protected override void Logic_WantsMeld(Player Discarder, Tile DiscardedTile, List<Meld> AvailableMelds, Player.Logic_WantsMeld_Delegate callback)
        {
            bool? wantsMeld = null;

            while (wantsMeld == null)
            {
                DrawBoard();
                Console.Write("Do you want to intercept {0}'s ",Discarder.Name);
                DiscardedTile.WriteColoredToConsole();
                Console.WriteLine("? (Y/N)");

                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case "Y": wantsMeld = true; break;
                    case "N": wantsMeld = false; break;
                }
            }


            if (wantsMeld.Value)
            {
                if (AvailableMelds.Count == 1)
                {
                    callback.Invoke(AvailableMelds.First());
                }
                else
                {
                    Meld selectedMeld = null;

                    while (selectedMeld == null)
                    {
                        DrawBoard();
                        Console.WriteLine("Select your meld");

                        int x = 0;
                        foreach (var Meld in AvailableMelds)
                        {
                            x++;
                            Console.Write("{0}: ", x);
                            Meld.Tiles.Sort();
                            Meld.Tiles.WriteColoredToConsole();
                            Console.WriteLine();
                        }
                        var input = Console.ReadLine();
                        int inputInt;
                        if (int.TryParse(input, out inputInt) && inputInt > 0 && inputInt <= AvailableMelds.Count)
                        {
                            selectedMeld = AvailableMelds[inputInt - 1];
                        }
                    }

                    callback.Invoke(selectedMeld);
                }
            }
            else
            {
                callback.Invoke(null);
            }
        }

        protected override void Logic_WantsRon(Player Discarder, Tile DiscardedTile, Player.Logic_WantsRon_Delegate callback)
        {
            DrawBoard();

            bool? WantsRon = null;
            while (WantsRon == null)
            {
                Console.Write("Do you want to ron {0} ",Discarder.Name);
                Console.Write(" with a ");
                DiscardedTile.WriteColoredToConsole();
                Console.Write("? (Y/N) ");

                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case "Y": WantsRon = true; break;
                    case "N": WantsRon = false; break;
                    default: Console.WriteLine("Y/N"); break;
                }
            }
            callback.Invoke(WantsRon.Value);
        }

        private const int MaxHistoryLogLines = 15;
        private void DrawBoard()
        {
            Console.Title = "Mahjong!";

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            var shantenInfo = Hand.GetShantenTiles(MyHand, ExposedMelds);
            if (shantenInfo.Key >= 0)
            {
                if (shantenInfo.Key == 0) Console.Write("Tenpai.");
                else Console.Write("Shanten {0}.", shantenInfo.Key);
                if (shantenInfo.Value != null && shantenInfo.Value.Count > 0)
                {
                    Console.Write(" Unused tiles: ");
                    shantenInfo.Value.Sort();
                    shantenInfo.Value.WriteColoredToConsole();
                }
                Console.WriteLine();
            }

            Console.WriteLine("{0} tiles left in the wall", this.Game.TilesLeft);
            Console.WriteLine("Game wind: {0}", this.Game.GameWind.ToString());
            Console.WriteLine("My wind: {0}", this.PlayerWind.ToString());
            Console.WriteLine("{0}'s turn", this.Game.CurrentPlayer.Name);
            Console.Write("Dora indicators: ");
            foreach (var dora in Game.DoraIndicators) { dora.WriteColoredToConsole(); }
            Console.Write(" (Actual dora: ");
            foreach (var dora in Game.Doras) { dora.WriteColoredToConsole(); }
            Console.WriteLine(")");
            if (Game.UradoraIndicators.Count > 0)
            {
                Console.Write("Uradora indicators: ");
                foreach (var dora in Game.UradoraIndicators) { dora.WriteColoredToConsole(); }
                Console.Write(" (Actual dora: ");
                foreach (var dora in Game.Uradoras) { dora.WriteColoredToConsole(); }
                Console.WriteLine(")");
            }

            Console.WriteLine("===Discards===");

            int longestPlayerNameLength = this.Game.Players.Max(x => x.Name.Length);
            string discardLabelFormat = "{0,-" + longestPlayerNameLength + "}: ";
            foreach (var player in this.Game.Players)
            {
                Console.Write(discardLabelFormat, player.Name);
                player.Discards.WriteColoredToConsole();
                Console.WriteLine();
            }

            Console.WriteLine("=== Hands === (* means riichi)");

            var exposedTilesSeparator = " |Exposed:";
            var unknownTilePlaceholder = "[??]";

            foreach (var player in this.Game.Players)
            {
                Console.Write(discardLabelFormat, player.Name);
                if (player.Riichi) Console.Write("*");

                if (player == this)
                {
                    MyHand.Sort();
                    MyHand.WriteColoredToConsole();

                    //for (int x = 0; x < 14 - HandTilesCount; x++)
                    //{ Console.Write("    "); }
                    if (ExposedMelds.Count > 0)
                    {
                        Console.Write(exposedTilesSeparator);
                        ExposedMelds.WriteColoredToConsole();
                    }
                }
                else
                {
                    for (int x = 0; x < player.HandTilesCount; x++)
                    { Console.Write(unknownTilePlaceholder); }

                    if (player.ExposedMelds.Count > 0)
                    {
                        Console.Write(exposedTilesSeparator);
                        player.ExposedMelds.WriteColoredToConsole();
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine();

            if (this.MessagesStrings != null && this.MessagesStrings.Count > 0)
            {
                this.MessagesStrings = this.MessagesStrings.Last(MaxHistoryLogLines).ToList();
                this.MessagesTiles = this.MessagesTiles.Last(MaxHistoryLogLines).ToList();

                Console.WriteLine("=== Last {0} movements ===", MaxHistoryLogLines);

                for (int x = 0; x < this.MessagesStrings.Count; x++)
                {
                    var msgStr = this.MessagesStrings[x];
                    var msgTiles = this.MessagesTiles[x];

                    var msgTokens = msgStr.Split(new string[] {MessageTilePlaceholder},StringSplitOptions.None);
                    for (int y = 0; y < msgTokens.Count(); y++)
                    {
                        Console.Write(msgTokens[y]);
                        if (msgTiles.Count > y)
                            msgTiles[y].WriteColoredToConsole();
                    }
                    Console.WriteLine();
                }

                /*foreach (var item in this.MessagesStrings)
                {
                    Console.WriteLine(item);
                }*/
                Console.WriteLine("==== === ====");
            }
        }

        protected override void Logic_WantsRiichi(List<Tile> Discards, Player.Logic_WantsRiichi_Delegate callback)
        {
            DrawBoard();

            bool? WantsRiichi = null;
            while (WantsRiichi == null)
            {
                Console.Write("You can declare riichi if you discard ");
                int i = 0;
                foreach (var discard in Discards)
                {
                    if (i > 0) Console.Write(" or ");
                    discard.WriteColoredToConsole();
                    i++;
                }
                Console.WriteLine(". Do you want to? (Y/N)");
                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case "Y": WantsRiichi = true; break;
                    case "N": WantsRiichi = false; break;
                    default: Console.WriteLine("Y/N"); break;
                }
            }
            callback.Invoke(WantsRiichi.Value);
        }

        protected override void Logic_Wants_CompleteQuad(Tile Bonus, Player.Logic_Wants_CompleteQuad_Delegate callback)
        {
            DrawBoard();

            bool? WantsQuad = null;
            while (WantsQuad == null)
            {
                Console.Write("Do you want to complete your kan with ");
                Bonus.WriteColoredToConsole();
                Console.WriteLine("? (Y/N)");
                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case "Y": WantsQuad = true; break;
                    case "N": WantsQuad = false; break;
                    default: Console.WriteLine("Y/N"); break;
                }
            }
            callback.Invoke(WantsQuad.Value);
        }
    }
}
