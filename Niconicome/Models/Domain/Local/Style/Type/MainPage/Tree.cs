using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Style.Type.MainPage
{
    public class Tree
    {
        [JsonPropertyName("width")]
        public int Width { get; set; } = 250;
    }
}
