using System;

namespace R1Engine.Jade {
	public class OBJ_GameObject : Jade_File {
		public Jade_FileType Type { get; set; }
		public uint UInt_04_Editor { get; set; }
		public uint UInt_04 { get; set; }
		public uint FlagsIdentity { get; set; } // 4 = DYN ON, 0x8000 = MSG ON, 0x400000 = Hierarchy, 
		public ushort FlagsStatus { get; set; }
		public ushort FlagsControl { get; set; }
		public byte Byte_10 { get; set; }
		public byte Byte_11 { get; set; }
		public ushort UShort_12_Editor { get; set; }
		public byte Byte_12 { get; set; }
		public byte Byte_13 { get; set; }
		public byte FlagsDesign { get; set; }
		public byte FlagsFix { get; set; }
		public Jade_Matrix Matrix { get; set; }
		
		// ???
		public uint Field10_UInt_00_Editor { get; set; }
		public Jade_Vector Field10_Vector_00 { get; set; }
		public Jade_Vector Field10_Vector_0C { get; set; }
		public Jade_Vector Field10_Vector_18 { get; set; }
		public Jade_Vector Field10_Vector_24 { get; set; }

		// Geometric objects
		public Jade_Reference<GEO_Object> GeometricObject0 { get; set; }
		public Jade_Reference<GEO_Object> GeometricObject1 { get; set; }
		public int Int_7A { get; set; }
		public uint UInt_7E { get; set; } // Seem to be flags of some sort
		public uint Code { get; set; }
		public Jade_Reference<OBJ_GameObjectRLI> RLI { get; set; }
		public uint[] RLI_UInts { get; set; }

		// Hierarchy
		public Jade_Reference<OBJ_GameObject> Parent { get; set; }

		// Actions
		public uint Action0 { get; set; }
		public uint Action1 { get; set; }
		public uint Action2 { get; set; }
		public Jade_Reference<ACT_ActionKit> ActionKit { get; set; }

		// ???
		public byte Unk_Type { get; set; }
		public bool Unk_Bool { get; set; }
		public byte Unk_Byte_02 { get; set; }
		public byte Unk_Byte_03 { get; set; }
		public Jade_Vector Unk_Type2_Vector { get; set; }
		public Jade_Matrix Unk_Type7_Matrix { get; set; }
		public float Unk_Type6_Float_0 { get; set; }
		public float Unk_Type6_Float_1 { get; set; }
		public float Unk_Float_00 { get; set; }
		public float Unk_Bool_Float_0 { get; set; }
		public float Unk_Bool_Float_1 { get; set; }
		public float Unk_Bool_Float_2 { get; set; }
		public uint Unk_Type4_UInt { get; set; }
		public float[] Unk_Type4_Floats { get; set; }

		// ???
		public uint Unk2_Count { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
			if(Type.Type != Jade_FileType.FileType.OBJ_GameObject)
				throw new Exception($"Parsing failed: File at {Offset} was not of type {Jade_FileType.FileType.OBJ_GameObject}");

			if(!Loader.IsBinaryData) UInt_04_Editor = s.Serialize<uint>(UInt_04_Editor, name: nameof(UInt_04_Editor));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			FlagsIdentity = s.Serialize<uint>(FlagsIdentity, name: nameof(FlagsIdentity));
			s.SerializeBitValues<uint>(bitFunc => {
				FlagsStatus = (ushort)bitFunc(FlagsStatus, 16, name: nameof(FlagsStatus));
				FlagsControl = (ushort)bitFunc(FlagsControl, 16, name: nameof(FlagsControl));
			});
			Byte_10 = s.Serialize<byte>(Byte_10, name: nameof(Byte_10));
			Byte_11 = s.Serialize<byte>(Byte_11, name: nameof(Byte_11));
			if(!Loader.IsBinaryData) UShort_12_Editor = s.Serialize<ushort>(UShort_12_Editor, name: nameof(UShort_12_Editor));
			Byte_12 = s.Serialize<byte>(Byte_12, name: nameof(Byte_12));
			Byte_13 = s.Serialize<byte>(Byte_13, name: nameof(Byte_13));
			FlagsDesign = s.Serialize<byte>(FlagsDesign, name: nameof(FlagsDesign));
			FlagsFix = s.Serialize<byte>(FlagsFix, name: nameof(FlagsFix));
			Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));

