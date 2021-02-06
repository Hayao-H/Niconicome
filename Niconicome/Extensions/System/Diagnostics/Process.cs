using System.Diagnostics;

namespace Niconicome.Extensions.System.Diagnostics
{
    class ProcessEx
    {
        /// <summary>
        /// シェルから立ち上げる
        /// </summary>
        /// <param name="arg"></param>
        public static void StartWithShell(string arg)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = arg;
            p.Start();
        }

        /// <summary>
        /// シェルから立ち上げる
        /// </summary>
        /// <param name="arg"></param>
        public static void StartWithShell(string filename,string arg)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = filename;
            p.StartInfo.Arguments = arg;
            p.Start();
        }
    }
}
