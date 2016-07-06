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
    }
}
