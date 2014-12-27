using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MahjongXNA
{
    class TileTextureInfo
    {
        public TileTextureInfo()
        {
            this.ShadeColor = Microsoft.Xna.Framework.Color.White;
            this.Rotation = 0f;
        }

        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get; set; }
        public Mahjong.Tile Tile { get; set; }
        //public bool Highlighted { get; set; }
        //public bool MouseOvered { get; set; }
        public Microsoft.Xna.Framework.Color ShadeColor { get; set; }
        
        //public Microsoft.Xna.Framework.Rectangle Rectangle { get; set; }
        public Microsoft.Xna.Framework.Rectangle Rectangle
        {
            get
            {
                if(this.Rotation == (float)Math.PI)  // 180 degrees;
                    return new Microsoft.Xna.Framework.Rectangle((int)this.Position.X - this.Texture.Width, (int)this.Position.Y - this.Texture.Height, this.Texture.Width, this.Texture.Height);
                else if (this.Rotation == (float)Math.PI / 2.0f)  // 90 degrees;
                    return new Microsoft.Xna.Framework.Rectangle((int)this.Position.X - this.Texture.Height, (int)this.Position.Y, this.Texture.Height, this.Texture.Width);
                else if (this.Rotation == (float)Math.PI * 1.5f)  // 270 degrees;
                    return new Microsoft.Xna.Framework.Rectangle((int)this.Position.X, (int)this.Position.Y - this.Texture.Width, this.Texture.Height, this.Texture.Width);
                else
                    return new Microsoft.Xna.Framework.Rectangle((int)this.Position.X, (int)this.Position.Y, this.Texture.Width, this.Texture.Height);
            }
        }
        
        public Microsoft.Xna.Framework.Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (this.Tile != null)
            {
                sb.Append(this.Tile);
                sb.Append(" ");
            }

            if (this.Rectangle != null)
            {
                sb.Append(this.Rectangle);
                sb.Append(" ");
            }

            if (this.ShadeColor != null)
            {
                sb.Append(this.ShadeColor);
            }

            return sb.ToString();
        }
    }
}
