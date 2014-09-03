using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using yEmu.Util;
using yEmu.World.Core.Classes.Characters;
using yEmu.World.Core.Enums;

namespace yEmu.World.Core.Handler
{
    class CharacterHandler
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        public void Character(Processor Processor , string data)
        {
            cacheLock.EnterReadLock();
            try
            {
                if (yEmu.World.Core.Databases.Requetes.Character.characters.FindAll(x => x.accounts == Processor.Clients.Accounts.Id).Count >= int.Parse(Configuration.getString("PersonnagesComptes")))
                {
                    Processor.Clients.Send("AAEf");
                    return;
                }

                var datas = data.Split('|');

                var name = datas[0];

                var classe = int.Parse(datas[1]);

                var sex = int.Parse(datas[2]);

                var color1 = int.Parse(datas[3]);

                var color2 = int.Parse(datas[4]);

                var color3 = int.Parse(datas[5]);

                if (yEmu.World.Core.Databases.Requetes.Character.characters.All(x => x.nom != name) && name.Length >= 3 && name.Length <= 20)
                {
                    var reg = new Regex("^[a-zA-Z-]+$");

                    if (reg.IsMatch(name) && name.Count(c => c == '-') < 3)
                    {
                        if (classe >= 1 && classe <= 12 && (sex == 1 || sex == 0))
                        {
                            var newCharacter = new Characters
                            {
                                id =
                                    yEmu.World.Core.Databases.Requetes.Character.characters.Count > 0
                                        ? yEmu.World.Core.Databases.Requetes.Character.characters.OrderByDescending(x => x.id).First().id + 1
                                        : 1,
                                nom = name,
                                Classes = (Class)classe,
                                sexe = sex,
                                color1 = color1,
                                color2 = color2,
                                color3 = color3,
                                level = int.Parse(Configuration.getString("Start_level")),
                                skin = int.Parse(classe + "" + sex),
                                accounts = Processor.Clients.Accounts.Id,
                                pdvNow =
                                    (int.Parse(Configuration.getString("Start_level")) - 1) * Characters.GainHpPerLvl + Characters.BaseHp,
                                PdvMax =
                                    (int.Parse(Configuration.getString("Start_level")) - 1) * Characters.GainHpPerLvl + Characters.BaseHp,
                            };

                            newCharacter.GenerateInfos(Processor.Clients.Accounts.Level);

                            yEmu.World.Core.Databases.Requetes.Character.Create(newCharacter);

                            Processor.Clients.Send("AAK");

                            Processor.Clients.Send("TB");

                            Processor.ListCharacters("");
                        }
                        else
                        {
                            Processor.Clients.Send("AAEf");
                        }
                    }
                    else
                    {
                        Processor.Clients.Send("AAEn");
                    }
                }
                else
                {
                    Processor.Clients.Send("AAEa");
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
                cacheLock.Dispose();
            }
          
        }
    }
}
