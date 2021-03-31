using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_VarEditorInfo : BinarySerializable {
		public int BufferOffset { get; set; }
		public int Int_04 { get; set; }
		public int Int_08 { get; set; }
		public ushort UShort_0C { get; set; }
		public ushort UShort_0E { get; set; }
		public int Int_10 { get; set; }

		public string SelectionString { get; set; }
		public string Description { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BufferOffset = s.Serialize<int>(BufferOffset, name: nameof(BufferOffset));
			Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
			Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
			UShort_0C = s.Serialize<ushort>(UShort_0C, name: nameof(UShort_0C));
			UShort_0E = s.Serialize<ushort>(UShort_0E, name: nameof(UShort_0E));
			Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
		}

		public void SerializeStrings(SerializerObject s) {
			if (Int_04 != 0)
				SelectionString = s.SerializeString(SelectionString, encoding: Jade_BaseManager.Encoding, name: nameof(SelectionString));
			if (Int_08 != 0)
				Description = s.SerializeString(Description, encoding: Jade_BaseManager.Encoding, name: nameof(Description));
		}
	}
}
