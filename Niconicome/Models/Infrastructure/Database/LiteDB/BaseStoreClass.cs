using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Niconicome.Models.Infrastructure.Database.LiteDB
{
    public interface IBaseStoreClass
    {
        public int Id { get; set; }

        public string TableName { get; }
    }
}
