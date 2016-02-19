//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using GecoSI.Net.Dataframe;
using GecoSI.Net.Internal;

namespace GecoSI.Net
{
    public class SiDriverState
    {
        public static readonly SiDriverState STARTUP = new StartupDriverState();

        public static readonly  SiDriverState STARTUP_CHECK = new StartupCheckDriverState();


        public static readonly SiDriverState STARTUP_TIMEOUT = new StartupTimeoutDriverState();


        public static readonly SiDriverState GET_CONFIG = new GetConfigDriverState();

        public static readonly  SiDriverState CONFIG_CHECK = new ConfigCheckDriverState();

        public static readonly SiDriverState EXTENDED_PROTOCOL_ERROR = new ExtendedProtocolErrorDriverState();

        public static readonly SiDriverState HANDSHAKE_MODE_ERROR = new HandshakeModeErrorDriverState();

        public static readonly SiDriverState GET_SI6_CARDBLOCKS = new GetSi6CardblocksDriverState();

        public static readonly SiDriverState SI6_CARDBLOCKS_SETTING = new Si6CardblocksSettingDriverState();

        public static readonly SiDriverState STARTUP_COMPLETE = new StartupCompleteDriverState();

        public static readonly SiDriverState DISPATCH_READY = new DispatchReadyDriverState();

        public static readonly SiDriverState RETRIEVE_SICARD_5_DATA = new RetrieveSicard5DataDriverState();

        public static readonly SiDriverState RETRIEVE_SICARD_6_DATA = new RetrieveSicard6DataDriverState();

        public static readonly SiDriverState RETRIEVE_SICARD_8_9_DATA = new RetrieveSicard89DataDriverState();

        public static readonly SiDriverState RETRIEVE_SICARD_10_PLUS_DATA = new RetrieveSicard10PlusDataDriverState();

        public static readonly SiDriverState ACK_READ = new AckReadDriverState();

        public static readonly SiDriverState WAIT_SICARD_REMOVAL = new WaitSicardRemovalDriverState();


        private const int EXTENDED_PROTOCOL_MASK = 1;

        private const int HANDSHAKE_MODE_MASK = 4;

        private const int CONFIG_CHECK_MASK = EXTENDED_PROTOCOL_MASK | HANDSHAKE_MODE_MASK;

// ReSharper disable once InconsistentNaming
        private static bool si6_192PunchesMode;

        public static bool sicard6_192PunchesMode()
        {
            return si6_192PunchesMode;
        }

        public static void setSicard6_192PunchesMode(bool flag)
        {
            si6_192PunchesMode = flag;
        }

        public virtual SiDriverState Send(ICommWriter writer, SiHandler siHandler)
        {
            WrongCall();
            return this;
        }

