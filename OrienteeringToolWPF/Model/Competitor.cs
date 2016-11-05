using OrienteeringToolWPF.Enumerations;
using OrienteeringToolWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public class Competitor : BaseModel, ISelectable, INotifyPropertyChanged
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public long Chip { get; set; }
        public long RelayId { get; set; }
        public long Category { get; set; }
        public GenderEnum Gender { get; set; }
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
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
