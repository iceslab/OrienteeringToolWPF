namespace OrienteeringToolWPF.Enumerations
{
    public enum Correctness
    {
        // Punch has not been checked for corectness
        NOT_CHECKED,
        // Punch is present and in correct order according to route
        CORRECT,
        // Punch is present and in invalid order according to route
        PRESENT,
        // Punch is not present according to route
        INVALID
    }
}
