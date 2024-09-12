using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Server.API.Watch.V1.HLS.AES
{
    public interface IAESInfomation
    {
        /// <summary>
        /// IV
        /// </summary>
        byte[] IV { get; }

        /// <summary>
        /// Key
        /// </summary>
        byte[] Key { get; }
    }

    public record AESInfomation(byte[] Key, byte[] IV) : IAESInfomation;
}
