using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.Model
{
    public class Result : BaseModel
    {
        public long Chip { get; set; }
        public long StartTime { get; set; }
        public long CheckTime { get; set; }
        public long FinishTime { get; set; }
    }
}
