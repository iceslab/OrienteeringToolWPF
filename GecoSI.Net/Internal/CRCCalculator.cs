/**
* Released by SPORTident under the CC BY 3.0 license as Java Class.
* Ported to C# by Yannis Guedel
 * 
 * 
* This work is licensed under the Creative Commons Attribution 3.0 Unported License.
* To view a copy of this license, visit http://creativecommons.org/licenses/by/3.0/
* or send a letter to Creative Commons, 444 Castro Street, Suite 900,
* Mountain View, California, 94041, USA.
*/
namespace GecoSI.Net.Internal
{
    public class CrcCalculator
    {
        private static readonly int POLY = 0x8005;
        private static readonly int BITF = 0x8000;


        public static int Crc(byte[] buffer)
        {
            int count = buffer.Length;
            int i, j;
            int tmp, val; // 16 Bit
            int ptr = 0;

            tmp = (short) (buffer[ptr++] << 8 | (buffer[ptr++] & 0xFF));

            if (count > 2)
            {
                for (i = count/2; i > 0; i--) // only even counts !!! and more
                    // than 4
                {
                    if (i > 1)
                    {
                        val = buffer[ptr++] << 8 | (buffer[ptr++] & 0xFF);
                    }
                    else
                    {
                        if (count%2 == 1)
                        {
                            val = buffer[count - 1] << 8;
                        }
                        else
                        {
                            val = 0; // last value with 0 // last 16 bit value
                        }
                    }

                    for (j = 0; j < 16; j++)
                    {
                        if ((tmp & BITF) != 0)
                        {
                            tmp <<= 1;

                            if ((val & BITF) != 0)
                            {
                                tmp++; // rotate carry
                            }
                            tmp ^= POLY;
                        }
                        else
                        {
                            tmp <<= 1;

                            if ((val & BITF) != 0)
                            {
                                tmp++; // rotate carry
                            }
                        }
                        val <<= 1;
                    }
                }
            }
            return (tmp & 0xFFFF);
        }
    }
}