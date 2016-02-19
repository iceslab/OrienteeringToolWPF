//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;

namespace GecoSI.Net.Dataframe
{
    public class SiPlusSeries
    {
        public static readonly SiPlusSeries SI8_SERIES = new SiPlusSeries("SiCard 8", 34);
        public static readonly SiPlusSeries SI9_SERIES = new SiPlusSeries("SiCard 9", 14);
        public static readonly SiPlusSeries SI10_PLUS_SERIES = new SiPlusSeries("SiCard 10/11/SIAC", 32);
        public static readonly SiPlusSeries PCARD_SERIES = new SiPlusSeries("pCard", 44);
        public static readonly SiPlusSeries UNKNOWN_SERIES = new SiPlusSeries("Unknown", 0);

        private readonly String ident;
        private readonly int punchesPageIndex;

        private SiPlusSeries(String ident, int punchesPageIndex)
        {
            this.ident = ident;
            this.punchesPageIndex = punchesPageIndex;
        }

        public string Ident
        {
            get { return ident; }
        }

        public int PunchesPageStartIndex
        {
            get { return punchesPageIndex; }
        }
    }
}