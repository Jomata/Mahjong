using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mahjong;

namespace ShantenHelper.Tiles
{
    public class TileLoadingComponent : TileBaseComponent, Interfaces.ILoadingComponent
    {
        public TileLoadingComponent(TileGameObject Tile) : base(Tile) { }

        private string TexturePath
        {
            get
            {
                var tile = this.Tile.Tile;

                string basePath = "img/tiles/png_42x64/";
                string folder = "";
                string filename = "";

                if (tile is SuitedTile)
                {
                    switch ((tile as SuitedTile).Suit)
                    {
                        case TileSuits.Bamboo:
                            folder = "bamboo";
                            break;
                        case TileSuits.Character:
                            folder = "man";
                            break;
                        case TileSuits.Circle:
                            folder = "pin";
                            break;
                    }

                    filename = folder + (tile as SuitedTile).Rank.ToString();
                }
                else if (tile is DragonTile)
                {
                    folder = "dragons";

                    switch ((tile as DragonTile).Dragon)
                    {
                        case TileDragons.Green:
                            filename = "dragon-green";
                            break;
                        case TileDragons.Red:
                            filename = "dragon-chun";
                            break;
                        case TileDragons.White:
                            filename = "dragon-haku";
                            break;
                        default:
                            break;
                    }
                }
                else if (tile is HonorTile)
                {
                    folder = "winds";
                    filename = "wind-" + (tile as HonorTile).Honor.ToString().ToLower();
                }
                else
                {
                    folder = "";
                    filename = "face-down";
                }

                return System.IO.Path.Combine(basePath, folder, filename);
            }
        }

        public void Load(ContentManager Content)
        {
            //TileTextures.Add(tile.ToString(), Content.Load<Texture2D>(tile.GetTexturePath()));
            Tile.Texture = Content.Load<Texture2D>(this.TexturePath);
            Tile.Font = Content.Load<SpriteFont>("CourierNew");
        }
    }
}
