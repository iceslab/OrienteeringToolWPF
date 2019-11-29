using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrienteeringToolWPF.Utils
{
    public sealed class Logger
    {
        enum VerbosityLevel
        {
            NONE,
            ERROR,
            WARN,
            INFO,
            DEBUG
        }

        private static Logger instance = null;
        private static readonly object padlock = new object();
        private readonly StreamWriter sw;

        Logger()
        {
            var fileName = Directory.GetCurrentDirectory() +
                DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") +
                ".log";
            File.WriteAllText(fileName, ""); // Truncate file
            sw = new StreamWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write))
            {
                AutoFlush = true
            };

            Verbosity = VerbosityLevel.DEBUG;
        }

        public static Logger Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Logger();
                    }
                    return instance;
                }
            }
        }

        private static VerbosityLevel Verbosity
        {
            get
            {
                return Verbosity;
            }

            set
            {
                Verbosity = value;
            }
        }

        private static void WriteMessage(string severity, string message)
        {
            Instance.sw.WriteLine(string.Format("{0} [{1}]: {2}", severity, GetTimestamp(), message));
        }

        public static void Debug(string message)
        {
            if (Verbosity >= VerbosityLevel.DEBUG)
            {
                WriteMessage("DEBUG", message);
            }
        }

        public static void Info(string message)
        {
            if (Verbosity >= VerbosityLevel.INFO)
            {
                WriteMessage("INFO ", message);
            }
        }

        public static void Warn(string message)
        {
            if (Verbosity >= VerbosityLevel.WARN)
            {
                WriteMessage("WARN ", message);
            }
        }

        public static void Error(string message)
        {
            if (Verbosity >= VerbosityLevel.ERROR)
            {
                WriteMessage("ERROR", message);
            }
        }

        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.tt");
        }
    }
}
