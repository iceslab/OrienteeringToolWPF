//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using GecoSI.Net.Internal;

namespace GecoSI.Net.Adapter.SerialPort
{
    internal class SerialPortCommWriter : ICommWriter
    {
        private readonly System.IO.Ports.SerialPort port;

        public SerialPortCommWriter(System.IO.Ports.SerialPort port)
        {
            // TODO: Complete member initialization
            this.port = port;
        }

        public void Write(SiMessage message)
        {
            GecoSiLogger.Log("SEND", message.ToString());
            byte[] buff = message.Sequence();
            port.Write(buff, 0, buff.Length);
        }
    }
}