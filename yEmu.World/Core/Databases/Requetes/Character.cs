﻿using System;
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
        public static List<Characters> characters = new List<Characters>();

        public void Load()
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                var results = connection.Query<Characters>("SELECT * FROM personnages");

                foreach (var result in results)
                {
                    result.Maps = Map.Maps.Find(x => x.ID == result.MapId);
                    characters.Add(result);
                }
            }
            Info.Write("database", string.Format("{0} Characters chargés", characters.Count()), ConsoleColor.Green);
        }

        public static void Create(Characters characters)
        {
            using (IDbConnection connection = Databases.GetConnection())
            {
                connection.Execute("INSERT INTO personnages SET id=@id, nom=@nom,sexe = @sexe,Classes=@Classes,"+
                "color1 = @color1 , color2 = @color2 , color3 = @color3 , accounts = @accounts , skin = @skin, "+
                "level = @level  , MapId = @MapId , CellId = @CellId , Direction =  @Direction , ServerId = @ServerId",
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
                        ServerId = Servers.ServerId
                        });                         
            }
            lock (Character.characters)
                Character.characters.Add(characters);

        }
    }
}
