using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Mahjong.EntityFramework
{
    [Table("players")]
    public class EntityPlayer
    {
        public int? id { get; set; }
        public string name { get; set; }
        public float weight_pair { get; set; }
        public float weight_adjacent { get; set; }
        public float weight_pseudo_adjacent { get; set; }
        public float weight_same_suit { get; set; }
        public float weight_dora { get; set; }
        public float weight_any_discard { get; set; }
        public float weight_my_discard { get; set; }
        public float weight_meld { get; set; }
        public float weight_honor_or_terminal { get; set; }
    }
}
