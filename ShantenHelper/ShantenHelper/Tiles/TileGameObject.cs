using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShantenHelper.Tiles
{
    public class TileGameObject : GameObject
    {
        public bool Highlighteable { get; set; }

        private Vector2 position;
        public Vector2 Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
                updateTextPosition();
            }
        }
        public Vector2 TextPosition { get; private set; }
        public Color TextColor { get; protected set; }

        private Texture2D texture;
        public Texture2D Texture
        {
            get { return this.texture; }
            set
            {
                this.texture = value;
                updateTextPosition();
            }
        }

        private void updateTextPosition()
        {
            if (this.Texture != null && this.Position != null && this.RecommendedDiscardIntensity != null && this.Font != null)
            {
                var textSize = this.Font.MeasureString(this.RecommendedDiscardIntensityText);
                this.TextPosition = new Vector2(this.Position.X - textSize.X/2 + Texture.Width/2, this.Position.Y + Texture.Height);
            }
        }

        private SpriteFont font;
        public SpriteFont Font
        {
            get { return this.font; }
            set
            {
                this.font = value;
                updateTextPosition();
            }
        }

        private Mahjong.Tile tile;
        public Mahjong.Tile Tile 
        {
            get { return this.tile;}
            set
            {
                this.tile = value;
                updateTextPosition();
            }
        }

        public bool Highlighted { get; set; }
        public Color HighlightColor { get; protected set; }
        public bool RecommendedDiscard { get; set; }
        public Color RecommendedDiscardColor { get; protected set; }

        public string RecommendedDiscardIntensityText
        {
            get { return Math.Round(this.RecommendedDiscardIntensity.Value, 1).ToString(); }
        }

        private float? recommendedDiscardIntensity;
        public float? RecommendedDiscardIntensity
        {
            get { return this.recommendedDiscardIntensity; }
            set
            {
                this.recommendedDiscardIntensity = value;
                updateTextPosition();
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                //if (this.Rotation == (float)Math.PI)  // 180 degrees;
                //    return new Microsoft.Xna.Framework.Rectangle((int)this.Position.X - this.Texture.Width, (int)this.Position.Y - this.Texture.Height, this.Texture.Width, this.Texture.Height);
                //else if (this.Rotation == (float)Math.PI / 2.0f)  // 90 degrees;
                //    return new Microsoft.Xna.Framework.Rectangle((int)this.Position.X - this.Texture.Height, (int)this.Position.Y, this.Texture.Height, this.Texture.Width);
                //else if (this.Rotation == (float)Math.PI * 1.5f)  // 270 degrees;
                //    return new Microsoft.Xna.Framework.Rectangle((int)this.Position.X, (int)this.Position.Y - this.Texture.Width, this.Texture.Height, this.Texture.Width);
                //else
                    return new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Texture.Width, this.Texture.Height);
            }
        }

        public TileGameObject(Game game) : base(game)
        {
            this.DrawingComp = new TileDrawingComponent(this);
            this.LoaderComp = new TileLoadingComponent(this);
            this.UpdatingComp = new TileUpdatingComponent(this);
            this.HighlightColor = Microsoft.Xna.Framework.Color.Red;
            this.RecommendedDiscardColor = Microsoft.Xna.Framework.Color.LightBlue;
            this.RecommendedDiscardIntensity = null;
            this.TextColor = Microsoft.Xna.Framework.Color.Black;
            this.Highlighteable = true;
        }
    }
}
