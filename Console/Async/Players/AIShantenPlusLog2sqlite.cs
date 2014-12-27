using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mahjong.EntityFramework;

namespace Mahjong.Async.Players
{
    public class AIShantenPlusLog2sqlite : AIShantenPlus
    {
        private EntityPlayer myRecord;
        public EntityPlayer Entity
        {
            get { return myRecord; }
        }

        public override void AverageWeightsWith(AIShantenPlus Another)
        {
            base.AverageWeightsWith(Another);
            //set entity as a new one
            updatedEntityValues();
            myRecord.id = null;
            saveEntity();
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
                //Update entity
                myRecord.name = value;
                saveEntity();
            }
        }

        private void updatedEntityValues()
        {
            myRecord.name = this.Name;
            myRecord.weight_adjacent = Adjacent;
            myRecord.weight_any_discard = AnyDiscard;
            myRecord.weight_dora = Dora;
            myRecord.weight_honor_or_terminal = HonorOrTerminal;
            myRecord.weight_meld = Meld;
            myRecord.weight_my_discard = MyDiscard;
            myRecord.weight_pair = Pair;
            myRecord.weight_pseudo_adjacent = PseudoAdjacent;
            myRecord.weight_same_suit = SameSuit;
        }

        private void saveEntity()
        {
            using (var context = new EntityFramework.MahjongDBContext())
            {
                if (myRecord.id.HasValue)
                {
                    context.Players.Attach(myRecord);
                    context.Entry(myRecord).Property(x => x.name).IsModified = true;
                }
                else
                {
                    context.Players.Add(myRecord);
                }
                context.SaveChanges();
            }
        }

        public AIShantenPlusLog2sqlite() : base()
        {
            myRecord = new EntityPlayer();
            updatedEntityValues();
            saveEntity();
        }
    }
}
