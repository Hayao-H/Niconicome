using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Infrastructure.Database.LiteDB;

namespace Niconicome.Models.Infrastructure.Database.Types
{
    public class Cookie : IBaseStoreClass
    {
        public int Id { get; set; }

        public string TableName => TableNames.Cookie;

        /// <summary>
        /// UserSession(暗号化)
        /// </summary>
        public string UserSession { get; set; } = string.Empty;

        /// <summary>
        /// UserSessionSecure(暗号化)
        /// </summary>
        public string UserSessionSecure { get; set; } = string.Empty;

        /// <summary>
        /// キー(暗号化)
        /// </summary>
        public string Key { get; set; } = string.Empty;
    }
}
