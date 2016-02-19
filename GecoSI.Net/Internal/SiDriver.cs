//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using System.Threading;

namespace GecoSI.Net
{
    public class SiDriver
    {
        private readonly SiMessageQueue messageQueue;
        private readonly SiHandler siHandler;
        private readonly ISiPort siPort;
        private readonly ICommWriter writer;
        private Thread thread;

        public SiDriver(ISiPort siPort, SiHandler siHandler)
        {
            this.siPort = siPort;
            messageQueue = siPort.CreateMessageQueue();
            writer = siPort.CreateWriter();
            this.siHandler = siHandler;
        }

        public SiDriver Start()
        {
            thread = new Thread(Run);
            thread.Start();
            return this;
        }

        public void Interrupt()
        {
            thread.Interrupt();
        }

        public void Run()
        {
            try
            {
                SiDriverState currentState = StartupBootstrap();
                while (IsAlive(currentState))
                {
                    GecoSiLogger.StateChanged(currentState.GetType().Name);
                    currentState = currentState.Receive(messageQueue, writer, siHandler);
                }
                if (currentState.IsError())
                {
                    siHandler.NotifyError(CommStatus.FatalError, currentState.Status());
                }
            }
            catch (ThreadInterruptedException)
            {
                // normal way out
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
                GecoSiLogger.Error(" #run# " + e);
            }
            finally
            {
                Stop();
            }
        }

        private bool IsAlive(SiDriverState currentState)
        {
            return thread.IsAlive && !currentState.IsError();
        }

        private SiDriverState StartupBootstrap()
        {
            try
            {
                siHandler.Notify(CommStatus.Starting);
                siPort.SetupHighSpeed();
                return Startup();
            }
            catch (TimeoutException)
            {
                try
                {
                    siPort.SetupLowSpeed();
                    return Startup();
                }
                catch (TimeoutException)
                {
                    return SiDriverState.STARTUP_TIMEOUT;
                }
            }
        }

        private SiDriverState Startup()
        {
            SiDriverState currentState = SiDriverState.STARTUP.Send(writer, siHandler)
                .Receive(messageQueue, writer, siHandler);
            return currentState;
        }

        private void Stop()
        {
            siPort.Close();
            siHandler.Notify(CommStatus.Off);
            GecoSiLogger.Close();
        }
    }
}