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
