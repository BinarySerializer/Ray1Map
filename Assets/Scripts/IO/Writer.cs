using System;
using System.IO;
using System.Text;

namespace R1Engine {
    public class Writer : BinaryWriter {
        public bool isLittleEndian = true;
        IXORCalculator xorCalculator;
        IChecksumCalculator checksumCalculator;
        uint bytesSinceAlignStart;
        bool autoAlignOn = false;

        public Writer(Stream stream) : base(stream) { isLittleEndian = true; }
        public Writer(Stream stream, bool isLittleEndian) : base(stream) { this.isLittleEndian = isLittleEndian; }

        public override void Write(Int32 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(Int16 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(UInt32 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(UInt16 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(Int64 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(UInt64 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public void Write(UInt24 value) {
            uint v = (uint)value;
            if (isLittleEndian) {
                Write((byte)(v & 0xFF));
                Write((byte)((v >> 8) & 0xFF));
                Write((byte)((v >> 16) & 0xFF));
            } else {
                Write((byte)((v >> 16) & 0xFF));
                Write((byte)((v >> 8) & 0xFF));
                Write((byte)(v & 0xFF));
            }
        }

        public override void Write(Single value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public override void Write(Double value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            Write(data);
        }

        public void WriteNullDelimitedString(string value, Encoding encoding = null) {
            if (encoding == null)
                encoding = Settings.StringEncoding;
            byte[] data = encoding.GetBytes(value + '\0');
            Write(data);
        }

        public void WriteString(string value, long size, Encoding encoding = null) {
            if (encoding == null)
                encoding = Settings.StringEncoding;
            byte[] data = encoding.GetBytes(value + '\0');
            if (data.Length != size) {
                Array.Resize(ref data, (int)size);
            }
            Write(data);
        }

        public override void Write(byte[] buffer) {
            if (buffer == null) 
                return;
            
            var data = buffer;

            if (checksumCalculator?.CalculateForDecryptedData == true)
                checksumCalculator?.AddBytes(data);

            if (xorCalculator != null) {
                // Avoid changing data in array, so create a copy
                data = new byte[buffer.Length];
                Array.Copy(buffer, 0, data, 0, buffer.Length);

                for (int i = 0; i < data.Length; i++) {
                    data[i] = xorCalculator.XORByte(data[i]);
                }
            }

            if (checksumCalculator?.CalculateForDecryptedData == false)
                checksumCalculator?.AddBytes(data);

            base.Write(data);
            
            if (autoAlignOn) 
                bytesSinceAlignStart += (uint)data.Length;
        }

        public override void Write(byte value) {

            if (checksumCalculator?.CalculateForDecryptedData == true)
                checksumCalculator?.AddByte(value);

            if (xorCalculator != null) {
                value = xorCalculator.XORByte(value);
            }

            if (checksumCalculator?.CalculateForDecryptedData == false)
                checksumCalculator?.AddByte(value);

            base.Write(value);
            
            if (autoAlignOn) 
                bytesSinceAlignStart++;
        }

        public override void Write(sbyte value) => Write((byte)value);

        #region Alignment
        // To make sure position is a multiple of alignBytes
        public void Align(int alignBytes) {
            if (BaseStream.Position % alignBytes != 0) {
                int length = alignBytes - (int)(BaseStream.Position % alignBytes);
                byte[] data = new byte[length];
                Write(data);
            }
        }
        public void AlignOffset(int alignBytes, int offset) {
            if ((BaseStream.Position - offset) % alignBytes != 0) {
                int length = alignBytes - (int)((BaseStream.Position - offset) % alignBytes);
                byte[] data = new byte[length];
                Write(data);
            }
        }

        // To make sure position is a multiple of alignBytes after reading a block of blocksize, regardless of prior position
        public void Align(int blockSize, int alignBytes) {
            int rest = blockSize % alignBytes;
            if (rest > 0) {
                int length = alignBytes - rest;
                byte[] data = new byte[length];
                Write(data);
            }
        }

        public void AutoAlign(int alignBytes) {
            if (bytesSinceAlignStart % alignBytes != 0) {
                int length = alignBytes - (int)(bytesSinceAlignStart % alignBytes);
                byte[] data = new byte[length];
                Write(data);
            }
            bytesSinceAlignStart = 0;
        }
        #endregion

        #region XOR & Checksum
        public void BeginXOR(IXORCalculator xorCalculator) {
            this.xorCalculator = xorCalculator;
        }
        public void EndXOR() {
            xorCalculator = null;
        }
        public IXORCalculator GetXORCalculator() => xorCalculator;
        public void BeginCalculateChecksum(IChecksumCalculator checksumCalculator) {
            this.checksumCalculator = checksumCalculator;
        }
        public T EndCalculateChecksum<T>() {
            IChecksumCalculator c = checksumCalculator;
            checksumCalculator = null;
            return ((IChecksumCalculator<T>)c).ChecksumValue;
        }
        #endregion
    }
}