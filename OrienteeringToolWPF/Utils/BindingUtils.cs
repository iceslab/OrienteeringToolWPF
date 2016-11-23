using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.Utils
{
    /// <summary>
    /// Class used only for binding to static resource
    /// </summary>
    class BindingUtils : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public bool IsDatabaseAccessible
        {
            get
            {
                return DatabaseUtils.IsDatabaseAccessible;
            }
        }

        public BindingUtils()
        {
            DatabaseUtils.GlobalPropertyChanged += DatabaseUtils_GlobalPropertyChanged;
        }

        private void DatabaseUtils_GlobalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DatabaseUtils.IsDatabaseAccessible))
                OnPropertyChanged(nameof(IsDatabaseAccessible));
        }
    }
}
