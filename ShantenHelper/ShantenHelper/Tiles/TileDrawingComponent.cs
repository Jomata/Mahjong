using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ShantenHelper.Tiles
{
    public class TileDrawingComponent : TileBaseComponent, Interfaces.IDrawingComponent
    {
        public TileDrawingComponent(TileGameObject Tile) : base(Tile) { }

        //public void Draw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        //{
        //    //spriteBatch.Draw(this.Tile.Texture, this.Tile.Position, null, TTI.ShadeColor, TTI.Rotation, origin, 1.0f, SpriteEffects.None, 0.0f);
        //    spriteBatch.Draw(this.Tile.Texture, this.Tile.Position, Microsoft.Xna.Framework.Color.White);
        //}

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.Tile.SpriteBatch.Begin();
            Microsoft.Xna.Framework.Color color;
            if (this.Tile.Highlighted)
            {
                color = this.Tile.HighlightColor;
            }
            else if (this.Tile.RecommendedDiscard)
            {
                if (false && this.Tile.RecommendedDiscardIntensity.HasValue)
                {
                    color = this.Tile.RecommendedDiscardColor * (this.Tile.RecommendedDiscardIntensity.Value + 1.0f); //We don't want intensity 0 tiles to be transparent
                }
                else
                {
                    color = this.Tile.RecommendedDiscardColor;
                }
            }
            else
            {
                color = Microsoft.Xna.Framework.Color.White;
            }
            this.Tile.SpriteBatch.Draw(this.Tile.Texture, this.Tile.Position, color);

            if (this.Tile.RecommendedDiscardIntensity.HasValue)
            {
                this.Tile.SpriteBatch.DrawString(Tile.Font, Tile.RecommendedDiscardIntensityText, this.Tile.TextPosition, this.Tile.TextColor);
            }

            this.Tile.SpriteBatch.End();
        }
    }
}
