//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using GecoSI.Net.Internal;

namespace GecoSI.Net.Dataframe
{
    public class Si6DataFrame : Si6PlusAbstractDataFrame
    {
        public const int PAGE_SIZE = 16;

        public const int DOUBLE_WORD = 4;

        public const int NB_PUNCHES_INDEX = 1*PAGE_SIZE + 2;

        public Si6DataFrame(SiMessage[] dataMessages) : base(dataMessages)
        {
        }

        protected override int SiNumberIndex()
        {
            return 2*DOUBLE_WORD + 3;
        }

        protected override int StartTimeIndex()
        {
            return 1*PAGE_SIZE + 2*DOUBLE_WORD;
        }

        protected override int FinishTimeIndex()
        {
            return 1*PAGE_SIZE + 1*DOUBLE_WORD;
        }

        protected override int CheckTimeIndex()
        {
            return 1*PAGE_SIZE + 3*DOUBLE_WORD;
        }

        protected override int NbPunchesIndex()
        {
            return NB_PUNCHES_INDEX;
        }

        protected int PunchesStartIndex()
        {
            return 8*PAGE_SIZE;
        }

        protected override SiPunch[] ExtractPunches(long startTime)
        {
            var punches = new SiPunch[RawNbPunches()];
            int punchesStart = PunchesStartIndex();
            long refTime = startTime;
            for (int i = 0; i < punches.Length; i++)
            {
                int punchIndex = punchesStart + (DOUBLE_WORD*i);
                long punchTime = AdvanceTimePast(ExtractFullTime(punchIndex), refTime);
                punches[i] = new SiPunch(ExtractCode(punchIndex), punchTime);
                refTime = NewRefTime(refTime, punchTime);
            }
            return punches;
        }


        public override string SiSeries
        {
            get { return "SiCard 6"; }
        }
    }
}