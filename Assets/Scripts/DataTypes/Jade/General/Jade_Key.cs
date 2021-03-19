using System;

namespace R1Engine.Jade {
	public class Jade_Key : R1Serializable, IEquatable<Jade_Key> {
		public uint Key { get; set; }
		public bool IsNull => Key == 0 || Key == 0xFFFFFFFF;

		public override void SerializeImpl(SerializerObject s) {
			Key = s.Serialize<uint>(Key, name: nameof(Key));
			s.Log($"Key: {Key:X8}");
		}

		public Jade_Key() { }
		public Jade_Key(uint k) => Key = k;
		public static implicit operator uint(Jade_Key k) => k.Key;
		public static explicit operator Jade_Key(uint k) => new Jade_Key(k);
		public override string ToString() {
			return Key.ToString("X8");
		}

		public bool Equals(Jade_Key other) => this == (Jade_Key)other;

		public override int GetHashCode() {
			return Key.GetHashCode();
		}
		public override bool Equals(object obj) {
			return obj is Jade_Key && this == (Jade_Key)obj;
		}
		public static bool operator ==(Jade_Key x, Jade_Key y) {
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			return x.Key == y.Key;
		}
		public static bool operator !=(Jade_Key x, Jade_Key y) {
			return !(x == y);
		}
	}
}
