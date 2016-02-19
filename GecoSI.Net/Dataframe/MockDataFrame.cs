//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;

namespace GecoSI.Net.Dataframe
{
    public class MockDataFrame : AbstractDataFrame
    {
        public MockDataFrame(String siNumber, long checkTime, long startTime, long finishTime, SiPunch[] punches)
        {
            this.siNumber = siNumber;
            this.checkTime = checkTime;
            this.startTime = startTime;
            this.finishTime = finishTime;
            this.punches = punches;
        }


        public override ISiDataFrame StartingAt(long zerohour)
        {
            return this;
        }


        public override string SiSeries
        {
            get { return "Mock Sicard"; }
        }
    }
}