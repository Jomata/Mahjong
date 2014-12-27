using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong.Async.Players
{
    public class AIShantenPlusLog2File : AIShantenPlus
    {
        private string LogFilePath
        {
            get
            {
                return String.Format("{0}.discards.log", this.Name);
            }
        }

        protected override void DiscardSelectionResults(Dictionary<Tile, float> UndesiredScores, Tile ChosenDiscard, List<KeyValuePair<Tile, string>> Log)
        {
#if DEBUG
            using (var file = new System.IO.StreamWriter(this.LogFilePath, true))
            {
                file.Write("Tiles: " + String.Join("", this.MyHand.Where(x => x != this.LastDraw).OrderBy(x => x.Value()).Select(x => x.ToString()).ToArray()));
                file.WriteLine("+" + this.LastDraw);

                foreach (var line in UndesiredScores.OrderBy(x => x.Value))
                {
                    var details = String.Join(", ", Log.Where(x => x.Key == line.Key).Select(x => x.Value).ToArray());
                    file.WriteLine(String.Format("{0}: {1} ({2})", line.Key, line.Value, details));
                }
                file.WriteLine(String.Format("Chosen Discard: {0}", ChosenDiscard));
                file.WriteLine();
            }
#endif
        }

        protected override void OnGameSet(Game NewGame)
        {
            NewGame.GameStart += new EventHandler(NewGame_GameStart);
            NewGame.NoTilesLeft += new EventHandler(NewGame_NoTilesLeft);
            NewGame.PlayerWin += new PlayerWinEventHandler(NewGame_PlayerWin);
            NewGame.PlayerRiichi += new PlayerRiichi(NewGame_PlayerRiichi);

            base.OnGameSet(NewGame);
        }

        void NewGame_PlayerRiichi(Player sender)
        {
            using (var file = new System.IO.StreamWriter(LogFilePath, true))
            {
                file.WriteLine(String.Format("{0} declared riichi", sender.Name));
            }
        }

        void NewGame_PlayerWin(Player Winner, List<Meld> Concealed, List<Meld> Exposed, Tile WinningTile, Player PlayerHit, IDictionary<IPlayer, int> Payments)
        {
            using (var file = new System.IO.StreamWriter(LogFilePath, true))
            {
                file.WriteLine(String.Format("Player {0} wins.", Winner.Name));

                if (PlayerHit == null)
                {
                    file.WriteLine(String.Format("Tsumo with {0}", WinningTile));
                }
                else
                {
                    file.WriteLine(String.Format("Ron on {0}'s {1}", PlayerHit.Name, WinningTile));
                }

                file.Write(String.Join(",", Concealed.Select(x => x.ToString()).ToArray()));
                if (Exposed != null && Exposed.Count > 0)
                {
                    file.Write(" + ");
                    file.Write(String.Join(",", Exposed.Select(x => x.ToString()).ToArray()));
                }
                file.WriteLine();

                foreach (var payer in Payments)
                {
                    file.WriteLine(String.Format("> {0} {1:+#;-#;---}", payer.Key.Name, payer.Value));
                }

                file.WriteLine();
            }
        }

        void NewGame_NoTilesLeft(object sender, EventArgs e)
        {
            using (var file = new System.IO.StreamWriter(LogFilePath, true))
            {
                file.WriteLine(String.Format("Draw, no tiles left"));
                file.WriteLine();
            }
        }

        void NewGame_GameStart(object sender, EventArgs e)
        {
            using (var file = new System.IO.StreamWriter(LogFilePath, true))
            {
                file.WriteLine();
                file.WriteLine(String.Format("==== Game Start ===="));
                file.WriteLine("> Pair: {0}", Pair);
                file.WriteLine("> Adjacent: {0}", Adjacent);
                file.WriteLine("> PseudoAdjacent: {0}", PseudoAdjacent);
                file.WriteLine("> SameSuit: {0}", SameSuit);
                file.WriteLine("> Dora: {0}", Dora);
                file.WriteLine("> AnyDiscard: {0}", AnyDiscard);
                file.WriteLine("> MyDiscard: {0}", MyDiscard);
                file.WriteLine("> Meld: {0}", Meld);
                file.WriteLine("> HonorOrTerminal: {0}", HonorOrTerminal);
                file.WriteLine();
            }
        }
    }
}
