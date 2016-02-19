using System;
using System.ComponentModel;

namespace OrienteeringToolWPF.Model
{
    public enum CourseEnum : long
    {
        [Description("Czas startu to czas odbity na chipie")]
        START_ON_CHIP = 0L,
        [Description("Czas startu to czas mety poprzednika")]
        START_CALCULATED = 1L

    }

    public class Tournament : BaseModel
    {
        public long? Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? StartedAtTime { get; set; }
        public DateTime? FinishedAtTime { get; set; }
        public string Name { get; set; }
        public CourseEnum CourseType { get; set; }
        public string Description { get; set; }

        public Tournament()
        {
            Id = null;
            StartTime = DateTime.Now;
            StartedAtTime = null;
            FinishedAtTime = null;
            Name = "";
            CourseType = CourseEnum.START_ON_CHIP;
            Description = null;
        }
    }
}
