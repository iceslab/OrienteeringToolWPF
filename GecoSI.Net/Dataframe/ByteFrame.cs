using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GecoSI.Net.Dataframe
{
    public class ByteFrame
    {
        public ByteFrame()
        {
            
        }

        public ByteFrame(byte[] frame)
        {
            dataFrame = frame;
        }

        protected byte[] dataFrame;

        protected int ByteAt(int i)
        {
            return dataFrame[i] & 0xFF;
        }

        protected int WordAt(int i)
        {
            return ByteAt(i) << 8 | ByteAt(i + 1);
        }

        public int Block3At(int i)
        {
            return ByteAt(i) << 16 | WordAt(i + 1);
        }

        protected long TimestampAt(int i)
        {
            return 1000L * WordAt(i);
        }
    }
}
