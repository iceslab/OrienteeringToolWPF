using GecoSI.Net.Dataframe;
using OrienteeringToolWPF.Enumerations;
using System.Collections.Generic;

/// <summary>
/// Models punch - data from chip describing acquired checkpoints
/// </summary>
namespace OrienteeringToolWPF.Model
{
    public class Punch : BaseModel
    {
        public long? Id { get; set; }
        public long Chip { get; set; }
        public long Code { get; set; }
        public long Timestamp { get; set; }
        public Correctness Correctness { get; set; }
        public long DeltaStart { get; set; }

        public Punch() : base()
        {
            Id = null;
            Chip = 0;
            Code = 0;
            Timestamp = 0;
            Correctness = Correctness.NOT_CHECKED;
            DeltaStart = 0;
        }

        public Punch(SiPunch sp, long Chip = 0) : this()
        {
            this.Chip = Chip;
            Code = sp.Code;
            Timestamp = sp.Timestamp;
        }

        // Parses array of SiPunch to list of Punch objects
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

        // Sets the same provided Chip for all Punch objects in list
        public static void SetChipNumber(ref List<Punch> punches, long Chip)
        {
            foreach (var p in punches)
                p.Chip = Chip;
        }

        // Checks correctness of Punch list according to RouteStep list
        // Method assumes that Punches are sorted by Timestamp and RouteSteps by Order fields
        public static void CheckCorrectnessOrdered(ref List<Punch> punches, List<RouteStep> routeSteps)
        {
            if (punches == null)
                throw new System.ArgumentNullException(nameof(punches), "List refers to null");
            if (routeSteps == null)
                throw new System.ArgumentNullException(nameof(routeSteps), "List refers to null");

            // Associative array of RouteStep code occurences
            var routeStepsAssociative = RouteStep.GetCodeOccurenceCount(routeSteps);

            // Normal for only because of using index comparision
            for (var i = 0; i < punches.Count; i++)
            {
                // If Code matches in both lists set to CORRECT
                if (i < routeSteps.Count && punches[i].Code == routeSteps[i].Code)
                {
                    punches[i].Correctness = Correctness.CORRECT;
                    routeStepsAssociative[punches[i].Code]--;
                }
                else
                {
                    // On other cases than mentioned below Punch is invalid
                    punches[i].Correctness = Correctness.INVALID;
                }
            }

            foreach(var punch in punches)
            {
                if(punch.Correctness == Correctness.INVALID)
                {
                    long value;
                    // If element exists on route
                    if (routeStepsAssociative.TryGetValue(punch.Code, out value) == true)
                    {
                        // If element exists and was collected proper number of times
                        if (value > 0)
                        {
                            routeStepsAssociative[punch.Code]--;
                            punch.Correctness = Correctness.PRESENT;
                        }
                    }
                }
            }
        }

        // Counts number of punches of wanted type(s)
        public static uint GetNoOfCorrectPunches(List<Punch> punches, List<Correctness> correctness)
        {
            uint count = 0;
            foreach(var p in punches)
            {
                foreach(var c in correctness)
                {
                    if (p.Correctness == c)
                    {
                        count++;
                        break;
                    }
                }
            }

            return count;
        }

        #region Object overrides
        // Equality method (returns true when all fields matches)
        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other is Punch)
            {
                var p = other as Punch;
                if (Id != p.Id)
                    return false;
                if (Chip != p.Chip)
                    return false;
                if (Code != p.Code)
                    return false;
                if (Timestamp != p.Timestamp)
                    return false;
            }
            else
                return false;
            return true;
        }

        // Generates hashcode from Chip, Code, Timestamp and Id if present
        public override int GetHashCode()
        {
            var hash = Chip ^ Code ^ Timestamp;
            if (Id != null)
                hash ^= (long)Id;
            return (int)hash;
        }
        #endregion
        #region Operators
        // Lesser than operator (compares only Timestamp field)
        public static bool operator <(Punch lhs, Punch rhs)
        {
            return lhs?.Timestamp < rhs?.Timestamp;
        }

        // Greater than operator (compares only Timestamp field)
        public static bool operator >(Punch lhs, Punch rhs)
        {
            return lhs?.Timestamp > rhs?.Timestamp;
        }

        // Equal operator (compares only Timestamp field)
        public static bool operator ==(Punch lhs, Punch rhs)
        {
            return lhs?.Timestamp == rhs?.Timestamp;
        }

        // Not equal operator (compares only Timestamp field)
        public static bool operator !=(Punch lhs, Punch rhs)
        {
            return lhs?.Timestamp != rhs?.Timestamp;
        }
        #endregion
    }
}
