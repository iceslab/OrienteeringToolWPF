//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;
using System.Text;

namespace GecoSI.Net.Internal
{
    public class SiMessage
    {
        /*
         * Command instructions
         */
        public const byte GET_SYSTEM_VALUE = 0x83;
        public const byte SET_MASTER_MODE = 0xF0;
        public const byte DIRECT_MODE = 0x4d;
        public const byte BEEP = 0xF9;

        /*
         * Card detected/removed
         */
        public const byte SI_CARD_5_DETECTED = 0xE5;
        public const byte SI_CARD_6_PLUS_DETECTED = 0xE6;
        public const byte SI_CARD_8_PLUS_DETECTED = 0xE8;
        public const byte SI_CARD_REMOVED = 0xE7;
        public const byte WAKEUP = 0xFF;
        public const byte STX = 0x02;
        public const byte ETX = 0x03;
        public const byte ACK = 0x06;
        public const byte NAK = 0x15;

        /*
         * Card Readout instructions
         */
        public const byte GET_SI_CARD_5 = 0xB1;
        public const byte GET_SI_CARD_6_BN = 0xE1;
        public const byte GET_SI_CARD_8_PLUS_BN = 0xEF;

        /*
         * SiCard special data
         */
        public const int SI3_NUMBER_INDEX = 5;
        public const byte SI_CARD_10_PLUS_SERIES = 0x0F;

        /*
         * Command messages
         */

        public static readonly SiMessage STARTUP_SEQUENCE = new SiMessage(new byte[]
        {
            WAKEUP, STX, STX, SET_MASTER_MODE, 0x01, DIRECT_MODE, 0x6D, 0x0A, ETX
        });

        public static readonly SiMessage GET_PROTOCOL_CONFIGURATION = new SiMessage(new byte[]
        {
            STX, GET_SYSTEM_VALUE, 0x02, 0x74, 0x01, 0x04, 0x14, ETX
        });

        public static readonly SiMessage GET_CARDBLOCKS_CONFIGURATION = new SiMessage(new byte[]
        {
            STX, GET_SYSTEM_VALUE, 0x02, 0x33, 0x01, 0x16, 0x11, ETX
        });

        public static readonly SiMessage ACK_SEQUENCE = new SiMessage(new[]
        {
            ACK
        });

        public static readonly SiMessage READ_SICARD_5 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_5, 0x00, GET_SI_CARD_5, 0x00, ETX
        });

        public static readonly SiMessage READ_SICARD_6_B0 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_6_BN, 0x01, 0x00, 0x46, 0x0A, ETX
        });

        public static readonly SiMessage READ_SICARD_6_PLUS_B2 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_6_BN, 0x01, 0x02, 0x44, 0x0A, ETX
        });

        public static readonly SiMessage READ_SICARD_6_PLUS_B3 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_6_BN, 0x01, 0x03, 0x45, 0x0A, ETX
        });

        public static readonly SiMessage READ_SICARD_6_PLUS_B4 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_6_BN, 0x01, 0x04, 0x42, 0x0A, ETX
        });

        public static readonly SiMessage READ_SICARD_6_PLUS_B5 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_6_BN, 0x01, 0x05, 0x43, 0x0A, ETX
        });

        public static readonly SiMessage READ_SICARD_6_B6 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_6_BN, 0x01, 0x06, 0x40, 0x0A, ETX
        });

        public static readonly SiMessage READ_SICARD_6_B7 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_6_BN, 0x01, 0x07, 0x41, 0x0A, ETX
        });

        public static readonly SiMessage READ_SICARD_6_B8 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_6_BN, 0x01, 0x08, 0x4E, 0x0A, ETX
        });

        public static readonly SiMessage READ_SICARD_8_PLUS_B0 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_8_PLUS_BN, 0x01, 0x00, 0xE2, 0x09, ETX
        });

        public static readonly SiMessage READ_SICARD_8_PLUS_B1 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_8_PLUS_BN, 0x01, 0x01, 0xE3, 0x09, ETX
        });

        public static readonly SiMessage READ_SICARD_10_PLUS_B0 = READ_SICARD_8_PLUS_B0;

        public static readonly SiMessage READ_SICARD_10_PLUS_B4 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_8_PLUS_BN, 0x01, 0x04, 0xE6, 0x09, ETX
        });

        public static readonly SiMessage READ_SICARD_10_PLUS_B5 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_8_PLUS_BN, 0x01, 0x05, 0xE7, 0x09, ETX
        });

        public static readonly SiMessage READ_SICARD_10_PLUS_B6 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_8_PLUS_BN, 0x01, 0x06, 0xE4, 0x09, ETX
        });

        public static readonly SiMessage READ_SICARD_10_PLUS_B7 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_8_PLUS_BN, 0x01, 0x07, 0xE5, 0x09, ETX
        });

        public static readonly SiMessage READ_SICARD_10_PLUS_B8 = new SiMessage(new byte[]
        {
            STX, GET_SI_CARD_8_PLUS_BN, 0x01, 0x08, 0xEA, 0x09, ETX
        });

        public static readonly SiMessage BEEP_TWICE = new SiMessage(new byte[]
        {
            STX, BEEP, 0x01, 0x02, 0x14, 0x0A, ETX
        });

        private readonly byte[] sequence;

        public SiMessage(byte[] sequence)
        {
            this.sequence = sequence;
        }

        public byte[] Sequence()
        {
            return sequence;
        }

        public byte Sequence(int i)
        {
            return sequence[i];
        }

        public byte[] Data()
        {
            int cmdLength = sequence.Length - 4;
            var command = new byte[cmdLength];
            Array.Copy(sequence, 1, command, 0, cmdLength);
            return command;
        }

        public byte CommandByte()
        {
            return sequence[1];
        }

        public byte StartByte()
        {
            return sequence[0];
        }

        public byte EndByte()
        {
            return sequence[sequence.Length - 1];
        }

        public int ExtractCrc()
        {
            int i = sequence.Length;
            return (sequence[i - 3] << 8 & 0xFFFF) | (sequence[i - 2] & 0xFF);
        }

        public int ComputeCrc()
        {
            return CrcCalculator.Crc(Data());
        }

        public bool Check(byte command)
        {
            return Valid() && CommandByte() == command;
        }

        public bool Valid()
        {
            return StartByte() == STX && EndByte() == ETX && ValidCrc();
        }

        public bool ValidCrc()
        {
            return ComputeCrc() == ExtractCrc();
        }

        public override String ToString()
        {
            var buf = new StringBuilder();
            for (int i = 0; i < sequence.Length; i++)
            {
                buf.Append(String.Format("{0:x4} ", sequence[i])); // & 0xFF);
            }
            return buf.ToString();
        }

        public String ToStringCrc()
        {
            if (sequence.Length >= 6)
            {
                return String.Format("{0:x4}", ComputeCrc());
            }
            return "none";
        }
    }
}