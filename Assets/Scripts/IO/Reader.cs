using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R1Engine {
    public class Reader : BinaryReader {
        public delegate void ReadAction(Reader reader, Pointer offset);
        bool isLittleEndian = true;
        uint bytesSinceAlignStart = 0;
        bool autoAlignOn = false;
        byte? xorKey = null;
        IChecksumCalculator checksumCalculator = null;
        public Reader(System.IO.Stream stream) : base(stream) { isLittleEndian = true; }
        public Reader(System.IO.Stream stream, bool isLittleEndian) : base(stream) { this.isLittleEndian = isLittleEndian; }
        public bool AutoAligning {
            get { return autoAlignOn; }
            set { autoAlignOn = value; bytesSinceAlignStart = 0; }
        }

        public override int ReadInt32() {
            var data = ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public override float ReadSingle() {
            var data = ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        public override Int16 ReadInt16() {
            var data = ReadBytes(2);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public override UInt16 ReadUInt16() {
            var data = ReadBytes(2);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public override Int64 ReadInt64() {
            var data = ReadBytes(8);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public override UInt32 ReadUInt32() {
            var data = ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
		}

		public override UInt64 ReadUInt64() {
			var data = ReadBytes(8);
			if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
			return BitConverter.ToUInt64(data, 0);
		}

		public override byte[] ReadBytes(int count) {
            byte[] bytes = base.ReadBytes(count);
            if(autoAlignOn) bytesSinceAlignStart += (uint)bytes.Length;
            if (xorKey.HasValue) {
                for (int i = 0; i < count; i++) {
                    bytes[i] = (byte)(bytes[i] ^ xorKey.Value);
                }
            }
            if (checksumCalculator != null) {
                checksumCalculator.AddBytes(bytes);
            }
            return bytes;
        }

        public override sbyte ReadSByte() {
            sbyte result = base.ReadSByte();
            if (autoAlignOn) bytesSinceAlignStart++;
            if (xorKey.HasValue) {
                result = (sbyte)(result ^ xorKey.Value);
            }
            if (checksumCalculator != null) {
                checksumCalculator.AddByte((byte)result);
            }
            return result;
        }

        public override byte ReadByte() {
            byte result = base.ReadByte();
            if(autoAlignOn) bytesSinceAlignStart++;
            if (xorKey.HasValue) {
                result = (byte)(result ^ xorKey.Value);
            }
            if (checksumCalculator != null) {
                checksumCalculator.AddByte(result);
            }
            return result;
        }

        public string ReadNullDelimitedString(Encoding encoding = null) {
            List<byte> bytes = new List<byte>();
            byte b = ReadByte();
            while (b != 0x0) {
                bytes.Add(b);
                b = ReadByte();
            }
            if (bytes.Count > 0) {
                if (encoding == null)
                    encoding = Settings.StringEncoding;
                return encoding.GetString(bytes.ToArray());
            }
            return "";
        }

        public string ReadString(int size, Encoding encoding = null) {
            if (encoding == null)
                encoding = Settings.StringEncoding;
			byte[] bytes = ReadBytes(size);
			int firstIndexOf = Array.IndexOf(bytes, (byte)0x0);
			if (firstIndexOf >= 0 && firstIndexOf < bytes.Length) {
				if(firstIndexOf == 0) return "";
				Array.Resize(ref bytes, firstIndexOf);
			}
			return encoding.GetString(bytes);
			//return System.Text.Encoding.UTF8.GetString(ReadBytes(size)).TrimEnd('\0');
		}

		#region Alignment
		// To make sure position is a multiple of alignBytes
		public void Align(int alignBytes) {
            if (BaseStream.Position % alignBytes != 0) {
                ReadBytes(alignBytes - (int)(BaseStream.Position % alignBytes));
            }
        }
        public void AlignOffset(int alignBytes, int offset) {
            if ((BaseStream.Position - offset) % alignBytes != 0) {
                ReadBytes(alignBytes - (int)((BaseStream.Position - offset) % alignBytes));
            }
        }

        // To make sure position is a multiple of alignBytes after reading a block of blocksize, regardless of prior position
        public void Align(int blockSize, int alignBytes) {
            int rest = blockSize % alignBytes;
            if (rest > 0) {
                byte[] aligned = ReadBytes(alignBytes - rest);
                foreach (byte b in aligned) if (b != 0x0) throw new Exception("A data byte was skipped during alignment");
            }
        }
        
        public void AutoAlign(int alignBytes) {
            if (bytesSinceAlignStart % alignBytes != 0) {
                ReadBytes(alignBytes - (int)(bytesSinceAlignStart % alignBytes));
            }
            bytesSinceAlignStart = 0;
        }
		#endregion

		#region XOR & Checksum
		public void BeginXOR(byte xorKey) {
            this.xorKey = xorKey;
        }
        public void EndXOR() {
            this.xorKey = null;
        }
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