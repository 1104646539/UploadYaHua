using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uploadyahua.Util
{
    public class SystemGlobal
    {
        public const bool IsDebugCode = true;
        public static Encoding Encoding = Encoding.GetEncoding("gbk");
        public static string KeyName = "upload_yahua";
        public static bool Statup = true;//是否开机自启
    }
}
