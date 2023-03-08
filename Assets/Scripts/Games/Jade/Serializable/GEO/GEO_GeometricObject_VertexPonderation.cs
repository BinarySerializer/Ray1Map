using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class GEO_GeometricObject_VertexPonderation : BinarySerializable, ISerializerShortLog {
		public float Weight {
			get => BitConverter.Int32BitsToSingle(WeightUpperBits << 16);
			set {
				WeightUpperBits = (short)(BitConverter.SingleToInt32Bits(value) >> 16);
			}
		}

		public ushort Index { get; set; }
		public short WeightUpperBits { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<uint>(b => {
				Index = b.SerializeBits<ushort>(Index, 16, name: nameof(Index));
				WeightUpperBits = b.SerializeBits<short>(WeightUpperBits, 16, sign: SignedNumberRepresentation.TwosComplement, name: nameof(WeightUpperBits));
			});
		}

		public override string ToString() => $"VertexPonderation({Index}, {Weight})";
        public string ShortLog => ToString();
    }
}
