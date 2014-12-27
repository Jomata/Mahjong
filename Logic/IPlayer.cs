using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public interface IPlayer
    {
        TileHonors PlayerWind { get; }
        bool Tsumo { get; }
        bool Ippatsu { get; }
        bool Riichi { get; }
        Tile WinningTile { get; }
        bool IsDealer { get; }
        string Name { get; }
    }

    //public abstract class IPlayer
    //{
    //    public abstract TileHonors PlayerWind { get; }
    //    public abstract bool Tsumo { get; }
    //    public abstract bool Ippatsu { get; }
    //    public abstract bool Riichi { get; }
    //    public abstract Tile TrackedTile { get; }
    //    public abstract bool IsDealer { get; }
    //}
}
