using System;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class TEXT_Strings : Jade_File {
		public override string Export_Extension => "txs";
		public TextString[] Strings { get; set; }

		public TEXT_Ids Ids { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Strings = s.SerializeObjectArrayUntil<TextString>(Strings, t => s.CurrentAbsoluteOffset >= Offset.AbsoluteOffset + FileSize, name: nameof(Strings));
		}

		public class TextString : BinarySerializable {
			public string String { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				String = s.SerializeString(String, encoding: Jade_BaseManager.Encoding, name: nameof(String));
			}
		}
	}
}
