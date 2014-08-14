using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Classes.Characters
{
    public class Alignments
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int Honor { get; set; }
        public int Deshonor { get; set; }
        public int Level { get; set; }
        public int Grade { get; set; }
        public int Enabled { get; set; }

        public AlignmentTypeEnum AlignmentType
        {
            get
            {
                return (AlignmentTypeEnum)this.Type;
            }
        }

        public string PatternInfos
        {
            get
            {
                return string.Format("{0},{1},{2}", Type, Type, (Enabled != 0? Level.ToString() : "0"));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}~2,{1},{2},{3},{4},{5}", Id, Level, Level, Honor, Deshonor, Enabled);
        }

        public enum AlignmentTypeEnum
        {
            ALIGNMENT_NEUTRAL = -1,
            ALIGNMENT_BONTARIAN = 1,
            ALIGNMENT_BRAKMARIAN = 2,
            ALIGNMENT_MERCENARY = 3,
        }
    }
}
