using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using System.Collections.Generic;
using System;
using System.ComponentModel;
/// <summary>
/// Models relay info
/// </summary>
namespace OrienteeringToolWPF.Model
{
    public class Relay : INotifyPropertyChanged, IComparable<Relay>
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        // For join queries
        public IList<Competitor> Competitors { get; set; }

        public Relay() : base()
        {
            PropertyChanged += Relay_PropertyChanged;
        }

        private void Relay_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(Competitors))
            {
                // Invalidate calculated values
                _OverallRunningTime = null;
                _OverallWrongCollections = null;
            }
        }

        #region Lazy calculated, readonly properties
        private long? _OverallRunningTime;
        public long? OverallRunningTime
        {
            get
            {
                if (_OverallRunningTime == null && Competitors != null)
                {
                    _OverallRunningTime = 0;
                    foreach(var c in Competitors)
                    {
                        _OverallRunningTime += c.Result?.RunningTime;
                    }
                }
                return _OverallRunningTime;
            }
        }

        private uint? _OverallWrongCollections;
        public uint? OverallWrongCollections
        {
            get
            {
                if (_OverallWrongCollections == null && Competitors != null)
                {
                    _OverallWrongCollections = 0;
                    foreach (var c in Competitors)
                    {
                        _OverallWrongCollections += c.WrongCollections;
                    }
                }
                return _OverallWrongCollections;
            }
        }
        #endregion
        #region Object overrides
        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if(obj is Relay)
            {
                Relay r = (Relay)obj;
                if(Id == r.Id)
                {
                    if (Name.Equals(r.Name))
                        return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Id.GetHashCode();
        }
        #endregion
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region IComparable<Relay> implementation
        public int CompareTo(Relay other)
        {
            var retVal = 0;
            var left = this;
            var right = other;
            // When right relay has more mistakes
            if (left.OverallWrongCollections < right.OverallWrongCollections)
            {
                retVal = -1;
            }
            // When left relay has more mistakes
            else if (left.OverallWrongCollections > right.OverallWrongCollections)
            {
                retVal = 1;
            }
            // When relays have the same amount of mistakes (this means none too)
            else
            {
                if (left.OverallRunningTime < right.OverallRunningTime)
                    retVal = -1;
                else if (left.OverallRunningTime > right.OverallRunningTime)
                    retVal = 1;
                else
                    retVal = 0;
            }

            return retVal;
        }
        #endregion
    }
}
