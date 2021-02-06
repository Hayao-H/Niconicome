using System;
using System.Net.Http;
using Niconicome.Models.Domain.Local;
using LiteDB;
using Niconicome.Models.Domain.Niconico;

namespace NiconicomeTest
{
    static class Static
    {
        public static ILiteDatabase LiteDataBaseInstance
        {
            get
            {
                if (Static.innerLiteDataBaseInstance == null)
                {
                    Static.innerLiteDataBaseInstance = new LiteDatabase("Filename=test.db;Mode=Shared;");
                }
                return Static.innerLiteDataBaseInstance;
            }
        }

        private static ILiteDatabase? innerLiteDataBaseInstance;

        public static IDataBase DataBaseInstance {
            get
            {
                if (Static.innerDataBaseInstance == null)
                {
                    Static.innerDataBaseInstance = new DataBase(Static.LiteDataBaseInstance,false);
                }
                return Static.innerDataBaseInstance;
            }
        }

        private static IDataBase? innerDataBaseInstance;
    }
}
