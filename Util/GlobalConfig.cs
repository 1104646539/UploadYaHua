using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uploadyahua.Util
{
    public class GlobalConfig : JsonConfigBase
    {
        private static GlobalConfig _instance;

        // 单例模式
        public static GlobalConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GlobalConfig();
                }
                return _instance;
            }
        }
 /// <summary>
        /// IP
        /// </summary>
        private string ip = "";
        public string IP
        {
            get => ip;
            set
            {
                if (ip != value)
                {
                    ip = value;
                    MarkDirty();
                }
            }
        }
        /// <summary>
        /// 端口
        /// </summary>
        private string port = "8866";
        public string Port
        {
            get => port;
            set
            {
                if (port != value)
                {
                    port = value;
                    MarkDirty();
                }
            }
        }
        /// <summary>
        /// 点击关闭后是否最小化到托盘
        /// </summary>
        private bool minimize = false;
        public bool Minimize
        {
            get => minimize;
            set
            {
                if (minimize != value)
                {
                    minimize = value;
                    MarkDirty();
                }
            }
        }
        /// <summary>
        /// 是否开启简单模式
        /// </summary>
        private bool sampleMode = false;
        public bool SampleMode
        {
            get => sampleMode;
            set
            {
                if (sampleMode != value)
                {
                    sampleMode = value;
                    MarkDirty();
                }
            }
        }
         /// <summary>
        /// 是否开启自动打印
        /// </summary>
        private string printer = "";
        public string Printer
        {
            get => printer;
            set
            {
                if (printer != value)
                {
                    printer = value;
                    MarkDirty();
                }
            }
        }
        /// <summary>
        /// 是否开机启动
        /// </summary>
        private bool autoStartup = false;
        public bool AutoStartup
        {
            get => autoStartup;
            set
            {
                if (autoStartup != value)
                {
                    autoStartup = value;
                    MarkDirty();
                }
            }
        }

        private GlobalConfig() : base()
        {
        }
    }
}
