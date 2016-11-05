using OrienteeringToolWPF.Enumerations;
using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// Models competitor
/// </summary>
namespace OrienteeringToolWPF.Model
{
    public class Competitor : BaseModel, INotifyPropertyChanged, IComparable<Competitor>
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public long Chip { get; set; }
        public long RelayId { get; set; }
        public long Category { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }

        // For join queries
        public IList<Punch> Punches { get; set; }
        public IList<Result> Results { private get; set; }

        public Result Result
        {
            get { return Results?[0]; }
            set
            {
                if (Results == null)
                    Results = new List<Result>();

                if (Results.Count > 0)
                    Results[0] = value;
                else
                    Results.Add(value);
            }
        }

        #region In time calculated, readonly properties
        public uint GoodCollections
        {
            get
            {
                uint retVal = 0;
                retVal += CorrectPunches ?? default(uint);
                retVal += PresentPunches ?? default(uint);
                return retVal;
            }
        }
        public uint WrongCollections {
            get
            {
                uint retVal = 0;
                retVal += InvalidPunches ?? default(uint);
                retVal += NotPresentPunches ?? default(uint);
                return retVal;
            }
        }
        #endregion
        #region Lazy calculated, readonly properties
        private uint? _NotCheckedPunches;
        public uint? NotCheckedPunches
        {
            get
            {
                if (_NotCheckedPunches == null && Punches != null)
                {
                    _NotCheckedPunches = Punch.GetNoOfCorrectnessPunches(
                        Punches,
                        Correctness.NOT_CHECKED);
                }
                return _NotCheckedPunches;
            }
        }
        private uint? _CorrectPunches;
        public uint? CorrectPunches
        {
            get
            {
                if (_CorrectPunches == null && Punches != null)
                {
                    _CorrectPunches = Punch.GetNoOfCorrectnessPunches(
                        Punches,
                        Correctness.CORRECT);
                }
                return _CorrectPunches;
            }
        }
        private uint? _PresentPunches;
        public uint? PresentPunches
        {
            get
            {
                if (_PresentPunches == null && Punches != null)
                {
                    _PresentPunches = Punch.GetNoOfCorrectnessPunches(
                        Punches,
                        Correctness.PRESENT);
                }
                return _PresentPunches;
            }
        }
        private uint? _InvalidPunches;
        public uint? InvalidPunches
        {
            get
            {
                if (_InvalidPunches == null && Punches != null)
                {
                    _InvalidPunches = Punch.GetNoOfCorrectnessPunches(
                        Punches,
                        Correctness.INVALID);
                }
                return _InvalidPunches;
            }
        }
        private uint? _NotPresentPunches;
        public uint? NotPresentPunches
        {
            get
            {
                if (_NotPresentPunches == null)
                {
                    var db = MainWindow.GetDatabase();
                    dynamic routesAlias, routeStepsAlias;
                    var RouteSteps = (List<RouteStep>)db.Categories
                        .FindAllById(Category)
                        .Join(db.Routes, out routesAlias)
                        .On(db.Categories.Id == db.Routes.Category)
                        .Join(db.RouteSteps, out routeStepsAlias)
                        .On(db.Routes.Id == db.RouteSteps.RouteId)
                        .Select(db.RouteSteps.AllColumns());

                    _NotPresentPunches = Punch.GetNoOfNotPresentPunches(
                        Punches,
                        RouteSteps);
                }
                return _NotPresentPunches;
            }
        }
        #endregion

        public Competitor() : base()
        {
            PropertyChanged += Competitor_PropertyChanged;
        }

        private void Competitor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Punches))
            {
                // Invalidate calculated values
                _NotCheckedPunches = null;
                _CorrectPunches = null;
                _PresentPunches = null;
                _InvalidPunches = null;
            }
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region IComparable<Competitor>
        public int CompareTo(Competitor other)
        {
            int retVal = 0;
            var left = this;
            var right = other;

            var leftPunches = left.WrongCollections;
            var rightPunches = right.WrongCollections;

            // When right competitor made more mistakes
            if (leftPunches < rightPunches)
            {
                retVal = -1;
            }
            // When left competitor made more mistakes
            else if (leftPunches > rightPunches)
            {
                retVal = 1;
            }
            // When competitors made the same amount of mistakes (this means none too)
            else
            {
                var leftTime = left.Result.RunningTime;
                var rightTime = right.Result.RunningTime;
                // When left one ran longer
                if (leftTime > rightTime)
                    retVal = 1;
                // When right one ran longer
                else if (leftTime < rightTime)
                    retVal = -1;
                // When both ran the same time
                else
                    retVal = 0;
            }

            return retVal;
        }
        #endregion
    }
}
