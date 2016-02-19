//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System.IO;
using System.Linq;

namespace GecoSI.Net
{
    public class FileWriter : Writer
    {
        private readonly FileStream writer;

        public FileWriter(string path)
        {
            writer = new FileStream(path, FileMode.Append);
        }

        public override
            void Close()
        {
            writer.Close();
        }

        public override
            void Flush()
        {
            writer.Flush();
        }

        public override
            void Write(char[] cbuf, int off, int len)
        {
            writer.Write(cbuf.Select(c => (byte) c).ToArray(), off, len);
        }
    }
}