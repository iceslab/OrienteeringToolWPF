using GecoSI.Net.Dataframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Models basic info acquired form chip excluding punches
/// </summary>
namespace OrienteeringToolWPF.Model
{
    public class Result : BaseModel
    {
        public long Chip { get; set; }
        public long StartTime { get; set; }
        public long CheckTime { get; set; }
        public long FinishTime { get; set; }

        public Result()
        {
            Chip = 0;
            StartTime = 0;
            CheckTime = 0;
            FinishTime = 0;
        }

        public Result(AbstractDataFrame abf) : this()
        {
            long n = 0;
            long.TryParse(abf.SiNumber, out n);
            Chip = n;
            StartTime = abf.StartTime;
            CheckTime = abf.CheckTime;
            FinishTime = abf.FinishTime;
        }
    }
}
