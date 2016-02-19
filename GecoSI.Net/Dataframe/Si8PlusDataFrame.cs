//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using GecoSI.Net.Internal;

namespace GecoSI.Net.Dataframe
{
    public class Si8PlusDataFrame : Si6PlusAbstractDataFrame

    {
        public static readonly int PAGE_SIZE = 4;

        private static readonly int SINUMBER_PAGE = 6*PAGE_SIZE;

        public static readonly int NB_PUNCHES_INDEX = 5*PAGE_SIZE + 2;


        private readonly SiPlusSeries siSeries;

        public Si8PlusDataFrame(SiMessage[] dataMessages) : base(dataMessages)
        {
            siSeries = ExtractSiSeries();
        }

        protected SiPlusSeries ExtractSiSeries()
        {
            switch (ByteAt(SINUMBER_PAGE) & 15)
            {
                case 2:
                    return SiPlusSeries.SI8_SERIES;
                case 1:
                    return SiPlusSeries.SI9_SERIES;
                case 4:
                    return SiPlusSeries.PCARD_SERIES;
                case 15:
                    return SiPlusSeries.SI10_PLUS_SERIES;
                default:
                    return SiPlusSeries.UNKNOWN_SERIES;
            }
        }

        protected override int SiNumberIndex()
        {
            return SINUMBER_PAGE + 1;
        }

        protected override int StartTimeIndex()
        {
            return 3*PAGE_SIZE;
        }

        protected override int FinishTimeIndex()
        {
            return 4*PAGE_SIZE;
        }

        protected override int CheckTimeIndex()
        {
            return 2*PAGE_SIZE;
        }

        protected override int NbPunchesIndex()
        {
            return NB_PUNCHES_INDEX;
        }

        protected override SiPunch[] ExtractPunches(long startTime)
        {
            var punches = new SiPunch[RawNbPunches()];
            int punchesStart = siSeries.PunchesPageStartIndex;
            long refTime = startTime;
            for (int i = 0; i < punches.Length; i++)
            {
                int punchIndex = (punchesStart + i)*PAGE_SIZE;
                long punchTime = AdvanceTimePast(ExtractFullTime(punchIndex), refTime);
                punches[i] = new SiPunch(ExtractCode(punchIndex), punchTime);
                refTime = NewRefTime(refTime, punchTime);
            }
            return punches;
        }

        public override string SiSeries
        {
            get { return siSeries.Ident; }
        }
    }
}