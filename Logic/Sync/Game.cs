using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using System.Collections.ObjectModel;

namespace Mahjong.Sync
{
    public class Game
    {
        List<Tile> Tiles = new List<Tile>();
        public Tile WinningTile { get; private set; }
        //private ObservableCollection<Tile> Tiles = new ObservableCollection<Tile>();
        public List<Player> Players {get; private set;}

        public bool GameOver
        {
            get
            {
                if (this.Winner != null) return true;
                if (this.Tiles.Count == 0) return true;

                return false;
            }
        }
        public Player Winner { get; private set; }
        public Player PlayerHit { get; private set; }

        public Game()
        {
            //this.Tiles = new List<Tile>();
            this.Players = new List<Player>();

            /*
            foreach (var item in this.Tiles)
            {
                Console.WriteLine(item);
            }
             */ 

            /*
            Console.WriteLine(this.Tiles.Count);
            var foo = this.Tiles.Take(1);
            Console.WriteLine(foo.Count());
            Console.WriteLine(this.Tiles.Count);
             */
        }

        public void RegisterPlayer(Player player)
        {
            player.Game = this;
            this.Players.Add(player);
        }

        public int TilesLeft { get { return this.Tiles.Count; } }

        public void Play()
        {
            if (this.Players.Count < 1) throw new InvalidOperationException("Can not start game without players");

            this.Winner = null;
            this.PlayerHit = null;
            this.WinningTile = null;

            //Shuffled tiles
            this.Tiles = Game.GetAllTiles();
            this.Tiles.Sort((A, B) => { return A == B ? 0 : Guid.NewGuid().GetHashCode(); });
            //var nonShuffledTiles = Game.GetAllTiles();
            //nonShuffledTiles.Sort((A, B) => { return A == B ? 0 : Guid.NewGuid().GetHashCode(); });
            //this.Tiles = new ObservableCollection<Tile>(nonShuffledTiles);
            //this.Tiles.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Tiles_CollectionChanged);
            
            int x = 0;
            foreach (var Player in this.Players)
            {
                for (int i = 0; i < 13; i++)
                {
                    Player.Draw(this.Tiles.ElementAt(0));
                    this.Tiles.RemoveAt(0);
                }
            }

            while (!GameOver)
            {
                var currPlayer = this.Players[x];
                var currTile = this.Tiles.First();
                this.Tiles.Remove(currTile);

                if (currPlayer.Draw(currTile))
                {
                    Winner = currPlayer;
                    WinningTile = currTile;
                    //Console.WriteLine("Player {0} wins!", currPlayer.Name);
                    //Console.WriteLine("Winning hand:");
                    //this.Winner.RevealedHand.WriteColoredToConsole();
                }
                else
                {
                    var discardedTile = currPlayer.Discard();

                    HandlePlayerDiscard(currPlayer, discardedTile);

                    x = (x + 1) % this.Players.Count;
                }
            }

            if (this.Winner != null)
            {
                Console.WriteLine("Player {0} wins!",this.Winner.Name);
                if (this.PlayerHit != null)
                {
                    Console.Write("Ron on {0}'s ", this.PlayerHit.Name);
                }
                else
                {
                    Console.Write("Tsumo with ");
                }
                this.WinningTile.WriteColoredToConsole();
                Console.WriteLine();
                Console.WriteLine("Winning hand:");
                this.Winner.RevealedHand.WriteColoredToConsole();
                Console.Write(" + ");
                this.Winner.ExposedMelds.WriteColoredToConsole();
                Console.WriteLine();
            }
            else if (this.Tiles.Count == 0)
            {
                Console.WriteLine("No more tiles available. Game over.");
            }
            else 
            {
                Console.WriteLine("Game Over");
            }
        }

        //void Tiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    if ((sender as IEnumerable<object>).Count() == 0)
        //    {
        //        this.GameOver = true;
        //    }
        //}

        private void HandlePlayerDiscard(Player Discarder, Tile DiscardedTile)
        {
            //Console.SetCursorPosition(0, 15);

            Player Intercepter = null;

            int x = this.Players.IndexOf(Discarder);
            for (int y = x + 1; y < x + this.Players.Count; y++)
            {
                var nextPlayer = this.Players[y % this.Players.Count];
                if (nextPlayer.WantsDiscard(DiscardedTile, Discarder))
                {
                    Intercepter = nextPlayer;
                    //Console.Write("{0} called dibs on ", nextPlayer.Name);
                    //DiscardedTile.WriteColoredToConsole();
                    //Console.WriteLine();
                    break;
                }
                else
                {
                    //Console.Write("{0} didn't care for the discarded", nextPlayer.Name);
                    //DiscardedTile.WriteColoredToConsole();
                    //Console.WriteLine();
                }
            }

            if (Intercepter == null)
            {
                Discarder.Discards.Add(DiscardedTile);
            }
            else
            {
                if (Intercepter.ProcessInterception(DiscardedTile))
                {
                    Winner = Intercepter;
                    WinningTile = DiscardedTile;
                    PlayerHit = Discarder;
                    //Console.WriteLine("Player {0} wins!", Intercepter.Name);
                    //Console.WriteLine("Direct hit on {0}", Discarder.Name);
                    //Console.WriteLine("Winning hand:");
                    //this.Winner.RevealedHand.WriteColoredToConsole();                    
                }
                else
                {
                    var interceptorDiscardedTile = Intercepter.Discard();

                    HandlePlayerDiscard(Intercepter, interceptorDiscardedTile);

                    //if (this.Tiles.Count == 0)
                    //{
                    //    Console.WriteLine("No more tiles available. Game over.");
                    //}
                }
            }
        }

        public static List<Tile> GetAllTiles()
        {
            List<Tile> Tiles = new List<Tile>();
            for (int x = 0; x < 4; x++)
            {
                foreach (var Tile in SuitedTile.GetAll()) Tiles.Add(Tile);
                foreach (var Tile in DragonTile.GetAll()) Tiles.Add(Tile);
                foreach (var Tile in HonorTile.GetAll()) Tiles.Add(Tile);
            }
            return Tiles;
        }
    }
}
