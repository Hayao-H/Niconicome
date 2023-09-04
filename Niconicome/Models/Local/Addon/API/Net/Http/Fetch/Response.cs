using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.Addon.API.Net.Http.Fetch
{
    public class Response
    {
        public Response(HttpResponseMessage message)
        {
            this.message = message;
        }

        #region field

        private readonly HttpResponseMessage message;

        #endregion

        public bool ok => this.message.IsSuccessStatusCode;

        public int status => (int)this.message.StatusCode;

        public async Task<string> text()
        {
            return await this.message.Content.ReadAsStringAsync();
        }
    }
}
