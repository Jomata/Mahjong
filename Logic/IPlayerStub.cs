using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public class IPlayerStub : IPlayer
    {
        public TileHonors PlayerWind
        {
            get;
            set;
        }


        public bool Tsumo
        {
            get;
            set;
        }

        public bool Ippatsu
        {
            get;
            set;
        }

        public bool Riichi
        {
            get;
            set;
        }

        public bool IsDealer
        {
            get;
            set;
        }


        public Tile WinningTile
        {
            get;
            set;
        }


        public string Name
        {
            get;
            set;
        }
    }
}
