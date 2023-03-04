using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Infrastructure.Database.LiteDB;

namespace Niconicome.Models.Infrastructure.Database.Types
{
    public class Tag : IBaseStoreClass
    {
        public int Id { get; set; }

        public string TableName { get; set; } = TableNames.Tag;

        public string Name { get; set; } = string.Empty;

        public bool IsNicodicExist { get; set; }
    }
}
