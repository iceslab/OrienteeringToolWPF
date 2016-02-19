//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
namespace GecoSI.Net
{
    public interface ISiPort
    {
        SiMessageQueue CreateMessageQueue();

        ICommWriter CreateWriter();

        void SetupHighSpeed();

        void SetupLowSpeed();

        void Close();
    }
}