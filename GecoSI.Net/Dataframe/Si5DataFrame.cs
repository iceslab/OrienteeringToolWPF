//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using System.Linq;
using GecoSI.Net.Internal;

namespace GecoSI.Net.Dataframe
{
    public class Si5DataFrame : SiAbstractDataFrame
    {
        private const int SI5_TIMED_PUNCHES = 30;

        public Si5DataFrame()
        {
        }

        public Si5DataFrame(SiMessage message)
        {
            dataFrame = ExtractDataFrame(message);
            siNumber = ExtractSiNumber();
        }

        protected byte[] ExtractDataFrame(SiMessage message)
        {
            return message.Sequence().Skip(5).Take(133 - 5).ToArray();
        }


        public override ISiDataFrame StartingAt(long zerohour)
        {
            startTime = AdvanceTimePast(RawStartTime(), zerohour);
            checkTime = AdvanceTimePast(RawCheckTime(), zerohour);
            long refTime = NewRefTime(zerohour, startTime);
            punches = ComputeShiftedPunches(refTime);
            if (punches.Length > 0)
            {
                SiPunch lastTimedPunch = punches[NbTimedPunches(punches) - 1];
                refTime = NewRefTime(refTime, lastTimedPunch.Timestamp);
            }
            finishTime = AdvanceTimePast(RawFinishTime(), refTime);
            return this;
        }

        public long AdvanceTimePast(long timestamp, long refTime)
        {
            return AdvanceTimePast(timestamp, refTime, TWELVE_HOURS);
        }

        private SiPunch[] ComputeShiftedPunches(long startTime)
        {
            int nbPunches = RawNbPunches();
            var punches = new SiPunch[nbPunches];
            int nbTimedPunches = Si5DataFrame.NbTimedPunches(punches);
            long refTime = startTime;
            for (int i = 0; i < nbTimedPunches; i++)
            {
                // shift each punch time after the previous
                long punchTime = AdvanceTimePast(GetPunchTime(i), refTime);
                punches[i] = new SiPunch(GetPunchCode(i), punchTime);
                refTime = NewRefTime(refTime, punchTime);
            }
            for (int i = 0; i < nbPunches - SI5_TIMED_PUNCHES; i++)
            {
                punches[i + SI5_TIMED_PUNCHES] = new SiPunch(GetNoTimePunchCode(i), NO_TIME);
            }
            return punches;
        }

        private static int NbTimedPunches(SiPunch[] punches)
        {
            return Math.Min(punches.Length, SI5_TIMED_PUNCHES);
        }

        protected String ExtractSiNumber()
        {
            int siNumber = WordAt(0x04);
            int cns = ByteAt(0x06);
            if (cns > 0x01)
            {
                siNumber = siNumber + cns*100000;
            }
            return siNumber.ToString();
        }

        protected int RawNbPunches()
        {
            return ByteAt(0x17) - 1;
        }

        private long RawStartTime()
        {
            return TimestampAt(0x13);
        }

        private long RawFinishTime()
        {
            return TimestampAt(0x15);
        }

        private long RawCheckTime()
        {
            return TimestampAt(0x19);
        }

        protected int PunchOffset(int i)
        {
            return 0x21 + (i/5)*0x10 + (i%5)*0x03;
        }

        protected int GetPunchCode(int i)
        {
            return ByteAt(PunchOffset(i));
        }

        protected int GetNoTimePunchCode(int i)
        {
            return ByteAt(0x20 + i*0x10);
        }

        protected long GetPunchTime(int i)
        {
            return TimestampAt(PunchOffset(i) + 1);
        }


        public override string SiSeries
        {
            get { return "SiCard 5"; }
        }
    }
}