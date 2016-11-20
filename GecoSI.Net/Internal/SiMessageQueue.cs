//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using System.Collections.Concurrent;
using GecoSI.Net.Internal;

namespace GecoSI.Net
{
    public class SiMessageQueue : BlockingCollection<SiMessage>
    {
        private readonly long defaultTimeout;

        public SiMessageQueue(int capacity) : base(capacity)
        {
        }

        public SiMessageQueue(int capacity, long defaultTimeout) : base(capacity)
        {
            this.defaultTimeout = defaultTimeout;
        }

        public SiMessage TimeoutPoll()
        {
            SiMessage message;
            if (!TryTake(out message, new TimeSpan(0, 0, 0, 0, (int) defaultTimeout)))
            {
                // Przy niektórych chipach wychodzi timeout, a w wersji Javowej biblioteki nie występuje
                throw new TimeoutException();
            }

            if (message != null)
            {
                return message;
            }
            throw new TimeoutException();
        }
    }
}