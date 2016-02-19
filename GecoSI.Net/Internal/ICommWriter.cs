//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using GecoSI.Net.Internal;

namespace GecoSI.Net
{
    public interface ICommWriter
    {
        void Write(SiMessage message);
    }
}