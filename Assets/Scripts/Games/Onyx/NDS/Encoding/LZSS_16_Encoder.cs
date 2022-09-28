using System;
using System.IO;

namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class LZSS_16_Encoder : IStreamEncoder
    {
        public string Name => "Onyx_LZSS_16";
        
        public void DecodeStream(Stream input, Stream output)
        {
            Reader reader = new(input);

            Writer writer = new(output);
            Reader outReader = new(output);

            byte magic = reader.ReadByte();

            if (magic != 0x10)
                throw new Exception("The data is not LZSS compressed");

            uint decompLength = reader.ReadUInt24();

            uint uVar4 = 0;
            uint uVar3 = 0;
            do
            {
                if ((int)decompLength < 1)
                    break;

                uint uVar8 = reader.ReadByte();
                int iVar9 = 8;

                while (0 < iVar9) 
                {
                    if ((uVar8 & 0x80) == 0)
                    {
                        uVar4 |= (uint)reader.ReadByte() << (int)uVar3;
                        decompLength--;
                        uVar3 ^= 8;

                        if (uVar3 == 0)
                        {
                            writer.Write((short)uVar4);
                            uVar4 = 0;
                        }
                    }
                    else
                    {
                        byte b1 = reader.ReadByte();
                        byte b2 = reader.ReadByte();

                        int iVar6 = (b1 >> 4) + 3;
                        uint uVar5 = (uint)((b2 | ((b1 & 0xf) << 8)) + 1);
                        uint uVar11 = 8 - uVar3 ^ (uVar5 & 1) << 3;
                        decompLength -= (uint)iVar6;
                        int iVar7;
                        bool bVar1;

                        do
                        {
                            uVar11 ^= 8;

                            long pos = output.Position;
                            output.Position = pos - (uVar5 + (8 - uVar3 >> 3) & 0xfffffffe);
                            uVar4 |= (uint)(((int)((uint)outReader.ReadUInt16() & 0xff << (int)(uVar11 & 0xff)) >> (int)(uVar11 & 0xff)) << (int)uVar3);
                            output.Position = pos;

                            uVar3 ^= 8;
                            if (uVar3 == 0)
                            {
                                writer.Write((short)uVar4);
                                uVar4 = 0;
                            }

                            iVar7 = iVar6 + -1;
                            bVar1 = 0 < iVar6;
                            iVar6 = iVar7;
                        } while (iVar7 != 0 && bVar1);
                    }
                    
                    if ((int)decompLength < 1) 
                        break;
                    
                    uVar8 <<= 1;
                    iVar9--;
                }
            } while (true);
        }

        public void EncodeStream(Stream input, Stream output)
        {
            throw new NotImplementedException();
        }
    }
}