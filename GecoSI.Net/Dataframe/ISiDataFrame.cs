//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;

namespace GecoSI.Net.Dataframe
{
    public interface ISiDataFrame
    {
        ISiDataFrame StartingAt(long zerohour);

        int NbPunches { get; }

        string SiNumber { get; }

        string SiSeries { get; }

        long StartTime { get; }

        long FinishTime { get; }

        long CheckTime { get; }

        SiPunch[] Punches { get; }

        void PrintString();
    }
}