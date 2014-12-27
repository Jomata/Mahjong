using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MahjongXNA
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuConfigEntry : MenuEntry
    {
        string ConfigKey {get;set;}
        List<string> ValidValues { get; set; }
        public string ConfigValue
        {
            get { return ConfigurationManager.AppSettings[ConfigKey]; }
            private set { ConfigurationManager.AppSettings[ConfigKey] = value; }
        }

        public MenuConfigEntry(string Text, string ConfigKey, List<string> ValidValues) : base(Text)
        {
            this.ConfigKey = ConfigKey;
            this.ValidValues = ValidValues;
        }
        public MenuConfigEntry(string ConfigKey, List<string> ValidValues) : base("{0}: {1}")
        {
            this.ConfigKey = ConfigKey;
            this.ValidValues = ValidValues;
        }

        private void moveSelectedValue(int direction)
        {
            int currentIndex = ValidValues.IndexOf(ConfigValue);
            int newIndex = (ValidValues.Count + currentIndex + direction) % ValidValues.Count;
            this.ConfigValue = ValidValues[newIndex];
        }

        protected internal override void OnSelectEntry(Microsoft.Xna.Framework.PlayerIndex playerIndex)
        {
            this.moveSelectedValue(+1);
            base.OnSelectEntry(playerIndex);
        }

        protected internal override void OnSelectEntryLeftwards(Microsoft.Xna.Framework.PlayerIndex playerIndex)
        {
            this.moveSelectedValue(-1);
            base.OnSelectEntryLeftwards(playerIndex);
        }

        public override string Text
        {
            get
            {
                return String.Format(base.Text,this.ConfigKey,this.ConfigValue);
            }
            set
            {
                base.Text = value;
            }
        }
    }
}
