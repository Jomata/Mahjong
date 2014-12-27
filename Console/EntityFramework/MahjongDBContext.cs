using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong.EntityFramework
{
    public class MahjongDBContext : System.Data.Entity.DbContext
    {
        public System.Data.Entity.DbSet<EntityHand> Hands { get; set; }
        public System.Data.Entity.DbSet<EntityPlayer> Players { get; set; }
        public MahjongDBContext() : base("MahjongDBContext") { }
    }
}
