using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierAnimatedGAO : MDF_Modifier {
		public uint Type { get; set; }
		public uint Flags { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public Jade_Vector Type2_Vector { get; set; }
		public Entry[] Entries { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Type = s.Serialize<uint>(Type, name: nameof(Type));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			if (Type >= 2) {
				GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
				Type2_Vector = s.SerializeObject<Jade_Vector>(Type2_Vector, name: nameof(Type2_Vector));
			}
			if(Entries == null) Entries = new Entry[9];
			for (int i = 0; i < Entries.Length; i++) {
				if (BitHelpers.ExtractBits((int)Flags, 1, (i + i / 3 + 1)) == 1) {
					Entries[i] = s.SerializeObject<Entry>(Entries[i], onPreSerialize: e => {
						e.Modifier = this;
					}, name: $"{nameof(Entries)}[{i}]");
				}
			}
		}

		public class Entry : BinarySerializable {
			public GAO_ModifierAnimatedGAO Modifier { get; set; }

			public uint Type { get; set; }

			public float Type0_Float_0 { get; set; }
			public float Type0_Float_1 { get; set; }
			public float Type0_Float_2 { get; set; }
			public float Type0_Float_3 { get; set; }
			public float Type0_Float_4 { get; set; }
			public uint Type0_UInt_5 { get; set; }

			public float Type1_Float_0 { get; set; }
			public float Type1_Float_1 { get; set; }
			public float Type1_Float_2 { get; set; }
			public float Type1_Float_3 { get; set; }

			public float Type2_Float_0 { get; set; }
			public float Type2_Float_1 { get; set; }
			public float Type2_Float_2 { get; set; }
			public uint Type2_UInt_3 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Type = s.Serialize<uint>(Type, name: nameof(Type));
				switch (Type) {
					case 0:
						Type0_Float_0 = s.Serialize<float>(Type0_Float_0, name: nameof(Type0_Float_0));
						Type0_Float_1 = s.Serialize<float>(Type0_Float_1, name: nameof(Type0_Float_1));
						Type0_Float_2 = s.Serialize<float>(Type0_Float_2, name: nameof(Type0_Float_2));
						Type0_Float_3 = s.Serialize<float>(Type0_Float_3, name: nameof(Type0_Float_3));
						Type0_Float_4 = s.Serialize<float>(Type0_Float_4, name: nameof(Type0_Float_4));
						Type0_UInt_5 = s.Serialize<uint>(Type0_UInt_5, name: nameof(Type0_UInt_5));
						break;
					case 1:
						Type1_Float_0 = s.Serialize<float>(Type1_Float_0, name: nameof(Type1_Float_0));
						Type1_Float_1 = s.Serialize<float>(Type1_Float_1, name: nameof(Type1_Float_1));
						Type1_Float_2 = s.Serialize<float>(Type1_Float_2, name: nameof(Type1_Float_2));
						Type1_Float_3 = s.Serialize<float>(Type1_Float_3, name: nameof(Type1_Float_3));
						break;
					case 2:
						Type2_Float_0 = s.Serialize<float>(Type2_Float_0, name: nameof(Type2_Float_0));
						Type2_Float_1 = s.Serialize<float>(Type2_Float_1, name: nameof(Type2_Float_1));
						if(Modifier.Type > 0) Type2_Float_2 = s.Serialize<float>(Type2_Float_2, name: nameof(Type2_Float_2));
						Type2_UInt_3 = s.Serialize<uint>(Type2_UInt_3, name: nameof(Type2_UInt_3));
						break;
				}
			}
		}
	}
}
