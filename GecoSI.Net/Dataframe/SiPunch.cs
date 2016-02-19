//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
namespace GecoSI.Net.Dataframe
{
    public class SiPunch
    {
        public SiPunch(int code, long timestamp)
        {
            Code = code;
            Timestamp = timestamp;
        }

        public int Code { get; private set; }

        public long Timestamp { get; private set; }
    }
}