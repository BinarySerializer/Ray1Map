using System;
using System.Linq;
using BinarySerializer;

namespace R1Engine.Jade {
	public class TEXT_Ids : Jade_File {
		public override string Export_Extension => "txi";
		public TextId[] Ids { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Ids = s.SerializeObjectArrayUntil<TextId>(Ids, t => s.CurrentAbsoluteOffset >= Offset.AbsoluteOffset + FileSize, name: nameof(Ids));
		}

		public class TextId : BinarySerializable {
			public uint StringOffset { get; set; }
			public string Name { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				StringOffset = s.Serialize<uint>(StringOffset, name: nameof(StringOffset));
				if (!Loader.IsBinaryData) {
					Name = s.SerializeString(Name, length: 0x40, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				}
			}
		}
	}
}
