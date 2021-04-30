using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierXMEN : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint Type { get; set; }
		public uint Flags { get; set; }
		public uint Number_Of_Chhlaahhh { get; set; }
		public XMEN_Chhlaahhh[] Chhlaahhh { get; set; }
		public Jade_Reference<GEO_Object> Material { get; set; }
		public uint ProjectionMethod { get; set; }
		public float DTMin { get; set; }
		public uint UserID { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Number_Of_Chhlaahhh = s.Serialize<uint>(Number_Of_Chhlaahhh, name: nameof(Number_Of_Chhlaahhh));
			Chhlaahhh = s.SerializeObjectArray<XMEN_Chhlaahhh>(Chhlaahhh, Number_Of_Chhlaahhh, name: nameof(Chhlaahhh));
			Material = s.SerializeObject<Jade_Reference<GEO_Object>>(Material, name: nameof(Material));
			if(Type >= 1) ProjectionMethod = s.Serialize<uint>(ProjectionMethod, name: nameof(ProjectionMethod));
			if(Type >= 2) DTMin = s.Serialize<float>(DTMin, name: nameof(DTMin));
			if(Type >= 3) UserID = s.Serialize<uint>(UserID, name: nameof(UserID));
			//if ((Flags & 8) == 0) { // This is always 0 -- it does Flags &= 0xFFFFFFF7 after reading it
				Material?.Resolve();
			//}
		}

		public class XMEN_Chhlaahhh : BinarySerializable {
			public uint BonesNum { get; set; }
			public float Length { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				BonesNum = s.Serialize<uint>(BonesNum, name: nameof(BonesNum));
				Length = s.Serialize<float>(Length, name: nameof(Length));
			}
		}
	}
}
