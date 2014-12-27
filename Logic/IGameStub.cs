using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public class IGameStub : IGame
    {
        bool UradorasRevealed = false;
        public IGameStub()
        {
            this.Doras = new List<Tile>();
            this.DoraIndicators = new List<Tile>();
            this.Uradoras = new List<Tile>();
            this.UradoraIndicators = new List<Tile>();
            this.Players = new List<IPlayer>();
            this.TilesLeft = 10;
        }

        public TileHonors GameWind
        {
            get;
            set;
        }

        public List<Tile> DoraIndicators { get; set; }
        public List<Tile> Uradoras
        {
            get;
            set;
        }


        public List<Tile> UradoraIndicators{ get; set; }

        public List<Tile> Doras
        {
            get;
            set;
        }


        public List<IPlayer> Players
        {
            get;
            set;
        }

        public IPlayer CurrentPlayer
        {
            get;
            set;
        }


        public int TilesLeft
        {
            get;
            set;
        }
    }
}
