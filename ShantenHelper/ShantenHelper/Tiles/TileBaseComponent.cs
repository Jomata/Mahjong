using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShantenHelper.Tiles
{
    public class TileBaseComponent
    {
        public TileGameObject Tile { get; private set; }

        public TileBaseComponent(TileGameObject Tile)
        {
            this.Tile = Tile;
        }
    }
}
