﻿using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_Vector : BinarySerializable {
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<float>(X, name: nameof(X));
			Y = s.Serialize<float>(Y, name: nameof(Y));
			Z = s.Serialize<float>(Z, name: nameof(Z));
		}
        public override bool UseShortLog => true;
		public override string ToString() => $"Vector({X}, {Y}, {Z})";
	}
}
