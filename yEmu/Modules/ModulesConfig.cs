using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEmu.Realm.Databases;
using yEmu.Realm.Databases.Interfaces;
using yEmu.Realm.Databases.Requetes;
using yEmu.Util;

namespace yEmu.Modules
{
   public class ModulesConfig : NinjectModule
    {
      
        public override void Load()
        {
            this.Bind<IDatabases>().To<Databases>();
        }
   
    }
}
