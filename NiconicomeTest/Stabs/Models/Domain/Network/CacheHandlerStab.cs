using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Network;

namespace NiconicomeTest.Stabs.Models.Domain.Network
{
    class CacheStreamStab : ICacheStraem
    {
        public void WriteCache(byte[] data, string filePath) { }
        public Task GetAndWrite(string url, string filePath)
        {
            return Task.CompletedTask;
        }
        public Task<bool> TryGetAndWrite(string url, string filePath)
        {
            return Task.FromResult(true);
        }
    }
}
