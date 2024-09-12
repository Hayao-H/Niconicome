using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Server.API.NG.V1
{
    public interface INGHandler
    {
        /// <summary>
        /// リクエストを処理する
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        IAttemptResult Handle(HttpListenerRequest request, HttpListenerResponse response);
    }

    public class NGHandler : INGHandler
    {
        public NGHandler(ISettingsContainer settingsContainer)
        {
            this._settingsContainer = settingsContainer;
        }


        private ISettingInfo<List<string>>? _ngWords;

        private ISettingInfo<List<string>>? _ngUsers;

        private ISettingInfo<List<string>>? _ngCommands;

        private readonly ISettingsContainer _settingsContainer;

        public IAttemptResult Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            RequestType type = this.GetRequestType(request.Url!.ToString());

            if (type == RequestType.Get)
            {
                this.HandleGet(response);
                return AttemptResult.Succeeded();
            }
            else if (type == RequestType.Set && request.HttpMethod == "POST")
            {
                this.HandleSet(request, response);
                return AttemptResult.Succeeded();
            }
            else if (type == RequestType.Delete && request.HttpMethod == "POST")
            {
                this.HandleDelete(request, response);
                return AttemptResult.Succeeded();
            }
            else
            {
                this.WriteMessage($"Invalid request. {request.Url}", (int)HttpStatusCode.BadRequest, response);
                return AttemptResult.Succeeded();
            }
        }

        private void HandleGet(HttpListenerResponse response)
        {
            this.SetSettings();

            var content = JsonParser.Serialize(new { Words = this._ngWords!.Value, Users = this._ngUsers!.Value, Commands = this._ngCommands!.Value });
            var writer = new StreamWriter(response.OutputStream, encoding: Encoding.UTF8);
            writer.Write(content);
            writer.Flush();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "application/json";
        }

        private void HandleSet(HttpListenerRequest request, HttpListenerResponse response)
        {
            this.SetSettings();

            var reader = new StreamReader(request.InputStream, encoding: Encoding.UTF8);
            string body = reader.ReadToEnd();

            Request requestObject = JsonParser.DeSerialize<Request>(body);
            NGType type = this.GetNGType(requestObject);

            if (type == NGType.Word)
            {
                var ngs = this._ngWords!.Value;
                this._ngWords.Value = ngs.AddUnique(requestObject.Value);
            }
            else if (type == NGType.User)
            {
                var ngs = this._ngUsers!.Value;
                this._ngUsers.Value = ngs.AddUnique(requestObject.Value);
            }
            else if (type == NGType.Command)
            {
                var ngs = this._ngCommands!.Value;
                this._ngCommands.Value = ngs.AddUnique(requestObject.Value);
            }

            Debug.WriteLine(body);

            // Do something
            this.WriteMessage("Hello World!!", (int)HttpStatusCode.OK, response);
        }

        private void HandleDelete(HttpListenerRequest request, HttpListenerResponse response)
        {
            this.SetSettings();

            var reader = new StreamReader(request.InputStream, encoding: Encoding.UTF8);
            string body = reader.ReadToEnd();

            Request requestObject = JsonParser.DeSerialize<Request>(body);
            NGType type = this.GetNGType(requestObject);

            if (type == NGType.Word)
            {
                var ngs = this._ngWords!.Value.Where(x => x != requestObject.Value).ToList();
                this._ngWords.Value = ngs;
            }
            else if (type == NGType.User)
            {
                var ngs = this._ngUsers!.Value.Where(x => x != requestObject.Value).ToList();
                this._ngUsers.Value = ngs;
            }
            else if (type == NGType.Command)
            {
                var ngs = this._ngCommands!.Value.Where(x => x != requestObject.Value).ToList();
                this._ngCommands.Value = ngs;
            }

            Debug.WriteLine(body);

            // Do something
            this.WriteMessage("{\"status\":200,\"message\":\"success\"}", (int)HttpStatusCode.OK, response);
        }

        private RequestType GetRequestType(string url)
        {
            if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/api/ng/v1/get/?"))
            {
                return RequestType.Get;
            }
            else if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/api/ng/v1/set/?"))
            {
                return RequestType.Set;
            }
            else if (Regex.IsMatch(url, @"https?://.+:\d+/niconicome/api/ng/v1/delete/?"))
            {
                return RequestType.Delete;
            }

            return RequestType.None;
        }

        private void WriteMessage(string message, int statusCode, HttpListenerResponse response)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<!DOCTYPE html>");
            builder.AppendLine("<html>");
            builder.AppendLine("<head>");
            builder.AppendLine($"<title>Niconicome | {statusCode}</title>");
            builder.AppendLine("<meta charset=\"utf-8\">");
            builder.AppendLine("</head>");
            builder.AppendLine("<body>");
            builder.AppendLine(message);
            builder.AppendLine("</body>");
            builder.AppendLine("</html>");

            byte[] content = Encoding.UTF8.GetBytes(builder.ToString());
            response.OutputStream.Write(content, 0, content.Length);
            response.StatusCode = statusCode;
            response.ContentType = "text/html";
        }

        private void SetSettings()
        {
            if (this._ngWords is null)
            {
                var result = this._settingsContainer.GetSetting(SettingNames.NGWords, new List<string>());
                if (result.IsSucceeded && result.Data is not null)
                {
                    this._ngWords = result.Data;
                }
            }

            if (this._ngUsers is null)
            {
                var result = this._settingsContainer.GetSetting(SettingNames.NGUsers, new List<string>());
                if (result.IsSucceeded && result.Data is not null)
                {
                    this._ngUsers = result.Data;
                }
            }

            if (this._ngCommands is null)
            {
                var result = this._settingsContainer.GetSetting(SettingNames.NGCommands, new List<string>());
                if (result.IsSucceeded && result.Data is not null)
                {
                    this._ngCommands = result.Data;
                }
            }
        }

        private NGType GetNGType(Request request)
        {
            if (request.Type == "user")
            {
                return NGType.User;
            }
            else if (request.Type == "command")
            {
                return NGType.Command;
            }
            else
            {
                return NGType.Word;
            }
        }

        private class Request
        {
            public string Type { get; set; } = string.Empty;

            public string Value { get; set; } = string.Empty;
        }

        private enum NGType
        {
            Word,
            User,
            Command,
        }

        private enum RequestType
        {
            Set,
            Get,
            Delete,
            None,
        }
    }
}
