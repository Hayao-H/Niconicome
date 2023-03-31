using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Infrastructure.Database.Types
{
    public class App : IBaseStoreClass
    {
        public int Id { get; set; }

        public string TableName { get; set; } = TableNames.AppInfomation;

        /// <summary>
        /// データベースのバージョン
        /// </summary>
        public string DBVersion { get; set; } = "0.0.0";
    }
}
