using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShantenHelper.Tiles
{
    public class TileUpdatingComponent : TileBaseComponent, Interfaces.IUpdatingComponent
    {
        public TileUpdatingComponent(TileGameObject Tile) : base(Tile) { }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            #region Setting Highlighted True/False
            var MouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            var MousePos = new Point(MouseState.X, MouseState.Y);
            this.Tile.Highlighted = this.Tile.Highlighteable && this.Tile.Rectangle.Contains(MousePos);
            #endregion
        }
    }
}
