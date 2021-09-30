using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage
{
    internal class StorageDataType : IStorable
    {

        public static string TableName = "data";

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Value { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