			if (true) { // if gao->field_10
				if (!Loader.IsBinaryData) Field10_UInt_00_Editor = s.Serialize<uint>(Field10_UInt_00_Editor, name: nameof(Field10_UInt_00_Editor));
				Field10_Vector_00 = s.SerializeObject<Jade_Vector>(Field10_Vector_00, name: nameof(Field10_Vector_00));
				Field10_Vector_0C = s.SerializeObject<Jade_Vector>(Field10_Vector_0C, name: nameof(Field10_Vector_0C));
				if ((FlagsIdentity & 0x80000) != 0) {
					Field10_Vector_18 = s.SerializeObject<Jade_Vector>(Field10_Vector_18, name: nameof(Field10_Vector_18));
					Field10_Vector_24 = s.SerializeObject<Jade_Vector>(Field10_Vector_24, name: nameof(Field10_Vector_24));
				}
			}
			if ((FlagsIdentity & 0x1000) != 0) {
				if ((FlagsIdentity & 0x4000) != 0) {
					GeometricObject0 = s.SerializeObject<Jade_Reference<GEO_Object>>(GeometricObject0, name: nameof(GeometricObject0))?.Resolve();
					GeometricObject1 = s.SerializeObject<Jade_Reference<GEO_Object>>(GeometricObject1, name: nameof(GeometricObject1))?.Resolve();
					Int_7A = s.Serialize<int>(Int_7A, name: nameof(Int_7A));
					UInt_7E = s.Serialize<uint>(UInt_7E, name: nameof(UInt_7E));
					Code = s.Serialize<uint>(Code, name: nameof(Code));
					if (Code == (uint)Jade_Code.RLI) {
						RLI = s.SerializeObject<Jade_Reference<OBJ_GameObjectRLI>>(RLI, name: nameof(RLI))?.Resolve();
					} else {
						RLI_UInts = s.SerializeArray<uint>(RLI_UInts, Code, name: nameof(RLI_UInts));
					}
				}
				if ((FlagsIdentity & 0x400000) != 0) {
					Parent = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Parent, name: nameof(Parent))?.Resolve();
				}
				if ((FlagsIdentity & 2) != 0) {
					Action0 = s.Serialize<uint>(Action0, name: nameof(Action0));
					Action1 = s.Serialize<uint>(Action1, name: nameof(Action1));
					Action2 = s.Serialize<uint>(Action2, name: nameof(Action2));
					ActionKit = s.SerializeObject<Jade_Reference<ACT_ActionKit>>(ActionKit, name: nameof(ActionKit))?.Resolve();
				}
				if ((FlagsIdentity & 0x10000000) != 0) {
					Unk_Type = s.Serialize<byte>(Unk_Type, name: nameof(Unk_Type));
					Unk_Bool = s.Serialize<bool>(Unk_Bool, name: nameof(Unk_Bool));
					Unk_Byte_02 = s.Serialize<byte>(Unk_Byte_02, name: nameof(Unk_Byte_02));
					Unk_Byte_03 = s.Serialize<byte>(Unk_Byte_03, name: nameof(Unk_Byte_03));
					if (Unk_Type >= 2)
						Unk_Type2_Vector = s.SerializeObject<Jade_Vector>(Unk_Type2_Vector, name: nameof(Unk_Type2_Vector));
					if (Unk_Type >= 7)
						Unk_Type7_Matrix = s.SerializeObject<Jade_Matrix>(Unk_Type7_Matrix, name: nameof(Unk_Type7_Matrix));
					if (Unk_Type >= 6) {
						Unk_Type6_Float_0 = s.Serialize<float>(Unk_Type6_Float_0, name: nameof(Unk_Type6_Float_0));
						Unk_Type6_Float_1 = s.Serialize<float>(Unk_Type6_Float_1, name: nameof(Unk_Type6_Float_1));
					}
					Unk_Float_00 = s.Serialize<float>(Unk_Float_00, name: nameof(Unk_Float_00));
					if (Unk_Bool) {
						Unk_Bool_Float_0 = s.Serialize<float>(Unk_Bool_Float_0, name: nameof(Unk_Bool_Float_0));
						Unk_Bool_Float_1 = s.Serialize<float>(Unk_Bool_Float_1, name: nameof(Unk_Bool_Float_1));
						Unk_Bool_Float_2 = s.Serialize<float>(Unk_Bool_Float_2, name: nameof(Unk_Bool_Float_2));
					}
					if (Unk_Type >= 4) {
						Unk_Type4_UInt = s.Serialize<uint>(Unk_Type4_UInt, name: nameof(Unk_Type4_UInt));
						Unk_Type4_Floats = s.SerializeArray<float>(Unk_Type4_Floats, 10, name: nameof(Unk_Type4_Floats));
					}
				}
				if ((FlagsIdentity & 0x200000) != 0) {
					Unk2_Count = s.Serialize<uint>(Unk2_Count, name: nameof(Unk2_Count));
				}
			}
			throw new NotImplementedException($"TODO: Implement {GetType()}");
		}
	}
}
