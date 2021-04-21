using BinarySerializer;

namespace R1Engine.Jade {
	public class MDF_ModifierWeather : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }

		public uint UInt_08 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public float Float_14 { get; set; }
		public float Float_18 { get; set; }

		public override void SerializeImpl(SerializerObject s) {

			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			if (UInt_00 != 0) {
				UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));

				if (UInt_04 == 0) {
					UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
					Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
					Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
					Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
					Float_18 = s.Serialize<float>(Float_18, name: nameof(Float_18));
				}
			}
		}
	}
}
