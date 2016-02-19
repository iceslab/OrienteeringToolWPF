//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
namespace GecoSI.Net
{
    public abstract class Writer
    {
        public abstract void Close();

        public abstract void Flush();

        public abstract void Write(char[] cbuf, int off, int len);

        public void Write(string message)
        {
            Write(message.ToCharArray(), 0, message.Length);
        }
    }
}