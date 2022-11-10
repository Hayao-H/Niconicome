using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Infrastructure.Database.LiteDB;

namespace Niconicome.Models.Domain.Utils.Error
{
    public static class ErrorTypes
    {
        /// <summary>
        /// エラー列挙型の型一覧
        /// </summary>
        public static Dictionary<int, Type> ErrorEnums = new()
        {
            { 1,typeof(LiteDBError) }
        };
    }
}
