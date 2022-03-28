using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Response
{
    public class ResponseRoot
    {
        [JsonPropertyName("chat")]
        public Chat? Chat { get; set; }
    }
}
