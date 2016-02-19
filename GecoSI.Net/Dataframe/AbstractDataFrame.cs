//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;

namespace GecoSI.Net.Dataframe
{
    public abstract class AbstractDataFrame : ByteFrame, ISiDataFrame
    {
        private  const string TIME_FORMATTER = "H:mm:ss";
        public  const long NO_TIME = -1;
        protected long checkTime;

        protected long finishTime;

        protected SiPunch[] punches;
        protected String siNumber;
        protected long startTime;

        public string SiNumber
        {
            get { return siNumber; }
        }


        public long StartTime
        {
            get { return startTime; }
        }


        public long FinishTime
        {
            get { return finishTime; }
        }


        public long CheckTime
        {
            get { return checkTime; }
        }


        public int NbPunches
        {
            get { return punches.Length; }
        }


        public SiPunch[] Punches
        {
            get { return punches; }
        }

        public void PrintString()
        {
            Console.WriteLine("{0}: {1} ", SiSeries, SiNumber);
            Console.WriteLine("(Start: {0} ", FormatTime(StartTime));
            Console.WriteLine(" - Finish: {0}", FormatTime(FinishTime));
            Console.WriteLine(" - Check: {0})", FormatTime(CheckTime));
            Console.WriteLine("Punches: {0}", NbPunches);
            for (int i = 0; i < NbPunches; i++)
            {
                Console.WriteLine("{0}: {1} {2} - ", i, Punches[i].Code, FormatTime(Punches[i].Timestamp));
            }
            Console.WriteLine("");
        }

        public abstract string SiSeries { get; }

        public abstract ISiDataFrame StartingAt(long zerohour);

        public String FormatTime(long timestamp)
        {
            if (timestamp == NO_TIME)
            {
                return "no time";
            }
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(timestamp);
            return dtDateTime.ToString(TIME_FORMATTER);
        }
    }
}