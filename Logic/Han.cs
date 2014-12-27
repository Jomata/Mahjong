using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public class Han
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }

        public Han(string Name, string Description, int Value)
        {
            this.Name = Name;
            this.Description = Description;
            this.Value = Value;
        }

        public override string ToString()
        {
            StringBuilder SB = new StringBuilder();
            SB.Append(this.Name);
            if (!string.IsNullOrEmpty(this.Description))
                SB.AppendFormat(" ({0})", this.Description);
            SB.AppendFormat(": {0} han", this.Value);
            if (this.Value != 1) SB.Append("s");

            return SB.ToString();
        }
    }
    public class Fu
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }

        public Fu(string Name, string Description, int Value)
        {
            this.Name = Name;
            this.Description = Description;
            this.Value = Value;
        }

        public override string ToString()
        {
            StringBuilder SB = new StringBuilder();
            SB.Append(this.Name);
            if (!string.IsNullOrEmpty(this.Description))
                SB.AppendFormat(" ({0})", this.Description);
            SB.AppendFormat(": {0} fu", this.Value);
            if (this.Value != 1) SB.Append("s");

            return SB.ToString();
        }
    }
}
