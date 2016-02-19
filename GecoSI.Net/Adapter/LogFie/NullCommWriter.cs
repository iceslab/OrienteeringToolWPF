//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using GecoSI.Net.Internal;

namespace GecoSI.Net.Adapter.LogFie
{
    public class NullCommWriter : ICommWriter
    {
        public void Write(SiMessage message)
        {
        }
    }
}