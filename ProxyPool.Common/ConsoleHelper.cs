using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Common
{
    /// <summary>
    /// 设置控制台输出字体颜色
    /// </summary>
    public class ConsoleHelper
    {
        /// <summary>
        /// 成功输出
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteSuccessLog(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} -> {msg}");
            Console.ResetColor();
        }

        /// <summary>
        /// 错误输出
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteErrorLog(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} -> {msg}");
            Console.ResetColor();
        }

        /// <summary>
        /// 提示输出
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteHintLog(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} -> {msg}");
            Console.ResetColor();
        }

    }
}
