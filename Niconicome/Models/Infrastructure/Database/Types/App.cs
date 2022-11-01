using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local;

namespace Niconicome.Models.Infrastructure.Database.Types
{
    public class App : IStorable
    {
        public int Id { get; set; }

        public static string TableName { get; set; } = "applicationinfomation";

        /// <summary>
        /// データベースのバージョン
        /// </summary>
        public string DBVersion { get; set; } = "0.0.0";
    }
}
