using System.Net.Http;

namespace Niconicome.Models.Domain.Network
{
    public interface INetWorkHelper
    {
        string GetHttpStatusForLog(HttpResponseMessage res);
    }

    public class NetWorkHelper:INetWorkHelper
    {
        /// <summary>
        /// ログ用のメッセージを取得する
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public string GetHttpStatusForLog(HttpResponseMessage res)
        {
            return $"status:{res.StatusCode}, status_code:{(int)res.StatusCode}";
        }

    }
}
