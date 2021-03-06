﻿using GecoSI.Net.Dataframe;
using OrienteeringToolWPF.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Models punch - data from chip describing acquired checkpoints
/// </summary>
namespace OrienteeringToolWPF.Model
{
    public class Punch : IComparable<Punch>
    {
        public long? Id { get; set; }
        public long Chip { get; set; }
        public long Code { get; set; }
        public long Timestamp { get; set; }
        public Correctness Correctness { get; set; }
        public long DeltaStart { get; set; }
        public long DeltaPrevious { get; set; }

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

        public void CalculateDeltaStart(long StartTime)
        {
            DeltaStart = Timestamp - StartTime;
        }

        public void CalculateDeltaPrevious(long previous)
        {
            DeltaPrevious = Timestamp - previous;
        }

        #region Static methods
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
        public static void CheckCorrectnessSorted(ref List<Punch> punches, List<RouteStep> routeSteps, bool allowRepeats = true)
        {
            if (punches == null)
                throw new ArgumentNullException(nameof(punches), "List refers to null");
            if (routeSteps == null)
                throw new ArgumentNullException(nameof(routeSteps), "List refers to null");

            // Associative array of RouteStep code occurences
            var routeStepsAssociative = RouteStep.GetCodeOccurenceCount(routeSteps);

            // Normal "for loop" only because of using index comparision
            for (var i = 0; i < punches.Count; i++)
            {
                // If Code matches in both lists set to CORRECT
                if (i < routeSteps.Count && punches[i].Code == routeSteps[i].Code)
                {
                    punches[i].Correctness = Correctness.CORRECT;
                    if(!allowRepeats)
                        routeStepsAssociative[punches[i].Code]--;
                }
                else
                {
                    // On other cases than mentioned below Punch is invalid
                    punches[i].Correctness = Correctness.INVALID;
                }
            }

            // Check invalid punches if they're misplaced
            foreach (var punch in punches)
            {
                if (punch.Correctness == Correctness.INVALID)
                {
                    long value;
                    // If element exists on route
                    if (routeStepsAssociative.TryGetValue(punch.Code, out value) == true)
                    {
                        // If element exists and was collected proper number of times
                        if (value > 0)
                        {
                            if(!allowRepeats)
                                routeStepsAssociative[punch.Code]--;
                            punch.Correctness = Correctness.PRESENT;
                        }
                    }
                }
            }
        }

        // Counts number of punches of wanted type
        public static uint GetNoOfCorrectnessPunches(IList<Punch> punches, Correctness correctness)
        {
            return GetNoOfCorrectnessPunches(punches, new List<Correctness> { correctness });
        }

        // Counts number of punches of wanted types
        public static uint GetNoOfCorrectnessPunches(IList<Punch> punches, IList<Correctness> correctness)
        {
            uint count = 0;
            foreach (var p in punches)
            {
                foreach (var c in correctness)
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

        // Counts number of punches which should be collected according to route
        public static uint GetNoOfNotPresentPunches(IList<Punch> punches, IList<RouteStep> routeSteps)
        {
            uint count = 0;
            if (punches == null)
                punches = new List<Punch>();

            var occurences = RouteStep.GetCodeOccurenceCount(routeSteps);

            foreach(var punch in punches)
            {
                if(occurences.ContainsKey(punch.Code))
                {
                    occurences[punch.Code]--;
                }
            }

            foreach(var code in occurences)
            {
                if (code.Value > 0)
                    count++;
            }

            return count;
        }

        public static void CalculateDeltaStart(ref List<Punch> punches, long StartTime)
        {
            if (punches == null)
                return;//throw new System.ArgumentNullException(nameof(punches), "List refers to null");

            foreach (var punch in punches)
                punch.CalculateDeltaStart(StartTime);
        }

        public static void CalculateDeltaPrevious(ref List<Punch> punches, long StartTime)
        {
            if (punches == null || punches.Count <= 0)
                return;// throw new System.ArgumentNullException(nameof(punches), "List refers to null");
            punches[0].CalculateDeltaPrevious(StartTime);
            for (int i = 1; i < punches.Count; i++)
            {
                punches[i].CalculateDeltaPrevious(punches[i - 1].Timestamp);
            }
        }
        #endregion
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
        #region IComparable<Punch> implementation
        public int CompareTo(Punch other)
        {
            return Timestamp.CompareTo(other.Timestamp);
        }
        #endregion
    }
}
