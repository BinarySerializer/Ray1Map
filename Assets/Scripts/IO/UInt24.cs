using System;
using System.Runtime.InteropServices;

namespace R1Engine {
    [StructLayout(LayoutKind.Sequential)]
    public struct UInt24 {
        private Byte _b0;
        private Byte _b1;
        private Byte _b2;

        public UInt24(UInt32 value) {
            _b0 = (byte)((value) & 0xFF);
            _b1 = (byte)((value >> 8) & 0xFF);
            _b2 = (byte)((value >> 16) & 0xFF);
        }

        public UInt32 Value { get { return (uint)(_b0 | (_b1 << 8) | (_b2 << 16)); } }

        public static implicit operator uint(UInt24 d) => d.Value;
        public static explicit operator UInt24(uint b) => new UInt24(b);

		public override string ToString() {
			return Value.ToString();
		}
	}
}
