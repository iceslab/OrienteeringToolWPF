//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using GecoSI.Net.Internal;

namespace GecoSI.Net
{
    public class InvalidMessage : Exception
    {
        private readonly SiMessage receivedMessage;

        public InvalidMessage(SiMessage receivedMessage)
        {
            this.receivedMessage = receivedMessage;
        }

        public SiMessage ReceivedMessage()
        {
            return receivedMessage;
        }
    }
}