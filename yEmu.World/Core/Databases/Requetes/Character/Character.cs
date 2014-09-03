using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using yEmu.Core.Reflection;
using yEmu.Collections;
using System.Data;
using yEmu.Util;
using yEmu.World.Core.Classes.Characters;

namespace yEmu.World.Core.Databases.Requetes
{
    class Character : Singleton<Character>
    {
        private object Lock = new object();
        public static List<Characters> characters = new List<Characters>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<Characters>("SELECT * FROM personnages");

                Parallel.ForEach(results, result =>
                {
                    result.Maps = Map.Maps.Find(x => x.ID == result.MapId);
                    result.Stats = Character_Stats.Characters_stats.Find(x => x.id == result.statsId);
                    result.Alignment = Alignment.Alignments.Find(x => x.Id == result.alignmentId);

                    if (result.savezaap.Contains('|'))
                    {
                        var id = result.savezaap.Split('|');

                        foreach (var item in id)
                        {
                            if (item.Length >= 1)
                            {
                                Characters.PacketZaap.Add(int.Parse(item));
                            }
                        }
                    }

                    lock (Lock)
                    {
                        characters.Add(result);
                    }
                });                
                
            }
            
            Info.Write("database", string.Format("{0} Characters chargés", characters.Count()), ConsoleColor.Green);
        }

        public static void Create(Characters characters)
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                connection.Execute("INSERT INTO personnages SET id=@id, nom=@nom,sexe = @sexe,Classes=@Classes,"+
                "color1 = @color1 , color2 = @color2 , color3 = @color3 , accounts = @accounts , skin = @skin, "+
                "level = @level  , MapId = @MapId , CellId = @CellId , Direction =  @Direction , ServerId = @ServerId,pdvNow=@pdvNow,alignmentId=@alignmentId",
                    new {
                        id = characters.id ,
                        nom = characters.nom,
                        sexe = characters.sexe,
                        Classes = characters.Classes,
                        color1 = characters.color1,
                        color2 = characters.color2,
                        color3 = characters.color3,
                        accounts = characters.accounts,
                        skin  = characters.skin ,
                        level = characters.level,
                        MapId = characters.Maps.ID,
                        CellId = characters.CellId,
                        Direction = characters.Direction,
                        ServerId = Servers.ServerId,
                        pdvNow = characters.pdvNow,
                        alignmentId = characters.alignmentId
                        });                         
            }
            lock (Character.characters)
                Character.characters.Add(characters);

        }
    }
}
