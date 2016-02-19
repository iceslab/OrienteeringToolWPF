//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using System.IO.Ports;
using System.Linq;
using GecoSI.Net.Internal;

namespace GecoSI.Net.Adapter.SerialPort
{
    internal class SerialPortCommReader
    {
        public const int MAX_MESSAGE_SIZE = 139;

        private const int METADATA_SIZE = 6;
        private readonly SiMessageQueue messageQueue;
        private readonly System.IO.Ports.SerialPort port;
        private readonly int timeoutDelay;

        private int accSize;
        private byte[] accumulator;

        private long lastTime;


        public SerialPortCommReader(SiMessageQueue messageQueue, System.IO.Ports.SerialPort port)
        {
            // TODO: Complete member initialization
            this.messageQueue = messageQueue;
            this.port = port;
            this.messageQueue = messageQueue;
            timeoutDelay = 500;
            lastTime = 0;
            port.DataReceived += port_DataReceived;
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                CheckTimeout();
                Accumulate();
                if (accSize == 1 && accumulator[0] != 0x02)
                {
                    SendMessage();
                }
                else
                {
                    CheckExpectedLength(accumulator, accSize);
                }
            }
            catch (Exception ex)
            {
                GecoSiLogger.Error(" #serialEvent# " + ex);
                ex.PrintStackTrace();
            }
        }


        private void Accumulate()
        {
            accSize += port.Read(accumulator, accSize, MAX_MESSAGE_SIZE - accSize);
        }

        private void CheckTimeout()
        {
            long currentTime = DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond;
            if (currentTime > lastTime + timeoutDelay)
            {
                ResetAccumulator();
            }
            lastTime = currentTime;
        }

        protected void CheckExpectedLength(byte[] accumulator, int accSize)
        {
            if (CompleteMessage(accumulator, accSize))
            {
                SendMessage();
            }
            else
            {
                GecoSiLogger.Debug("Fragment");
            }
        }

        protected bool CompleteMessage(byte[] answer, int nbReadBytes)
        {
            return (answer[2] & 0xFF) == nbReadBytes - METADATA_SIZE;
        }

        private void SendMessage()
        {
            QueueMessage(ExtractMessage(accumulator, accSize));
            ResetAccumulator();
        }

        private void QueueMessage(SiMessage message)
        {
            GecoSiLogger.Log("READ", message.ToString());
            messageQueue.Add(message);
        }

        private SiMessage ExtractMessage(byte[] answer, int nbBytes)
        {
            return new SiMessage(answer.Take(nbBytes).ToArray());
        }

        private void ResetAccumulator()
        {
            accumulator = new byte[MAX_MESSAGE_SIZE];
            accSize = 0;
        }
    }
}