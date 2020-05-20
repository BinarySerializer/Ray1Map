using System;
using System.IO;

namespace R1Engine {
    /// <summary>
    /// Compresses/decompresses PC save data
    /// </summary>
    public class R1PCSaveEncoder : IStreamEncoder
    {
        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The encoded stream</param>
        /// <returns>The stream with the decoded data</returns>
        public Stream DecodeStream(Stream s) {
            var decompressedStream = new MemoryStream();

            using (Stream unxor = UnXORStream(s)) {
                //unxor.CopyTo(decompressedStream);
                using (Reader reader = new Reader(unxor, isLittleEndian: true)) {
                    byte[] compr_big_window = new byte[256 * 8];
                    for (int i = 0; i < 256; i++) {
                        for (int j = 0; j < 8; j++) {
                            compr_big_window[i * 8 + j] = (byte)i;
                        }
                    }
                    byte[] compr_window = new byte[8];
                    for (int j = 0; j < 8; j++) {
                        compr_window[j] = 0;
                    }
                    byte startByte = reader.ReadByte();
                    byte checksum = startByte;
                    byte bytesInWindow = startByte;
                    int compressedByte = -1;
                    uint decompressedSize = reader.ReadUInt32();
                    byte windowUpdateBitArray = 0;

                    reader.BeginCalculateChecksum(new NegativeChecksum8Calculator(checksum));
                    bool isFinished = false;
                    while (!isFinished) {
                        if (compressedByte == -1) {
                            if (reader.BaseStream.Position >= reader.BaseStream.Length) {
                                isFinished = true;
                            } else {
                                compressedByte = reader.ReadByte();
                                windowUpdateBitArray = reader.ReadByte();
                            }
                        } else {
                            if (reader.BaseStream.Position >= reader.BaseStream.Length) {
                                for (int i = 0; i < 8; i++) {
                                    decompressedStream.WriteByte(compr_window[i]);
                                }
                                isFinished = true;
                            } else {
                                compressedByte = reader.ReadByte();
                                if (reader.BaseStream.Position >= reader.BaseStream.Length) {
                                    // TODO: only if bytes in window > 0
                                    for (int i = 0; i < compressedByte; i++) {
                                        decompressedStream.WriteByte(compr_window[i]);
                                    }
                                    isFinished = true;
                                } else {
                                    windowUpdateBitArray = reader.ReadByte();
                                    for (int i = 0; i < 8; i++) {
                                        decompressedStream.WriteByte(compr_window[i]);
                                    }
                                }
                            }
                        }
                        if (!isFinished) {
                            int bigWindowIndex = compressedByte;
                            for (int i = 0; i < 8; i++) {
                                if (windowUpdateBitArray % 2 == 1) {
                                    compr_big_window[(bigWindowIndex * 8) + i] = reader.ReadByte();
                                }
                                compr_window[i] = compr_big_window[(bigWindowIndex * 8) + i];
                                windowUpdateBitArray /= 2;
                            }
                        }
                    }
                    byte endChecksum = reader.EndCalculateChecksum<byte>();
                    if (endChecksum != 0)
                        UnityEngine.Debug.LogWarning("Checksum failed! " + checksum + " - " + endChecksum);
                }
            }

            // Set position back to 0
            decompressedStream.Position = 0;

            // Return the compressed data stream
            return decompressedStream;
        }

        public Stream EncodeStream(Stream s) {
            throw new NotImplementedException();
        }

        private Stream UnXORStream(Stream s) {
            byte compr_incremental_xor = 0x57;

            var decompressedStream = new MemoryStream();
            Reader reader = new Reader(s, isLittleEndian: true); // No using, because we don't want to close the stream

            byte bytes_in_window = reader.ReadByte();
            bytes_in_window ^= 0x53;
            decompressedStream.WriteByte(bytes_in_window);

            uint decompressedSize = reader.ReadUInt32();
            decompressedSize ^= 0x54555657;
            decompressedStream.Write(BitConverter.GetBytes(decompressedSize), 0, 4);
            while (s.Position < s.Length) {
                byte b = reader.ReadByte();
                byte xor = 0;
                // Bit reverse
                for (int i = 0; i < 8; i++) {
                    xor = (byte)BitHelpers.SetBits(xor, BitHelpers.ExtractBits(compr_incremental_xor, 1, i), 1, 7 - i);
                }
                b = (byte)(b ^ (xor ^ 0xB9));
                if (compr_incremental_xor == 0xFF) {
                    compr_incremental_xor = 0x0;
                } else {
                    compr_incremental_xor++;
                }
                decompressedStream.WriteByte(b);
            }
            decompressedStream.Position = 0;
            return decompressedStream;
        }

        public class NegativeChecksum8Calculator : IChecksumCalculator<byte> {

            public NegativeChecksum8Calculator(byte checksum) {
                ChecksumValue = checksum;
            }
            /// <summary>
            /// Adds a byte to the checksum
            /// </summary>
            /// <param name="b">The byte to add</param>
            public void AddByte(byte b) {
                ChecksumValue = (byte)((256 + ChecksumValue - b) % 256);
            }

            /// <summary>
            /// Adds an array of bytes to the checksum
            /// </summary>
            /// <param name="bytes">The bytes to add</param>
            public void AddBytes(byte[] bytes) {
                foreach (var b in bytes)
                    AddByte(b);
            }

            /// <summary>
            /// The current checksum value
            /// </summary>
            public byte ChecksumValue { get; set; }
        }
    }
}