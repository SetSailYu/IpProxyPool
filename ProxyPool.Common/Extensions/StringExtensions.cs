using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Common.Extensions
{
    /// <summary>
    /// string扩展
    /// </summary>
    public static class StringExtensions
    {
        public static int ToInt32(this string source)
        {
            try
            {
                return Convert.ToInt32(source);
            }catch (Exception e)
            {
                return 0;
            }
        }


    }
}
