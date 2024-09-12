using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Utils.Reactive;

namespace Niconicome.Models.Domain.Network
{
    public interface INetWorkState
    {
        Task<bool> IsNetWorkAvailable();
    }

    public class NetWorkState : INetWorkState
    {
        public NetWorkState(INicoHttp http)
        {
            this._http = http;
        }

        private readonly INicoHttp _http;

        public async Task<bool> IsNetWorkAvailable()
        {
            try
            {
                var result = await this._http.GetAsync(new Uri("http://clients3.google.com/generate_204"));
                return result.StatusCode == HttpStatusCode.NoContent;
            }
            catch
            {
                return false;
            }
        }
    }
}
