//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using GecoSI.Net.Internal;

namespace GecoSI.Net.Dataframe
{
    public abstract class Si6PlusAbstractDataFrame : SiAbstractDataFrame
    {
        public Si6PlusAbstractDataFrame(SiMessage[] dataMessages)
        {
            dataFrame = ExtractDataFrame(dataMessages);
            siNumber = ExtractSiNumber();
        }

        protected byte[] ExtractDataFrame(SiMessage[] dataMessages)
        {
            var dataFrame = new byte[dataMessages.Length*128];
            for (int i = 0; i < dataMessages.Length; i++)
            {
                Array.Copy(dataMessages[i].Sequence(), 6, dataFrame, i*128, 128);
            }
            return dataFrame;
        }

        public override ISiDataFrame StartingAt(long zerohour)
        {
            startTime = AdvanceTimePast(ExtractStartTime(), zerohour);
            checkTime = AdvanceTimePast(ExtractCheckTime(), zerohour);
            long refTime = NewRefTime(zerohour, startTime);
            punches = ExtractPunches(refTime);
            if (punches.Length > 0)
            {
                SiPunch lastPunch = punches[punches.Length - 1];
                refTime = NewRefTime(refTime, lastPunch.Timestamp);
            }
            finishTime = AdvanceTimePast(ExtractFinishTime(), refTime);
            return this;
        }

        public long AdvanceTimePast(long timestamp, long refTime)
        {
            return AdvanceTimePast(timestamp, refTime, ONE_DAY);
        }

        protected String ExtractSiNumber()
        {
            return Block3At(SiNumberIndex()).ToString();
        }

        protected long ExtractStartTime()
        {
            return ExtractFullTime(StartTimeIndex());
        }

        protected long ExtractFinishTime()
        {
            return ExtractFullTime(FinishTimeIndex());
        }

        protected long ExtractCheckTime()
        {
            return ExtractFullTime(CheckTimeIndex());
        }

        protected int RawNbPunches()
        {
            return ByteAt(NbPunchesIndex());
        }

        protected long ExtractFullTime(int pageStart)
        {
//		int tdByte = byteAt(pageStart);
//		int weekCounter = (tdByte & 48) >> 4;
//		int numDay = (tdByte & 14) >> 1;
            int pmFlag = ByteAt(pageStart) & 1;
            return ComputeFullTime(pmFlag, TimestampAt(pageStart + 2));
        }

        public long ComputeFullTime(int pmFlag, long twelveHoursTime)
        {
            if (twelveHoursTime == NO_SI_TIME)
            {
                return NO_SI_TIME;
            }
            return pmFlag*TWELVE_HOURS + twelveHoursTime;
        }

        protected int ExtractCode(int punchIndex)
        {
            int codeHigh = (ByteAt(punchIndex) & 192) << 2;
            int code = codeHigh + ByteAt(punchIndex + 1);
            return code;
        }

        protected abstract int SiNumberIndex();

        protected abstract int StartTimeIndex();

        protected abstract int FinishTimeIndex();

        protected abstract int CheckTimeIndex();

        protected abstract int NbPunchesIndex();

        protected abstract SiPunch[] ExtractPunches(long startTime);
    }
}