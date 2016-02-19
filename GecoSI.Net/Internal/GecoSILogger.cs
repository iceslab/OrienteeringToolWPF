//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using System.Configuration;
using System.IO;

namespace GecoSI.Net
{
    public class GecoSiLogger
    {
        private static Writer logger;

        public static void Open()
        {
            if (logger != null)
            {
                Close();
            }
            String logProp = ConfigurationManager.AppSettings["GECOSI_LOG"] ?? "FILE";
            if (logProp.Equals("FILE"))
            {
                OpenFileLogger();
            }
            else if (logProp.Equals("NONE"))
            {
                OpenNullLogger();
            }
            else if (logProp.Equals("OUTSTREAM"))
            {
                OpenOutStreamLogger();
            }
        }

        public static void OpenFileLogger()
        {
            try
            {
                logger = new FileWriter("gecosi.log");
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
                OpenOutStreamLogger();
            }
        }


        public static void OpenNullLogger()
        {
            logger = new NullWriter();
        }

        public static void OpenOutStreamLogger()
        {
            logger = new OutStreamWriter();
        }

        public static void Open(String header)
        {
            Open();
            Log(header, "");
        }

        public static void Log(String header, String message)
        {
            try
            {
                logger.Write(String.Format("{0} {1}\n", header, message));
                logger.Flush();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
        }

        public static void LogTime(String message)
        {
            Log(new DateTime().ToString(), message);
        }

        public static void StateChanged(String message)
        {
            Log("-->", message);
        }

        public static void Info(String message)
        {
            Log("[Info]", message);
        }

        public static void Debug(String message)
        {
            Log("[Debug]", message);
        }

        public static void Warning(String message)
        {
            Log("[Warning]", message);
        }

        public static void Error(String message)
        {
            Log("[Error]", message);
        }

        public static void Close()
        {
            try
            {
                logger.Close();
                logger = null;
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
        }

        public class NullWriter : Writer
        {
            public override void Close()
            {
            }

            public override void Flush()
            {
            }

            public override void Write(char[] cbuf, int off, int len)
            {
            }
        }

        public class OutStreamWriter : Writer
        {
            public override void Close()
            {
            }

            public override void Flush()
            {
            }

            public override void Write(char[] cbuf, int off, int len)
            {
                for (int i = off; i < off + len; i++)
                {
                    Console.Write(cbuf[i]);
                }
            }
        }
    }
}