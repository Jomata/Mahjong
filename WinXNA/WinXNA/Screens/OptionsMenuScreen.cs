#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace MahjongXNA
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry menuEntry_RecommendTiles;
        MenuEntry menuEntry_ToolTips;
        MenuConfigEntry menuEntry_RecommendedTileColor;
        MenuConfigEntry menuEntry_HighlightedTilesColor;
        MenuConfigEntry menuEntry_SelectedTileColor;
        List<string> ColorsList = Colors.Dictionary.Keys.ToList();

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            menuEntry_ToolTips = new MenuEntry(string.Empty);
            menuEntry_RecommendTiles = new MenuEntry(string.Empty);
            menuEntry_RecommendedTileColor = new MenuConfigEntry("Recommended tiles color: {1}", "SelectableTileColor_Recommended", ColorsList);
            menuEntry_HighlightedTilesColor = new MenuConfigEntry("Selectable tiles color: {1}","SelectableTileColor_Highlighted", ColorsList);
            menuEntry_SelectedTileColor = new MenuConfigEntry("Selected tile color: {1}", "SelectableTileColor_MouseOver", ColorsList);
            
            //frobnicateMenuEntry = new MenuEntry(string.Empty);
            //elfMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            menuEntry_RecommendTiles.Selected += new System.EventHandler<PlayerIndexEventArgs>(menuEntry_RecommendTiles_Selected);
            menuEntry_ToolTips.Selected += new System.EventHandler<PlayerIndexEventArgs>(menuEntry_ToolTips_Selected);

            menuEntry_HighlightedTilesColor.Selected += new System.EventHandler<PlayerIndexEventArgs>(ColorSelectEntry_Changed);
            menuEntry_RecommendedTileColor.Selected += new System.EventHandler<PlayerIndexEventArgs>(ColorSelectEntry_Changed);
            menuEntry_SelectedTileColor.Selected += new System.EventHandler<PlayerIndexEventArgs>(ColorSelectEntry_Changed);
            menuEntry_HighlightedTilesColor.SelectedLeftwards += new System.EventHandler<PlayerIndexEventArgs>(ColorSelectEntry_Changed);
            menuEntry_RecommendedTileColor.SelectedLeftwards += new System.EventHandler<PlayerIndexEventArgs>(ColorSelectEntry_Changed);
            menuEntry_SelectedTileColor.SelectedLeftwards += new System.EventHandler<PlayerIndexEventArgs>(ColorSelectEntry_Changed);
            ColorSelectEntry_Changed(menuEntry_HighlightedTilesColor, null);
            ColorSelectEntry_Changed(menuEntry_RecommendedTileColor, null);
            ColorSelectEntry_Changed(menuEntry_SelectedTileColor, null);

            back.Selected += OnCancel;

            MenuEntries.Add(menuEntry_ToolTips);
            MenuEntries.Add(menuEntry_RecommendTiles);
            MenuEntries.Add(menuEntry_RecommendedTileColor);
            MenuEntries.Add(menuEntry_HighlightedTilesColor);
            MenuEntries.Add(menuEntry_SelectedTileColor);
            
            MenuEntries.Add(back);
        }

        void ColorSelectEntry_Changed(object sender, PlayerIndexEventArgs e)
        {
            if (sender is MenuConfigEntry)
            {
                (sender as MenuConfigEntry).SelectedColor = Colors.FromName((sender as MenuConfigEntry).ConfigValue);
            }
        }

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            this.menuEntry_ToolTips.Text = "Show tiles tooltips: " + (bool.Parse(ConfigurationManager.AppSettings["tileToolTips"]) ? "Yes" : "No");
            this.menuEntry_RecommendTiles.Text = "Recommend tiles to discard: " + (bool.Parse(ConfigurationManager.AppSettings["recommendDiscards"]) ? "Yes" : "No");
        }

        #endregion

        #region Handle Input

        void menuEntry_ToolTips_Selected(object sender, PlayerIndexEventArgs e)
        {
            var current = bool.Parse(ConfigurationManager.AppSettings["tileToolTips"]);
            ConfigurationManager.AppSettings["tileToolTips"] = (!current).ToString();

            SetMenuEntryText();
        }

        void menuEntry_RecommendTiles_Selected(object sender, PlayerIndexEventArgs e)
        {
            var current = bool.Parse(ConfigurationManager.AppSettings["recommendDiscards"]);
            ConfigurationManager.AppSettings["recommendDiscards"] = (!current).ToString();

            SetMenuEntryText();
        }

        #endregion
    }
}
