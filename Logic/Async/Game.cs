using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong.Async
{
    public enum PlayerDrawResponse { Discard, Tsumo, CompleteQuad };

    public delegate void PlayerDraw(Player sender, PlayerDrawResponse response);
    public delegate void PlayerRiichi(Player sender);
    public delegate void PlayerWantsRon(Player sender, bool ron);
    public delegate void PlayerWantsMeld(Player sender, Meld meld);
    public delegate void PlayerDiscardSelected(Player sender, Tile discarded);

    public delegate void DiscardInterceptedEventHandler(Player InterceptedPlayer, Tile InterceptedTile, Player IntercepterPlayer, Meld RevealedMeld);
    public delegate void DiscardCompletedEventHandler(Player Discarder, Tile Discarded);
    public delegate void PlayerWinEventHandler(Player Winner, List<Meld> Concealed, List<Meld> Exposed, Tile WinningTile, Player PlayerHit, IDictionary<IPlayer, int> Payments);

    public class Game : IGame
    {
        /**
         *  Dora tile is not the revealed one, but the following one, wraping around from first to last
         *      Ex: DWhite > DGreen > DRed > DWhite
         *  The number of dora indicators increases in the following manner:
         *      Each time a player calls a quad, the next adjacent dora indicator tile is flipped
         *      The indicator is flipped immediately after the quad is called, and after that the player draws a supplemental tile for their hand from the back end of the dead wall.
         *      The number of indicators increases in that direction, which becomes five if a single player calls four quads, and that is the largest possible number on the upper stack
         *      When a player goes out with a declaration of rīchi (ready hand), the tiles underneath the dora indicators are flipped after the win and become additional dora indicators, making their succeeding tiles also counted as dora which are called ura-dora (裏ドラ, underneath dora).
        */

        public event EventHandler GameStart;
        protected virtual void OnGameStart()
        {
            if (this.GameStart != null)
                this.GameStart.Invoke(this, EventArgs.Empty);
        }

        public event DiscardInterceptedEventHandler DiscardIntercepted;
        protected virtual void OnDiscardIntercepted(Player InterceptedPlayer, Tile InterceptedTile, Player IntercepterPlayer, Meld RevealedMeld)
        {
            if (this.DiscardIntercepted != null)
                this.DiscardIntercepted.Invoke(InterceptedPlayer, InterceptedTile.Clone() as Tile, IntercepterPlayer, new Meld(RevealedMeld.Tiles.Clone().ToList()));
        }

        public event DiscardCompletedEventHandler DiscardCompleted;
        protected virtual void OnDiscardCompleted(Player Discarder, Tile Discarded)
        {
            if (this.DiscardCompleted != null)
                this.DiscardCompleted.Invoke(Discarder, Discarded.Clone() as Tile);
        }

        public event PlayerWinEventHandler PlayerWin;
        protected virtual void OnPlayerWin(Player Winner, List<Meld> Concealed, List<Meld> Exposed, Tile WinningTile, Player PlayerHit)
        {
            var Payments = ScorerJapanese.GetPayments(Concealed, Exposed, this, Winner);

            if (this.PlayerWin != null)
                this.PlayerWin.Invoke(Winner, Concealed/*.Clone().ToList()*/, Exposed/*.Clone().ToList()*/, WinningTile/*.Clone() as Tile*/, PlayerHit, Payments);
        }

        public event PlayerRiichi PlayerRiichi;
        protected virtual void OnPlayerRiichi(Player Player)
        {
            if (this.PlayerRiichi != null)
                this.PlayerRiichi.Invoke(Player);
        }

        public event EventHandler NoTilesLeft;
        protected virtual void OnNoTilesLeft()
        {
            if (this.NoTilesLeft != null)
                this.NoTilesLeft.Invoke(this, new EventArgs());
        }

        private void ConsoleLog(string log)
        {
            Console.WriteLine(log);
        }

        private Stack<Tile> Tiles { get; set; }
        private Stack<Tile> KanReplacements { get; set; }

        public Player CurrentPlayer { get; private set; }
        public List<Player> Players {get;private set;}
        public Player GetPlayerAfter(Player Current)
        {
            return this.Players[(this.Players.IndexOf(Current) + 1) % this.Players.Count];
        }
        public TileHonors GameWind { get; private set; }

        public TileHonors GetPlayerWind(Player player)
        {
            var winds = Enum.GetValues(typeof(TileHonors));
            var iP = this.Players.IndexOf(player);
            var iW = 0;
            foreach (var wind in winds)
            {
                if(iP == iW) return (TileHonors)wind;
                iW++;
            }

            return 0;
        }

        public int TilesLeft
        {
            get
            {
                return this.Tiles.Count;
            }
        }

        public Game()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.Tiles = new Stack<Tile>();
            this.Players = new List<Player>();
        }

        public void Play()
        {
            if (this.Players.Count != 4) throw new InvalidOperationException("Can not start game without 4 players");

            this.ConsoleLog("Game starting");
            var rng = new Random();
            this.GameWind = Enum.GetValues(typeof(TileHonors)).Cast<TileHonors>().ElementAt(rng.Next(0, 4));
            
            this.RevealedDorasCount = 1;
            this.UradorasRevealed = false;

            var allTiles = Tile.GetAllTiles();
            allTiles.Sort((A, B) => 
            {
                if (A == B) 
                    return 0;
                if (A == null && B == null)
                    return 0;
                if (A != null && B != null && A.Value() == B.Value())
                    return 0;

                return Guid.NewGuid().GetHashCode(); 
            });
            this.Tiles = new Stack<Tile>(allTiles);

            //Set apart doras and uradoras
            this._DoraIndicators = new List<Tile>();
            this._UradoraIndicators = new List<Tile>();
            for (int i = 0; i < 5; i++)
            {
                this._DoraIndicators.Add(this.Tiles.Pop());
                this._UradoraIndicators.Add(this.Tiles.Pop());
            }

            //Dead wall
            this.KanReplacements = new Stack<Tile>(4);
            for (int i = 0; i < 4; i++)
            {
                this.KanReplacements.Push(this.Tiles.Pop());
            }

            this.OnGameStart();

            //Initiate players, give 13 tiles each
            foreach (var player in this.Players)
            {
                for (int i = 0; i < 13; i++)
                {
                    player.ReceiveTile(this.Tiles.Pop(), this.PlayerInitialDraw_Callback);
                }
            }
            //Play turn on player 1
            this.Turn(this.Players.First());
        }

        public void RegisterPlayer(Player player)
        {
            if (this.Players.Count < 4)
            {
                this.Players.Add(player);
                player.Game = this;
            }
        }

        private void Turn(Player player)
        {
            if (this.Tiles.Count == 0)
            {
                this.ConsoleLog("Ran out of tiles :(");
                this.OnNoTilesLeft();
                return;
            }
            else
            {
                this.ConsoleLog(string.Format("{0}'s turn", player.Name));

                this.CurrentPlayer = player;
                //pop tile form wall
                //give tile to Player
                player.ReceiveTile(this.Tiles.Pop(), this.PlayerDraw_Callback);
                //check tsumo
            }
        }

        private void NextTurn()
        {
            this.Turn(this.GetPlayerAfter(this.CurrentPlayer));
        }

        private void PlayerInitialDraw_Callback(Player sender, PlayerDrawResponse reaction)
        {
            //this.ConsoleLog(string.Format("{0}'s initial draw callback", sender.Name));
            //~do nothing~
        }

        private void PlayerDraw_Callback(Player sender, PlayerDrawResponse reaction)
        {
            this.ConsoleLog(string.Format("{0}'s draw callback", sender.Name));

            switch (reaction)
            {
                case PlayerDrawResponse.Discard:
                    sender.DiscardTile(this.PlayerDiscardSelected_Callback);
                    break;
                case PlayerDrawResponse.Tsumo:
                    this.ConsoleLog("Tsumo!");
                this.OnPlayerWin(sender, sender.RevealedHand, sender.ExposedMelds, sender.WinningTile, null);
                    break;
                case PlayerDrawResponse.CompleteQuad:
                    //Player needs to draw another tile to complete
                    sender.ReceiveTile(this.Tiles.Pop(), this.PlayerDraw_Callback);
                    break;
                default:
                    break;
            }
        }

        //Stack<Player> discarders;
        //Stack<Tile> discardedTiles;
        Player currentDiscardPlayer;
        Tile currentDiscardTile;
        public Tile CurrentDiscardTile { get { return currentDiscardTile; } }
        private void PlayerDiscardSelected_Callback(Player sender, Tile discarded)
        {
            this.ConsoleLog(string.Format("{0}'s discard selected callback", sender.Name));

            this.currentDiscardPlayer = sender;
            this.currentDiscardTile = discarded;
            //this.discarders.Push(sender);
            //this.discardedTiles.Push(discarded);

            if (sender.Riichi && sender.Ippatsu) //he just declared riichi
            {
                this.OnPlayerRiichi(sender);
            }

            //for the player to the right of the discarder, we check if they can/want to ron
            this.GetPlayerAfter(sender).WantsRon(this.currentDiscardPlayer, this.currentDiscardTile, this.PlayerWantsRon_Callback);
        }

        private void PlayerKanDraw_Callback(Player sender, bool ron)
        {
            this.ConsoleLog(string.Format("{0} kan draw callback", sender.Name));
            //if player rons, game over, notify the others
            if (ron)
            {
                this.ConsoleLog("Ron via kan replacement draw!");
                this.OnPlayerWin(sender, sender.RevealedHand, sender.ExposedMelds, sender.WinningTile, this.currentDiscardPlayer);
            }
            //if player does not ron, then he discards
            else
            {
                sender.DiscardTile(this.PlayerDiscardSelected_Callback);
            }
        }

        private void PlayerWantsRon_Callback(Player sender, bool ron)
        {
            this.ConsoleLog(string.Format("{0} wants ron callback", sender.Name));

            //if player rons, game over, notify the others
            if (ron)
            {
                this.ConsoleLog("Ron!");
                this.OnPlayerWin(sender, sender.RevealedHand, sender.ExposedMelds, sender.WinningTile, this.currentDiscardPlayer);
            }
            //if player does not ron, check for a meld
            else
            {
                //sender.WantsMeld(this.discarders.Peek(), this.discardedTiles.Peek(), this.PlayerWantsMeld_Callback);
                sender.WantsMeld(this.currentDiscardPlayer, this.currentDiscardTile, this.PlayerWantsMeld_Callback);
            }
        }

        //Idea: Keep a stack of the discarding players, so I can go back
        private void PlayerWantsMeld_Callback(Player sender, Meld meld)
        {
            this.ConsoleLog(string.Format("{0} wants meld callback", sender.Name));

            //if meld is null, check next player, unless this was the last player
            if (meld == null)
            {
                var nextPlayer = this.GetPlayerAfter(sender);
                //we've cycled through, discard goes through
                //if (nextPlayer == this.discarders.Peek())
                if (nextPlayer == this.currentDiscardPlayer)
                {
                    //add to player's discard pile
                    this.currentDiscardPlayer.Discards.Add(this.currentDiscardTile);
                    this.OnDiscardCompleted(this.currentDiscardPlayer, this.currentDiscardTile);
                    //clean up the discard stacks
                    this.currentDiscardPlayer = null;
                    this.currentDiscardTile = null;
                    //next turn
                    this.NextTurn();
                }
                else
                {
                    //nextPlayer.WantsTsumo(this.discarders.Peek(), this.discardedTiles.Peek(), this.PlayerWantsRon_Callback);
                    nextPlayer.WantsRon(this.currentDiscardPlayer, this.currentDiscardTile, this.PlayerWantsRon_Callback);
                }
            }
            //In case of Kan, you first draw a replacement tile from the dead wall and then discard
            else if (meld.IsKan())
            {
                this.RevealedDorasCount++;
                this.OnDiscardIntercepted(this.currentDiscardPlayer, this.currentDiscardTile, sender, meld);
                
                var rinshan = this.KanReplacements.Pop();
                var backfill = this.Tiles.Pop();
                this.KanReplacements.Push(backfill);
                sender.ReceiveKanReplacement(rinshan, this.currentDiscardPlayer, this.PlayerKanDraw_Callback);
            }
            //In other melds just, ask player to discard
            else
            {

                this.OnDiscardIntercepted(this.currentDiscardPlayer, this.currentDiscardTile, sender, meld);
                sender.DiscardTile(this.PlayerDiscardSelected_Callback);
            }
        }

        private int RevealedDorasCount = 1;
        private bool UradorasRevealed = false;

        private List<Tile> _DoraIndicators = new List<Tile>();
        public List<Tile> DoraIndicators
        {
            get { return this._DoraIndicators.Take(this.RevealedDorasCount).ToList(); }
            //private set { this._DoraIndicators = value; }
        }

        private List<Tile> _UradoraIndicators = new List<Tile>();
        public List<Tile> UradoraIndicators
        {
            get
            {
                if (!this.UradorasRevealed) return new List<Tile>();
                else return _UradoraIndicators.Take(this.RevealedDorasCount).ToList(); 
            }
            //private set { _UradoraIndicators = value; }
        }

        public List<Tile> Uradoras
        {
            get
            {
                if (!this.UradorasRevealed) return new List<Tile>();

                var indicators = this._UradoraIndicators;
                var doras = new List<Tile>(indicators.Count);
                foreach (var indicator in indicators)
                {
                    doras.Add(indicator.IsDoraIndicatorOf());
                }
                return doras; 
            }
        }

        public List<Tile> Doras
        {
            get
            {
                var indicators = this.DoraIndicators;
                var doras = new List<Tile>(indicators.Count);
                foreach (var indicator in indicators)
                {
                    doras.Add(indicator.IsDoraIndicatorOf());
                }
                return doras; 
            }
        }


        List<IPlayer> IGame.Players
        {
            get { return this.Players.Cast<IPlayer>().ToList(); }
        }

        IPlayer IGame.CurrentPlayer
        {
            get { return this.CurrentPlayer; }
        }
    }
}
