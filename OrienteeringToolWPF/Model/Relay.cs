﻿using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using System.Collections.Generic;
using System;
using System.ComponentModel;
/// <summary>
/// Models relay info
/// </summary>
namespace OrienteeringToolWPF.Model
{
    public class Relay : BaseModel, ISelectable, IExpandable, INotifyPropertyChanged
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
        #region ISelectable implementation
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        #endregion
        #region IExpandable implementation
        private bool _isExpanded;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }
        #endregion
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
