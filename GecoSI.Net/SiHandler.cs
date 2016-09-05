//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
#define SI_NOT_AVAILABLE
using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
using GecoSI.Net.Adapter.LogFie;
using GecoSI.Net.Adapter.SerialPort;
using GecoSI.Net.Dataframe;
using System.ComponentModel;

namespace GecoSI.Net
{
    public class SiHandler : INotifyPropertyChanged
    {
        private readonly BlockingCollection<ISiDataFrame> dataQueue;

        public readonly ISiListener siListener;

        private SiDriver driver;

        private Thread thread;
        private long zerohour;

        public SiHandler()
        {
            dataQueue = new BlockingCollection<ISiDataFrame>();
        }

        public SiHandler(ISiListener siListener)
        {
            dataQueue = new BlockingCollection<ISiDataFrame>();
            this.siListener = siListener;
        }

        public void SetZeroHour(long zerohour)
        {
            this.zerohour = zerohour;
        }

        public void Connect(String portname)
        {
            try
            {
                GecoSiLogger.Open("######");
                GecoSiLogger.LogTime("Start " + portname);
                Start();
                var port = new SerialPort(portname, 2000);
                port.Open();
                driver = new SiDriver(new SerialComPort(port), this).Start();
                IsConnected = true;
            }
            catch (Exception)
            {
                IsConnected = false;
                siListener.Notify(CommStatus.FatalError, "Port in use");
            }
        }

        public void ReadLog(String logFilename)
        {
            try
            {
                GecoSiLogger.OpenOutStreamLogger();
                Start();
                driver = new SiDriver(new LogFilePort(logFilename), this).Start();
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
        }

        public void Start()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        public Thread Stop()
        {
            if (driver != null)
            {
                driver.Interrupt();
            }
            if (thread != null)
            {
                thread.Interrupt();
            }
            IsConnected = false;
            return thread;
        }

        public bool IsAlive()
        {
            return thread != null && thread.IsAlive;
        }

        public virtual void Notify(ISiDataFrame data)
        {
            data.StartingAt(zerohour);
            dataQueue.Add(data); // TODO check true
        }

        public virtual void Notify(CommStatus status)
        {
            GecoSiLogger.Log("!", status.GetType().Name);
            siListener.Notify(status);
        }

        public virtual void NotifyError(CommStatus errorStatus, String errorMessage)
        {
            GecoSiLogger.Error(errorMessage);
            siListener.Notify(errorStatus, errorMessage);
            Stop();
        }

        public virtual bool OnEcardDown(string siNumber)
        {
            return siListener.OnEcardDown(siNumber);
        }

        public void Run()
        {
            try
            {
                ISiDataFrame dataFrame;
                while ((dataFrame = dataQueue.Take()) != null)
                {
                    siListener.HandleEcard(dataFrame);
                }
            }
            catch (ThreadInterruptedException)
            {
                //dataQueue.Dispose();
            }
        }

        #region IsConnected and NotIsConnected properties
        private bool _connected;
        public bool IsConnected
        {
            get
            {
#if SI_NOT_AVAILABLE
                return true;
#else
                return _connected;
#endif
            }

            set
            {
                _connected = value;
                OnPropertyChanged("IsConnected");
                OnPropertyChanged("NotIsConnected");
            }
        }
        public bool NotIsConnected
        {
            get
            {
#if SI_NOT_AVAILABLE
                return false;
#else
             return !_connected; 
#endif
            }

            set
            {
                _connected = !value;
                OnPropertyChanged("IsConnected");
                OnPropertyChanged("NotIsConnected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}