//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System.IO.Ports;

namespace GecoSI.Net.Adapter.SerialPort
{
    public class SerialComPort : ISiPort
    {
        private readonly System.IO.Ports.SerialPort port;


        private SerialPortCommReader reader;

        public SerialComPort(System.IO.Ports.SerialPort port)
        {
            this.port = port;
        }

        public SiMessageQueue CreateMessageQueue()
        {
            var messageQueue = new SiMessageQueue(10, 10*1000);
            reader = new SerialPortCommReader(messageQueue, port);
            return messageQueue;
        }

        public ICommWriter CreateWriter()
        {
            return new SerialPortCommWriter(port);
        }

        public void SetupHighSpeed()
        {
            SetSpeed(38400);
        }

        public void SetupLowSpeed()
        {
            SetSpeed(4800);
        }


        public void Close()
        {
            // TODO: close streams?
            port.Close();
        }

        public void SetSpeed(int baudRate)
        {
            port.BaudRate = baudRate;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
        }
    }
}