using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Models competitor
/// </summary>
namespace OrienteeringToolWPF.Model
{
    public enum GenderEnum : long
    {
        MALE = 0L,
        FEMALE = 1L
    }

    public class Competitor : BaseModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public long Chip { get; set; }
        public long RelayId { get; set; }
        public long Class { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
