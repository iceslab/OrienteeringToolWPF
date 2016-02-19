//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;

namespace GecoSI.Net.Adapter.LogFie
{
    public class LogFilePort : ISiPort
    {
        private readonly String logFilename;

        public LogFilePort(String logFilename)
        {
            this.logFilename = logFilename;
        }


        public SiMessageQueue CreateMessageQueue()
        {
            return new LogFileCommReader(logFilename).CreateMessageQueue();
        }


        public ICommWriter CreateWriter()
        {
            return new NullCommWriter();
        }


        public void SetupHighSpeed()
        {
        }


        public void SetupLowSpeed()
        {
        }


        public void Close()
        {
        }
    }
}