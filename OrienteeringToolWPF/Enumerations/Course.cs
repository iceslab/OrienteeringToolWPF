using System.ComponentModel;

namespace OrienteeringToolWPF.Enumerations
{
    public enum Course : long
    {
        [Description("Czas startu to czas odbity na chipie")]
        START_ON_CHIP = 0L,
        [Description("Czas startu to czas mety poprzednika")]
        START_CALCULATED = 1L
    }
}
