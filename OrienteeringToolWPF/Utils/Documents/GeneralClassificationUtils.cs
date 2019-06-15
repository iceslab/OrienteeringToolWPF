using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.Utils.Documents
{
    class ClassificationCompetitor
    {
        private static readonly int PLACE_FIELD_START = 0;
        private static readonly int PLACE_FIELD_LENGTH = 3;
        private static readonly int CLUB_FIELD_START = 29;
        private static readonly int CLUB_FIELD_LENGTH = 21;
        public int Place;
        public string ClubName;

        public ClassificationCompetitor(int place, string club)
        {
            Place = place;
            ClubName = club;
        }

        public static ClassificationCompetitor GetClassificationCompetitor(string line)
        {
            ClassificationCompetitor retVal = null;
            if (line.Length >= PLACE_FIELD_START + PLACE_FIELD_LENGTH)
            {
                string placeStr = line.Substring(PLACE_FIELD_START, PLACE_FIELD_LENGTH);
                int place = 0;

                // Check if competitor line
                if (line.StartsWith("   ") || int.TryParse(placeStr, out place))
                {
                    // Extract club name
                    string club = line.Substring(CLUB_FIELD_START, CLUB_FIELD_LENGTH);
                    retVal = new ClassificationCompetitor(place, club);
                }
            }

            return retVal;
        }
    }

    class ClassificationClub : IComparable<ClassificationClub>
    {
        public string Name;
        public int CompetitorsPoints;
        public int PlacesPoints;
        public int AllPoints
        {
            get
            {
                return CompetitorsPoints + PlacesPoints;
            }
        }

        public ClassificationClub(string name)
        {
            Name = name;
        }

        public int CompareTo(ClassificationClub other)
        {
            if (AllPoints > other.AllPoints)
                return -1;
            if (AllPoints < other.AllPoints)
                return 1;
            return 0;
        }
    }

    class GeneralClassificationUtils
    {
        private static readonly int HEADER_LINES = 7;

        public static void Classify(string inFilePath, string outFilePath)
        {
            var inFile = new StreamReader(inFilePath);
            var outFile = new StreamWriter(outFilePath);
            Classify(inFile, outFile);
        }

        public static void Classify(StreamReader inFile, StreamWriter outFile)
        {
            var clubList = new List<ClassificationClub>();
            // Skip file header
            for (int i = 0; i < HEADER_LINES; i++)
            {
                inFile.ReadLine();
            }

            string line;
            while ((line = inFile.ReadLine()) != null)
            {
                ProcessLine(line, ref clubList);
            }
            inFile.Close();

            clubList.Sort();

            int idx = 1;
            outFile.WriteLine("Lp. Nazwa klubu:           Zawod.:     M-ca:    Razem:");
            foreach (var c in clubList)
            {
                outFile.WriteLine(string.Format("{0,2}. {1} {2,8} {3,9} {4,9}",
                    idx++,
                    c.Name,
                    c.CompetitorsPoints,
                    c.PlacesPoints,
                    c.AllPoints));
            }
            outFile.Flush();
            outFile.Close();
        }

        private static void ProcessLine(string line, ref List<ClassificationClub> clubList)
        {
            var cc = ClassificationCompetitor.GetClassificationCompetitor(line);
            if (cc != null)
            {
                int perCompetitorPoints = 1;
                int perPlacePoints = 0;
                var found = clubList.Find(club => club.Name == cc.ClubName);
                if (found == null)
                {
                    found = new ClassificationClub(cc.ClubName);
                    clubList.Add(found);
                }

                if (cc.Place >= 1 && cc.Place <= 6)
                {
                    perPlacePoints = 7 - cc.Place;
                }

                found.CompetitorsPoints += perCompetitorPoints;
                found.PlacesPoints += perPlacePoints;
            }
        }
    }
}
