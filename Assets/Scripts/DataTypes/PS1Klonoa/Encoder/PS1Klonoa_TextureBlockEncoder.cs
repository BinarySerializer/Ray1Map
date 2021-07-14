using System.IO;

namespace BinarySerializer.KlonoaDTP
{
    public class PS1Klonoa_TextureBlockEncoder : IStreamEncoder
    {
        public PS1Klonoa_TextureBlockEncoder(uint decompressedSize)
        {
            DecompressedSize = decompressedSize;
        }

        public string Name => $"PS1Klonoa_TextureBlockEncoder";

        public uint DecompressedSize { get; }

        public Stream DecodeStream(Stream s)
        {
            // Create a reader for the input
            var reader = new Reader(s, isLittleEndian: true);

            // Create a stream to store the decompressed data
            var decompressedStream = new MemoryStream();

            var writer = new Writer(decompressedStream, isLittleEndian: true, leaveOpen: true);

            // Re-implemented from 0x800171f0
            int currentDecompCount = 0;
            bool bVar1 = false;
            uint uVar6 = 0;
            int uVar7 = 0x20;
            uint uVar8 = 0;
            uint in_t4 = 0;
            uint in_t6 = 0;

            do
            {
                uint uVar2 = in_t4 >> (uVar7 & 0x1f);

                if (!bVar1)
                {
                    uVar2 = reader.ReadUInt32();
                    uVar7 = 0;
                    in_t4 = uVar2;
                }

                uVar7 += 8;

                if ((uVar2 & 0x80) == 0)
                {
                    uint uVar3 = uVar2 & 0xff;

                    in_t6 = uVar2;

                    while (--uVar3 != 0xffffffff)
                    {
                        in_t6 = in_t4 >> (uVar7 & 0x1f);

                        if (0x1f < uVar7)
                        {
                            in_t6 = reader.ReadUInt32();
                            uVar7 = 0;
                            in_t4 = in_t6;
                        }

                        uVar8 = uVar8 | (in_t6 & 0xff) << (int)(uVar6 & 0x1f);
                        uVar6 += 8;
                        uVar7 += 8;

                        if (0x1f < (int)uVar6)
                        {
                            writer.Write(uVar8);
                            uVar6 = 0;
                            uVar8 = 0;
                            currentDecompCount += 4;
                        }
                    }
                }
                else
                {
                    uVar2 &= 0x7f;

                    while (--uVar2 != 0xffffffff)
                    {
                        uVar8 = uVar8 | (in_t6 & 0xff) << (int)(uVar6 & 0x1f);
                        uVar6 += 8;

                        if (0x1f < (int)uVar6)
                        {
                            writer.Write(uVar8);
                            uVar6 = 0;
                            uVar8 = 0;
                            currentDecompCount += 4;
                        }
                    }
                }

                bVar1 = uVar7 < 0x20;
            } while (currentDecompCount < DecompressedSize);

            // Set position back to 0
            decompressedStream.Position = 0;

            // Return the compressed data stream
            return decompressedStream;
        }

        public Stream EncodeStream(Stream s)
        {
            throw new System.NotImplementedException();
        }
    }
}