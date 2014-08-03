using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using yEmu.Core.Reflection;
using System.Data;
using yEmu.Util;
using yEmu.World.Core.Classes.Accounts;

namespace yEmu.World.Core.Databases.Requetes
{
    class CharactersAccounts : Singleton<CharactersAccounts>
    {
        public static readonly List<AccountCharacters> AccountCharacters = new List<AccountCharacters>();

        public void Load()
        {
           
        }
    }
}
