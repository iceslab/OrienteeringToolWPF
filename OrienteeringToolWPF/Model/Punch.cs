using GecoSI.Net.Dataframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.Model
{
    public class Punch : BaseModel
    {
        public long? Id { get; set; }
        public long Chip { get; set; }
        public long Code { get; set; }
        public long Timestamp { get; set; }

        public Punch() : base()
        {
            Id = null;
            Chip = 0;
            Code = 0;
            Timestamp = 0;
        }

        public Punch(SiPunch sp, long Chip = 0) : base()
        {
            Id = null;
            this.Chip = Chip;
            Code = sp.Code;
            Timestamp = sp.Timestamp;
        }

        public static List<Punch> Parse(SiPunch[] siPunches, long Chip = 0)
        {
            if (siPunches != null)
            {
                List<Punch> punches = new List<Punch>(siPunches.Length);
                foreach (var sp in siPunches)
                    punches.Add(new Punch(sp, Chip));

                return punches;
            }
            else
                return new List<Punch>();
        }

        public static void SetChipNumber(ref List<Punch> punches, long Chip)
        {
            foreach (var p in punches)
                p.Chip = Chip;
        }
    }
}
