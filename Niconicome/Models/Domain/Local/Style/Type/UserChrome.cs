using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Style.Type
{
    public class UserChrome
    {
        [JsonPropertyName("mainPage")]
        public MainPage.MainPage MainPage { get; set; } = new();
    }
}
