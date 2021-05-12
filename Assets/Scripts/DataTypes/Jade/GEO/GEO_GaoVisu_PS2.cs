using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GEO_GaoVisu_PS2 : BinarySerializable {
		public uint ElementsCount { get; set; }
		public Element[] Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			Elements = s.SerializeObjectArray<Element>(Elements, ElementsCount, name: nameof(Elements));
		}

		public class Element : BinarySerializable {

			public override void SerializeImpl(SerializerObject s) {
				throw new NotImplementedException($"TODO: Implement {GetType()}");
			}
		}
	}
}
