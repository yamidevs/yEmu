using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Classes.Characters
{
    class Characters
    {
        public int id { get; set; }
        public string nom { get; set; }
        public int level { get; set; }
        //TODO : changement de la colone sur la table personnages 
       // public Class class { get; set; }
        public int skin { get; set; }
        public int sexe { get; set; }
        public int color1 { get; set; }
        public int color2 { get; set; }
        public int color3 { get; set; }
        public int kamas { get; set; }
        public int statsPoints { get; set; }
        public int spellPoints { get; set; }
        public int accounts { get; set; }

    }
}
