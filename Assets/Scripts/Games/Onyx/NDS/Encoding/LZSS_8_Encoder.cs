using System;
using System.IO;

namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class LZSS_8_Encoder : IStreamEncoder
    {
        public string Name => "Onyx_LZSS_8";
        
        public void DecodeStream(Stream input, Stream output)
        {
            Reader reader = new(input);

            byte magic = reader.ReadByte();

            if (magic != 0x10)
                throw new Exception("The data is not LZSS compressed");

            uint decompLength = reader.ReadUInt24();

            byte[] outputBuffer = new byte[decompLength];
            int outputBufferPos = 0;

            do
            {
                if ((int)decompLength < 1)
                    break;

                uint uVar9 = reader.ReadByte();
                int iVar8 = 8;
                while (0 < iVar8)
                {
                    if ((uVar9 & 0x80) == 0)
                    {
                        outputBuffer[outputBufferPos] = reader.ReadByte();
                        outputBufferPos++;
                        decompLength--;
                    }
                    else
                    {
                        byte b1 = reader.ReadByte();
                        byte b2 = reader.ReadByte();
                        int iVar6 = (b1 >> 4) + 3;
                        decompLength -= (uint)iVar6;
                        bool bVar1;
                        do
                        {
                            outputBuffer[outputBufferPos] = outputBuffer[outputBufferPos - ((b2 | ((b1 & 0xf) << 8)) + 1)];
                            outputBufferPos++;
                            bVar1 = 0 < iVar6;
                            iVar6 += -1;
                        } while (iVar6 != 0 && bVar1);
                    }

                    if ((int)decompLength < 1) 
                        break;

                    uVar9 <<= 1;
                    iVar8--;
                }
            } while (true);

            output.Write(outputBuffer, 0, outputBuffer.Length);
        }

        public void EncodeStream(Stream input, Stream output)
        {
            throw new NotImplementedException();
        }
    }
}