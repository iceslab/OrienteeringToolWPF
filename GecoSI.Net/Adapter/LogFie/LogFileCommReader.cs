//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using System.Collections.Generic;
using System.IO;
using GecoSI.Net.Internal;

namespace GecoSI.Net.Adapter.LogFie
{
    public class LogFileCommReader
    {
        private readonly List<SiMessage> messages;

        public LogFileCommReader(String filename)
        {
            messages = Read(filename, new List<SiMessage>(100));
        }

        public List<SiMessage> Read(String filename, List<SiMessage> messages)
        {
            var fileReader = new StreamReader(new FileStream(filename, FileMode.Open));
            String line = fileReader.ReadLine();
            while (line != null)
            {
                if (line.StartsWith("READ"))
                {
                    String[] bytes = line.Split(' ');
                    var seq = new byte[bytes.Length - 1];
                    for (int i = 1; i < bytes.Length; i++)
                    {
                        seq[i - 1] = Byte.Parse(bytes[i]);
                    }
                    messages.Add(new SiMessage(seq));
                }
                line = fileReader.ReadLine();
            }
            fileReader.Close();
            return messages;
        }

        public SiMessageQueue CreateMessageQueue()
        {
            var queue = new SiMessageQueue(messages.Count);
            messages.ForEach(queue.Add);
            return queue;
        }
    }
}