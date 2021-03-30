using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class GAO_ModifierShadow : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint Flags { get; set; }
		public Jade_TextureReference Texture { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public uint UInt_10 { get; set; }
		public uint UInt_Editor_14 { get; set; }
		public uint UInt_Editor_18 { get; set; }
		public float Float_14 { get; set; }
		public uint UInt_18 { get; set; }
		public float Float_1C { get; set; }
		public Jade_Vector Vector_20 { get; set; }
		public float Float_2C { get; set; }
		public uint UInt_30 { get; set; }
		public byte Byte_34 { get; set; }
		public bool HasGameObject { get; set; }
		public short Short_36 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
			if (!Loader.IsBinaryData) {
				UInt_Editor_14 = s.Serialize<uint>(UInt_Editor_14, name: nameof(UInt_Editor_14));
				UInt_Editor_18 = s.Serialize<uint>(UInt_Editor_18, name: nameof(UInt_Editor_18));
			}
			Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
			UInt_18 = s.Serialize<uint>(UInt_18, name: nameof(UInt_18));
			Float_1C = s.Serialize<float>(Float_1C, name: nameof(Float_1C));
			Vector_20 = s.SerializeObject<Jade_Vector>(Vector_20, name: nameof(Vector_20));
			Float_2C = s.Serialize<float>(Float_2C, name: nameof(Float_2C));
			UInt_30 = s.Serialize<uint>(UInt_30, name: nameof(UInt_30));
			Byte_34 = s.Serialize<byte>(Byte_34, name: nameof(Byte_34));
			HasGameObject = s.Serialize<bool>(HasGameObject, name: nameof(HasGameObject));
			Short_36 = s.Serialize<short>(Short_36, name: nameof(Short_36));
			if (HasGameObject) {
				GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
			}
			Texture?.Resolve();
		}
	}
}
