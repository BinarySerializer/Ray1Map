using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierShadow : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint Flags { get; set; }
		public Jade_TextureReference Texture { get; set; }
		public float XSizeFactor { get; set; }
		public float YSizeFactor { get; set; }
		public uint TextureIndex { get; set; }
		public uint NextShadowModifierPointer { get; set; }
		public uint GameObjectPointer { get; set; }
		public float ZAttenuationFactor { get; set; }
		public Jade_Color ShadowColor { get; set; }
		public float ZStart { get; set; }
		public Jade_Vector Center { get; set; }
		public float ZSizeFactor { get; set; }
		public uint ProjectionMethod { get; set; }
		public byte TextureTiling { get; set; }
		public byte Version { get; set; }
		public short Dummy { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture));
			XSizeFactor = s.Serialize<float>(XSizeFactor, name: nameof(XSizeFactor));
			YSizeFactor = s.Serialize<float>(YSizeFactor, name: nameof(YSizeFactor));
			TextureIndex = s.Serialize<uint>(TextureIndex, name: nameof(TextureIndex));
			if (!Loader.IsBinaryData) {
				NextShadowModifierPointer = s.Serialize<uint>(NextShadowModifierPointer, name: nameof(NextShadowModifierPointer));
				GameObjectPointer = s.Serialize<uint>(GameObjectPointer, name: nameof(GameObjectPointer));
			}
			ZAttenuationFactor = s.Serialize<float>(ZAttenuationFactor, name: nameof(ZAttenuationFactor));
			ShadowColor = s.SerializeObject<Jade_Color>(ShadowColor, name: nameof(ShadowColor));
			ZStart = s.Serialize<float>(ZStart, name: nameof(ZStart));
			Center = s.SerializeObject<Jade_Vector>(Center, name: nameof(Center));
			ZSizeFactor = s.Serialize<float>(ZSizeFactor, name: nameof(ZSizeFactor));
			ProjectionMethod = s.Serialize<uint>(ProjectionMethod, name: nameof(ProjectionMethod));
			TextureTiling = s.Serialize<byte>(TextureTiling, name: nameof(TextureTiling));
			if (s.GetR1Settings().EngineVersion >= EngineVersion.Jade_KingKong || !Loader.IsBinaryData) {
				Version = s.Serialize<byte>(Version, name: nameof(Version));
				Dummy = s.Serialize<short>(Dummy, name: nameof(Dummy));
				if (Version >= 1) {
					GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
				}
			}
			Texture?.Resolve(s, RRR2_readBool: true);
		}
	}
}
