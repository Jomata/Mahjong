using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public interface IGame
    {
        TileHonors GameWind { get; }

        List<Tile> UradoraIndicators { get; }
        List<Tile> DoraIndicators { get; }
        List<Tile> Doras { get; }
        List<Tile> Uradoras { get; }

        List<IPlayer> Players { get; }
        IPlayer CurrentPlayer { get; }

        int TilesLeft { get; }
    }
}
