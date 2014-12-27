using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mahjong;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace MahjongXNA
{
    public static class Colors
    {
        public static readonly Dictionary<string, Color> Dictionary =
            typeof(Color).GetProperties(BindingFlags.Public | 
                                        BindingFlags.Static)
                         .Where(prop => prop.PropertyType == typeof(Color))
                         .ToDictionary(prop => prop.Name,
                                       prop => (Color) prop.GetValue(null, null));

        public static Color FromName(string name)
        {
            // Adjust behaviour for lookup failure etc
            return Dictionary[name];
        }
    }

    public static class Utils
    {
        public static string GetTexturePath(this Tile tile)
        {
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
}
