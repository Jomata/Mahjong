using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mahjong.Sync;

namespace Mahjong.Sync.Players
{
    public class PlayerHuman : Player
    {
        protected override Tile Logic_ChooseDiscard()
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
            return discarded;
        }

        protected override bool Logic_WantsDiscard(Tile Tile,Player Discarder)
        {
            this.DrawBoard();

            Console.Write("Player {0} discarded: ", Discarder.Name);
            Tile.WriteColoredToConsole();
            Console.WriteLine();

            string question = "Do you want that tile? (Y/N) ";
            Console.Write(question);

            while (true)
            {
                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case "Y": return true;
                    case "N": return false;
                    default: Console.WriteLine(question); break;
                }
            }
        }

        private void DrawBoard()
        {
            Console.Title = "Mahjong!";

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            Console.WriteLine("{0} tiles left in the wall", this.Game.TilesLeft);

            Console.WriteLine("===Discards===");

            int longestPlayerNameLength = this.Game.Players.Max(x => x.Name.Length);
            string discardLabelFormat = "{0,-" + longestPlayerNameLength + "}: ";
            foreach (var player in this.Game.Players)
            {
                Console.Write(discardLabelFormat, player.Name);
                player.Discards.WriteColoredToConsole();
                Console.WriteLine();
            }

            Console.WriteLine("=== Hands ===");

            var exposedTilesSeparator = " |Exposed:";
            var unknownTilePlaceholder = "[??]";

            foreach (var player in this.Game.Players)
            {
                Console.Write(discardLabelFormat, player.Name);
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
        }

        protected override List<Tile> Logic_ChooseExposedTilesAfterIntercept(List<List<Tile>> Melds)
        {
            bool validMeldSelected = false;

            while (!validMeldSelected)
            {
                Console.WriteLine("Choose your meld:");

                int x = 0;
                foreach (var Meld in Melds)
                {
                    x++;
                    Console.Write("{0}: ", x);
                    Meld.Sort();
                    Meld.WriteColoredToConsole();
                    Console.WriteLine();
                }
                var input = Console.ReadLine();
                int inputInt;
                if (int.TryParse(input, out inputInt))
                {
                    if (inputInt > 0 && inputInt <= Melds.Count) return Melds[inputInt-1];
                }
            }

            return null;
        }

        protected override bool Logic_WantsRon(Tile Tile, Player Discarder)
        {
            while (true)
            {
                Console.Write("Do you want to ron ");
                Tile.WriteColoredToConsole();
                Console.Write(" on {0}? (Y/N) ", Discarder.Name);

                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case "Y": return true;
                    case "N": return false;
                }
            }
        }
    }
}
