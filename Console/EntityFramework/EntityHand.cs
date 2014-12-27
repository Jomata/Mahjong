using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Mahjong.EntityFramework
{
    [Table("hands")]
    public class EntityHand
    {
        public int? id { get; set; }

        [ForeignKey("south_player_id")]
        public EntityPlayer south_player { get; set; }
        [ForeignKey("north_player_id")]
        public EntityPlayer north_player { get; set; }
        [ForeignKey("east_player_id")]
        public EntityPlayer east_player { get; set; }
        [ForeignKey("west_player_id")]
        public EntityPlayer west_player { get; set; }

        public int south_player_id { get; set; }
        public int north_player_id { get; set; }
        public int east_player_id { get; set; }
        public int west_player_id { get; set; }

        public int south_player_score { get; set; }
        public int north_player_score { get; set; }
        public int east_player_score { get; set; }
        public int west_player_score { get; set; }
        public string winning_hand { get; set; }
    }
}
