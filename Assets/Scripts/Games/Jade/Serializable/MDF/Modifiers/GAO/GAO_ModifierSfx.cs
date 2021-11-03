using System;
using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GAO_ModifierSfx : MDF_Modifier {
        public uint Version { get; set; }
        public uint UInt_00 { get; set; }
        public uint Type { get; set; }

        public uint Type0_Flags { get; set; }
        public float Type0_Float1 { get; set; }
        public float Type0_V3_Float { get; set; }
        public uint Type0_V4_UInt_Editor { get; set; }
        public Jade_Reference<OBJ_GameObject> Type0_GameObject1 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type0_GameObject2 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type0_GameObject3 { get; set; }
        public float Type0_V6_Float1 { get; set; }
        public float Type0_V6_Float2 { get; set; }
        public float Type0_V6_Float3 { get; set; }
        public float Type0_V7_Float1 { get; set; }
        public uint Type0_V7_UInt2 { get; set; }
        public float Type0_V8_Float1 { get; set; }
        public float Type0_V8_Float2 { get; set; }
        public float Type0_V8_Float3 { get; set; }
        public float Type0_V8_Float4 { get; set; }
        public float Type0_V8_Float5 { get; set; }
        public uint Type0_V10_UInt1 { get; set; }
        public uint Type0_V17_UInt1 { get; set; }
        public uint Type0_V11_UInt1 { get; set; }
        public uint Type0_V10_Count { get; set; }
        public Type0_Struct[] Type0_V10_Structs { get; set; }
        public float Type0_V12_Float1 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type0_V16_GameObject { get; set; }

        public Jade_Reference<OBJ_GameObject> Type1_GameObject1 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type1_GameObject2 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type1_GameObject3 { get; set; }
        public float Type1_Float1 { get; set; }
        public Jade_Vector Type1_Vector1 { get; set; }
        public Jade_Vector Type1_Vector2 { get; set; }
        public Jade_Vector Type1_Vector3 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type1_GameObject4 { get; set; }
        public Jade_Vector Type1_Vector4 { get; set; }

        public Jade_Vector Type2_Vector { get; set; }
        public float Type2_Float { get; set; }
        public float Type2_V22_Float { get; set; }
        public float Type2_V23_Float { get; set; }
        public float Type2_V25_Float { get; set; }

        public uint Type3_UInt1 { get; set; }
        public uint Type3_UInt2 { get; set; }

        public uint Type4_UInt1 { get; set; }
        public Jade_Vector Type4_Vector2 { get; set; }
        public float Type4_Float3 { get; set; }
        public uint Type4_UInt4 { get; set; }
        public uint Type4_UInt5 { get; set; }
        public uint Type4_UInt6 { get; set; }
        public uint Type4_UInt7 { get; set; }
        public uint Type4_UInt8 { get; set; }

        public float Type5_Float1 { get; set; }
        public uint Type5_UInt2 { get; set; }
        public uint Type5_UInt3 { get; set; }
        public uint Type5_UInt4 { get; set; }
        public uint Type5_UInt5 { get; set; }
        public uint Type5_UInt6 { get; set; }

        public uint Type6_UInt1 { get; set; }
        public uint Type6_UInt2 { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
            Version = s.Serialize<uint>(Version, name: nameof(Version));
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Type = s.Serialize<uint>(Type, name: nameof(Type));
            switch (Type) {
                case 0:
					Type0_Flags = s.Serialize<uint>(Type0_Flags, name: nameof(Type0_Flags));
					Type0_Float1 = s.Serialize<float>(Type0_Float1, name: nameof(Type0_Float1));
					if (Version >= 3) Type0_V3_Float = s.Serialize<float>(Type0_V3_Float, name: nameof(Type0_V3_Float));
					if (!Loader.IsBinaryData && Version >= 4 && Version < 8)
                        Type0_V4_UInt_Editor = s.Serialize<uint>(Type0_V4_UInt_Editor, name: nameof(Type0_V4_UInt_Editor));
					Type0_GameObject1 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type0_GameObject1, name: nameof(Type0_GameObject1))?.Resolve();
					if (Version >= 5) Type0_GameObject2 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type0_GameObject2, name: nameof(Type0_GameObject2))?.Resolve();
					if (Version >= 1) Type0_GameObject3 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type0_GameObject3, name: nameof(Type0_GameObject3))?.Resolve();
                    if (Version >= 6) {
						Type0_V6_Float1 = s.Serialize<float>(Type0_V6_Float1, name: nameof(Type0_V6_Float1));
						Type0_V6_Float2 = s.Serialize<float>(Type0_V6_Float2, name: nameof(Type0_V6_Float2));
						Type0_V6_Float3 = s.Serialize<float>(Type0_V6_Float3, name: nameof(Type0_V6_Float3));
					}
                    if (Version >= 7) {
						Type0_V7_Float1 = s.Serialize<float>(Type0_V7_Float1, name: nameof(Type0_V7_Float1));
						Type0_V7_UInt2 = s.Serialize<uint>(Type0_V7_UInt2, name: nameof(Type0_V7_UInt2));
					}
                    if (Version >= 8) {
						Type0_V8_Float1 = s.Serialize<float>(Type0_V8_Float1, name: nameof(Type0_V8_Float1));
						Type0_V8_Float2 = s.Serialize<float>(Type0_V8_Float2, name: nameof(Type0_V8_Float2));
						Type0_V8_Float3 = s.Serialize<float>(Type0_V8_Float3, name: nameof(Type0_V8_Float3));
						Type0_V8_Float4 = s.Serialize<float>(Type0_V8_Float4, name: nameof(Type0_V8_Float4));
						Type0_V8_Float5 = s.Serialize<float>(Type0_V8_Float5, name: nameof(Type0_V8_Float5));
					}
                    if (Version >= 10) {
                        Type0_V10_UInt1 = s.Serialize<uint>(Type0_V10_UInt1, name: nameof(Type0_V10_UInt1));
                        if (Version >= 17) Type0_V17_UInt1 = s.Serialize<uint>(Type0_V17_UInt1, name: nameof(Type0_V17_UInt1));
                        if (Version >= 11) Type0_V11_UInt1 = s.Serialize<uint>(Type0_V11_UInt1, name: nameof(Type0_V11_UInt1));
						Type0_V10_Count = s.Serialize<uint>(Type0_V10_Count, name: nameof(Type0_V10_Count));
						Type0_V10_Structs = s.SerializeObjectArray<Type0_Struct>(Type0_V10_Structs, Type0_V10_Count,
                            onPreSerialize: str => str.Pre_Modifier = this, name: nameof(Type0_V10_Structs));
					}
					if (Version >= 12) Type0_V12_Float1 = s.Serialize<float>(Type0_V12_Float1, name: nameof(Type0_V12_Float1));
					if (Version >= 16) Type0_V16_GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type0_V16_GameObject, name: nameof(Type0_V16_GameObject))?.Resolve();
					break;
                case 1:
					Type1_GameObject1 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type1_GameObject1, name: nameof(Type1_GameObject1))?.Resolve();
					Type1_GameObject2 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type1_GameObject2, name: nameof(Type1_GameObject2))?.Resolve();
					Type1_GameObject3 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type1_GameObject3, name: nameof(Type1_GameObject3))?.Resolve();
					Type1_Float1 = s.Serialize<float>(Type1_Float1, name: nameof(Type1_Float1));
                    if (Version >= 9) {
						Type1_Vector1 = s.SerializeObject<Jade_Vector>(Type1_Vector1, name: nameof(Type1_Vector1));
						Type1_Vector2 = s.SerializeObject<Jade_Vector>(Type1_Vector2, name: nameof(Type1_Vector2));
						Type1_Vector3 = s.SerializeObject<Jade_Vector>(Type1_Vector3, name: nameof(Type1_Vector3));
					}
                    if (Version >= 10) {
						Type1_GameObject4 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type1_GameObject4, name: nameof(Type1_GameObject4))?.Resolve();
						Type1_Vector4 = s.SerializeObject<Jade_Vector>(Type1_Vector4, name: nameof(Type1_Vector4));
					}
					break;
                case 2:
					Type2_Vector = s.SerializeObject<Jade_Vector>(Type2_Vector, name: nameof(Type2_Vector));
					Type2_Float = s.Serialize<float>(Type2_Float, name: nameof(Type2_Float));
					if (Version >= 22) Type2_V22_Float = s.Serialize<float>(Type2_V22_Float, name: nameof(Type2_V22_Float));
					if (Version >= 23) Type2_V23_Float = s.Serialize<float>(Type2_V23_Float, name: nameof(Type2_V23_Float));
					if (Version >= 25) Type2_V25_Float = s.Serialize<float>(Type2_V25_Float, name: nameof(Type2_V25_Float));
					break;
                case 3:
					Type3_UInt1 = s.Serialize<uint>(Type3_UInt1, name: nameof(Type3_UInt1));
					Type3_UInt2 = s.Serialize<uint>(Type3_UInt2, name: nameof(Type3_UInt2));
					break;
                case 4:
                    if (Version >= 17) {
                        if (Version <= 18) Type4_UInt1 = s.Serialize<uint>(Type4_UInt1, name: nameof(Type4_UInt1));
                        if (Version == 17) Type4_Vector2 = s.SerializeObject<Jade_Vector>(Type4_Vector2, name: nameof(Type4_Vector2));
                        if (Version >= 18) {
                            Type4_Float3 = s.Serialize<float>(Type4_Float3, name: nameof(Type4_Float3));
                            Type4_UInt4 = s.Serialize<uint>(Type4_UInt4, name: nameof(Type4_UInt4));
                            Type4_UInt5 = s.Serialize<uint>(Type4_UInt5, name: nameof(Type4_UInt5));
                            Type4_UInt6 = s.Serialize<uint>(Type4_UInt6, name: nameof(Type4_UInt6));
                        }
                        if (Version >= 19) Type4_UInt7 = s.Serialize<uint>(Type4_UInt7, name: nameof(Type4_UInt7));
                        if (Version >= 24) Type4_UInt8 = s.Serialize<uint>(Type4_UInt8, name: nameof(Type4_UInt8));
                    }
					break;
                case 5:
                    if (Version >= 19) {
						Type5_Float1 = s.Serialize<float>(Type5_Float1, name: nameof(Type5_Float1));
						Type5_UInt2 = s.Serialize<uint>(Type5_UInt2, name: nameof(Type5_UInt2));
						Type5_UInt3 = s.Serialize<uint>(Type5_UInt3, name: nameof(Type5_UInt3));
						Type5_UInt4 = s.Serialize<uint>(Type5_UInt4, name: nameof(Type5_UInt4));
						Type5_UInt5 = s.Serialize<uint>(Type5_UInt5, name: nameof(Type5_UInt5));
						if (Version >= 20) Type5_UInt6 = s.Serialize<uint>(Type5_UInt6, name: nameof(Type5_UInt6));
					}
                    break;
                case 6:
                    if (Version >= 21) {
                        Type6_UInt1 = s.Serialize<uint>(Type6_UInt1, name: nameof(Type6_UInt1));
                        Type6_UInt2 = s.Serialize<uint>(Type6_UInt2, name: nameof(Type6_UInt2));
                    }
					break;
            }
		}

		public class Type0_Struct : BinarySerializable {
            public GAO_ModifierSfx Pre_Modifier { get; set; } // Set in onPreSerialize

            public float Float_0 { get; set; }
            public float[] Floats0 { get; set; }
            public float[] Floats1 { get; set; }
            public byte Byte_1 { get; set; }
            public byte[] Bytes2 { get; set; }
            public byte V13_Byte { get; set; }

            public float V15_Float1 { get; set; }
            public float V15_Float2 { get; set; }


            public uint Count => Pre_Modifier.Version >= 14 ? (uint)7 : 3;
            
			public override void SerializeImpl(SerializerObject s) {
				Float_0 = s.Serialize<float>(Float_0, name: nameof(Float_0));
				Floats0 = s.SerializeArray<float>(Floats0, Count, name: nameof(Floats0));
				Floats1 = s.SerializeArray<float>(Floats1, Count, name: nameof(Floats1));
				Byte_1 = s.Serialize<byte>(Byte_1, name: nameof(Byte_1));
				Bytes2 = s.SerializeArray<byte>(Bytes2, Count, name: nameof(Bytes2));
				if (Pre_Modifier.Version >= 13) V13_Byte = s.Serialize<byte>(V13_Byte, name: nameof(V13_Byte));
				if (Pre_Modifier.Version >= 15) {
					V15_Float1 = s.Serialize<float>(V15_Float1, name: nameof(V15_Float1));
					V15_Float2 = s.Serialize<float>(V15_Float2, name: nameof(V15_Float2));
				}
			}
		}
	}
}
