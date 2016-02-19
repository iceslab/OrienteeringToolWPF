//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using GecoSI.Net.Dataframe;

namespace GecoSI.Net
{
    public interface ISiListener
    {
        void HandleEcard(ISiDataFrame dataFrame);

        void Notify(CommStatus status);

        void Notify(CommStatus errorStatus, String errorMessage);

        bool OnEcardDown(string siNumber);

        //string Status { get; set; }
    }
}