        public virtual SiDriverState Receive(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
        {
            WrongCall();
            return this;
        }

        public virtual SiDriverState Retrieve(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
        {
            WrongCall();
            return this;
        }

        public virtual ISiDataFrame CreateDataFrame(SiMessage[] dataMessages)
        {
            WrongCall();
            return null;
        }

        private void WrongCall()
        {
            throw new Exception(String.Format("This method should not be called on {0}", GetType().Name));
        }

        public virtual bool  IsError()
        {
            return false;
        }

        public virtual String Status()
        {
            return GetType().Name;
        }

        protected void CheckAnswer(SiMessage message, byte command)
        {
            if (! message.Check(command))
            {
                throw new InvalidMessage(message);
            }
        }

        protected SiMessage PollAnswer(SiMessageQueue queue, byte command)
        {
            SiMessage message = queue.TimeoutPoll();
            CheckAnswer(message, command);
            return message;
        }

        protected int ExtractNumberOfDataBlocks(SiMessage firstBlock, int nbPunchesIndex)
        {
            int nbPunches = firstBlock.Sequence(nbPunchesIndex) & 0xFF;
            int nbPunchesPerBlock = 32;
            int nbPunchDataBlocks = (nbPunches/nbPunchesPerBlock) + Math.Min(1, nbPunches%nbPunchesPerBlock);
            GecoSiLogger.Info(String.Format("Nb punches/Data blocks: {0}/{1}", nbPunches, nbPunchDataBlocks));
            return nbPunchDataBlocks + 1;
        }

        protected SiDriverState RetrieveDataMessages(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler,
            SiMessage[] readoutCommands, int nbPunchesIndex, String timeoutMessage)
        {
            try
            {
                GecoSiLogger.StateChanged(GetType().Name);
                SiMessage readoutCommand = readoutCommands[0];
                writer.Write(readoutCommand);
                SiMessage firstDataBlock = PollAnswer(queue, readoutCommand.CommandByte());
                int nbDataBlocks = (nbPunchesIndex == -1)
                    ? readoutCommands.Length
                    : ExtractNumberOfDataBlocks(firstDataBlock, nbPunchesIndex);
                var dataMessages = new SiMessage[nbDataBlocks];
                dataMessages[0] = firstDataBlock;
                for (int i = 1; i < nbDataBlocks; i++)
                {
                    readoutCommand = readoutCommands[i];
                    writer.Write(readoutCommand);
                    dataMessages[i] = PollAnswer(queue, readoutCommand.CommandByte());
                }
                siHandler.Notify(CreateDataFrame(dataMessages));
                return ACK_READ.Send(writer, siHandler);
            }
            catch (TimeoutException)
            {
                return ErrorFallback(siHandler, timeoutMessage);
            }
            catch (InvalidMessage e)
            {
                return ErrorFallback(siHandler, "Invalid message: " + e.ReceivedMessage());
            }
        }

        protected SiDriverState ErrorFallback(SiHandler siHandler, String errorMessage)
        {
            GecoSiLogger.Error(errorMessage);
            siHandler.Notify(CommStatus.ProcessingError);
            return DISPATCH_READY;
        }

        protected class AckReadDriverState : SiDriverState
        {
            public override SiDriverState Send(ICommWriter writer, SiHandler siHandler)
            {
                writer.Write(SiMessage.ACK_SEQUENCE);
                return WAIT_SICARD_REMOVAL;
            }
        }

        protected class ConfigCheckDriverState : SiDriverState
        {
            public override SiDriverState Receive(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
            {
                SiMessage message = PollAnswer(queue, SiMessage.GET_SYSTEM_VALUE);
                byte cpcByte = message.Sequence(6);
                if ((cpcByte & CONFIG_CHECK_MASK) == CONFIG_CHECK_MASK)
                {
                    return GET_SI6_CARDBLOCKS.Send(writer, siHandler);
                }
                if ((cpcByte & EXTENDED_PROTOCOL_MASK) == 0)
                {
                    return EXTENDED_PROTOCOL_ERROR;
                }
                return HANDSHAKE_MODE_ERROR;
            }
        }

        protected class DispatchReadyDriverState : SiDriverState
        {
            public override SiDriverState Receive(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
            {
                siHandler.Notify(CommStatus.Ready);
                SiMessage message = queue.Take();
                siHandler.Notify(CommStatus.Processing);

                if (!message.Valid())
                {
                    return DISPATCH_READY;
                }

                if (message.CommandByte() == SiMessage.SI_CARD_5_DETECTED ||
                    message.CommandByte() == SiMessage.SI_CARD_6_PLUS_DETECTED ||
                    message.CommandByte() == SiMessage.SI_CARD_8_PLUS_DETECTED)
                {
                    var bytes = new ByteFrame(message.Data());
                    if (!siHandler.OnEcardDown(bytes.Block3At(5).ToString()))
                    {
                        return ACK_READ.Send(writer, siHandler);
                    }
                }

                switch (message.CommandByte())
                {
                    case SiMessage.SI_CARD_5_DETECTED:
                        return RETRIEVE_SICARD_5_DATA.Retrieve(queue, writer, siHandler);
                    case SiMessage.SI_CARD_6_PLUS_DETECTED:
                        return RETRIEVE_SICARD_6_DATA.Retrieve(queue, writer, siHandler);
                    case SiMessage.SI_CARD_8_PLUS_DETECTED:
                        return DispatchSicard8Plus(message, queue, writer, siHandler);
                    case SiMessage.BEEP:
                        break;
                    case SiMessage.SI_CARD_REMOVED:
                        GecoSiLogger.Debug("Late removal " + message);
                        break;
                    default:
                        GecoSiLogger.Debug("Unexpected message " + message);
                        break;
                }
                return DISPATCH_READY;
            }

            private static SiDriverState DispatchSicard8Plus(SiMessage message, SiMessageQueue queue, ICommWriter writer,
                SiHandler siHandler)
            {
                if (message.Sequence(SiMessage.SI3_NUMBER_INDEX) == SiMessage.SI_CARD_10_PLUS_SERIES)
                {
                    return RETRIEVE_SICARD_10_PLUS_DATA.Retrieve(queue, writer, siHandler);
                }
                return RETRIEVE_SICARD_8_9_DATA.Retrieve(queue, writer, siHandler);
            }
        }

        protected class ExtendedProtocolErrorDriverState : SiDriverState
        {
            public override bool IsError()
            {
                return true;
            }

            public override String Status()
            {
                return "Master station should be configured with extended protocol";
            }
        }

        protected class GetConfigDriverState : SiDriverState
        {
            public override SiDriverState Send(ICommWriter writer, SiHandler siHandler)
            {
                writer.Write(SiMessage.GET_PROTOCOL_CONFIGURATION);
                return CONFIG_CHECK;
            }
        }

        protected class GetSi6CardblocksDriverState : SiDriverState
        {
            public override SiDriverState Send(ICommWriter writer, SiHandler siHandler)
            {
                writer.Write(SiMessage.GET_CARDBLOCKS_CONFIGURATION);
                return SI6_CARDBLOCKS_SETTING;
            }
        }

        protected class HandshakeModeErrorDriverState : SiDriverState
        {
            public override bool IsError()
            {
                return true;
            }

            public override String Status()
            {
                return "Master station should be configured in handshake mode (no autosend)";
            }
        }

        protected class RetrieveSicard10PlusDataDriverState : SiDriverState
        {
            private readonly SiMessage[] readoutCommands =
            {
                SiMessage.READ_SICARD_10_PLUS_B0,
                SiMessage.READ_SICARD_10_PLUS_B4,
                SiMessage.READ_SICARD_10_PLUS_B5,
                SiMessage.READ_SICARD_10_PLUS_B6,
                SiMessage.READ_SICARD_10_PLUS_B7
            };

            public override SiDriverState Retrieve(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
            {
                int nbPunchesIndex = Si8PlusDataFrame.NB_PUNCHES_INDEX + 6;
                return RetrieveDataMessages(queue, writer, siHandler, readoutCommands, nbPunchesIndex,
                    "Timeout on retrieving SiCard 10/11/SIAC data");
            }


            public override ISiDataFrame CreateDataFrame(SiMessage[] dataMessages)
            {
                return new Si8PlusDataFrame(dataMessages);
            }
        }

        protected class RetrieveSicard5DataDriverState : SiDriverState
        {
            public override SiDriverState Retrieve(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
            {
                return RetrieveDataMessages(queue, writer, siHandler, new[] {SiMessage.READ_SICARD_5}, -1,
                    "Timeout on retrieving SiCard 5 data");
            }

            public override ISiDataFrame CreateDataFrame(SiMessage[] dataMessages)
            {
                return new Si5DataFrame(dataMessages[0]);
            }
        }

        protected class RetrieveSicard6DataDriverState : SiDriverState
        {
            private readonly SiMessage[] readoutCommands =
            {
                SiMessage.READ_SICARD_6_B0,
                SiMessage.READ_SICARD_6_B6,
                SiMessage.READ_SICARD_6_B7,
                SiMessage.READ_SICARD_6_PLUS_B2,
                SiMessage.READ_SICARD_6_PLUS_B3,
                SiMessage.READ_SICARD_6_PLUS_B4,
                SiMessage.READ_SICARD_6_PLUS_B5
            };

            public override SiDriverState Retrieve(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
            {
                int nbPunchesIndex = Si6DataFrame.NB_PUNCHES_INDEX + 6;
                return RetrieveDataMessages(queue, writer, siHandler, readoutCommands, nbPunchesIndex,
                    "Timeout on retrieving SiCard 6 data");
            }


            public override ISiDataFrame CreateDataFrame(SiMessage[] dataMessages)
            {
                return new Si6DataFrame(dataMessages);
            }
        }

        protected class RetrieveSicard89DataDriverState : SiDriverState
        {
            public override SiDriverState Retrieve(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
            {
                return RetrieveDataMessages(queue, writer, siHandler,
                    new[] {SiMessage.READ_SICARD_8_PLUS_B0, SiMessage.READ_SICARD_8_PLUS_B1},
                    -1, "Timeout on retrieving SiCard 8/9 data");
            }


            public override ISiDataFrame CreateDataFrame(SiMessage[] dataMessages)
            {
                return new Si8PlusDataFrame(dataMessages);
            }
        }

        protected class Si6CardblocksSettingDriverState : SiDriverState
        {
            public override SiDriverState Receive(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
            {
                SiMessage message = PollAnswer(queue, SiMessage.GET_SYSTEM_VALUE);
                si6_192PunchesMode = (message.Sequence(6) & 0xFF) == 0xFF;
                GecoSiLogger.Info("SiCard6 192 Punches Mode " + (si6_192PunchesMode ? "Enabled" : "Disabled"));
                return STARTUP_COMPLETE.Send(writer, siHandler);
            }
        }

        protected class StartupCompleteDriverState : SiDriverState
        {
            public override SiDriverState Send(ICommWriter writer, SiHandler siHandler)
            {
                writer.Write(SiMessage.BEEP_TWICE);
                siHandler.Notify(CommStatus.On);
                return DISPATCH_READY;
            }
        }

        protected class StartupCheckDriverState : SiDriverState
        {
            public override SiDriverState Receive
                (SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
            {
                PollAnswer(queue, SiMessage.SET_MASTER_MODE);
                return GET_CONFIG.Send(writer, siHandler);
            }
        }

        protected class StartupDriverState : SiDriverState
        {
            public override SiDriverState Send(ICommWriter writer, SiHandler siHandler)
            {
                writer.Write(SiMessage.STARTUP_SEQUENCE);
                return STARTUP_CHECK;
            }
        }

        protected class StartupTimeoutDriverState : SiDriverState
        {
            public override bool IsError()
            {
                return true;
            }

            public override String Status()
            {
                return "Master station did not answer to startup sequence (high/low baud)";
            }
        }

        public class WaitSicardRemovalDriverState : SiDriverState
        {
            public override SiDriverState Receive(SiMessageQueue queue, ICommWriter writer, SiHandler siHandler)
            {
                try
                {
                    PollAnswer(queue, SiMessage.SI_CARD_REMOVED);
                    return DISPATCH_READY;
                }
                catch (TimeoutException)
                {
                    GecoSiLogger.Info("Timeout on SiCard removal");
                    return DISPATCH_READY;
                }
                catch (InvalidMessage e)
                {
                    return ErrorFallback(siHandler, "Invalid message: " + e.ReceivedMessage());
                }
            }
        }
    }
}