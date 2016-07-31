using GecoSI.Net;
using GecoSI.Net.Dataframe;
using OrienteeringToolWPF.Utils;
using System;
using System.ComponentModel;
using System.Windows;

namespace OrienteeringToolWPF
{
    public class SiListener : ISiListener, INotifyPropertyChanged
    {

        public void HandleEcard(ISiDataFrame dataFrame)
        {
            //dataFrame.PrintString();
            AbstractDataFrame adf = (AbstractDataFrame)dataFrame;
            DataFrame = adf;
            string result = "";

            result += String.Format("{0}: {1} \n", adf.SiSeries, adf.SiNumber);
            result += String.Format("(Start: {0} ", adf.FormatTime(adf.StartTime));
            result += String.Format(" - Finish: {0}", adf.FormatTime(adf.FinishTime));
            result += String.Format(" - Check: {0})\n", adf.FormatTime(adf.CheckTime));
            result += String.Format("Punches: {0}\n", dataFrame.NbPunches);
            for (int i = 0; i < dataFrame.NbPunches; i++)
            {
                result += String.Format("{0}: {1} {2} - \n", i, adf.Punches[i].Code, adf.FormatTime(adf.Punches[i].Timestamp));
            }

            result += String.Format("\n" + DateTime.Now.ToString("hh:mm:ss.fff") + "\n");
            DataString = result;
        }

        public void Notify(CommStatus status)
        {
            Status = "Status: " + status;
        }

        public void Notify(CommStatus errorStatus, String errorMessage)
        {
            Status = "Status: Error: " + errorStatus + " " + errorMessage;
            MessageUtils.ShowSiHandlerError(errorStatus, errorMessage);
        }

        public bool OnEcardDown(string siNumber)
        {
            return true;
        }

        private string _status;
        private string _dataString;
        private AbstractDataFrame _dataFrame;

        public string Status
        {
            get { return _status; }

            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public string DataString
        {
            get { return _dataString; }

            set
            {
                _dataString = value;
                OnPropertyChanged("DataString");
            }
        }

        public AbstractDataFrame DataFrame
        {
            get { return _dataFrame; }

            set
            {
                _dataFrame = value;
                OnPropertyChanged("DataFrame");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
