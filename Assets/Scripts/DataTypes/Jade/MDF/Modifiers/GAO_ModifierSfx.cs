using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class GAO_ModifierSfx : MDF_Modifier {
        public uint Version { get; set; }
        public uint UInt_00 { get; set; }
        public uint Type { get; set; }

        public uint Type0_UInt0 { get; set; }
        public float Type0_Float1 { get; set; }
        public float Type0_V3_UInt { get; set; }
        public uint Type0_V4_UInt_Editor { get; set; }
        public Jade_Reference<OBJ_GameObject> Type0_GameObject1 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type0_GameObject2 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type0_GameObject3 { get; set; }
        public float Type0_V6_Float1 { get; set; }
        public float Type0_V6_Float2 { get; set; }
        public float Type0_V6_Float3 { get; set; }
        public uint Type0_V7_UInt1 { get; set; }
        public uint Type0_V7_UInt2 { get; set; }
        public uint Type0_V8_UInt1 { get; set; }
        public uint Type0_V8_UInt2 { get; set; }
        public uint Type0_V8_UInt3 { get; set; }
        public uint Type0_V8_UInt4 { get; set; }
        public uint Type0_V8_UInt5 { get; set; }
        public uint Type0_V10_UInt1 { get; set; }
        public uint Type0_V11_UInt1 { get; set; }
        public uint Type0_V10_Count { get; set; }
        public Type0_Struct[] Type0_V10_Structs { get; set; }
        public uint Type0_V12_UInt1 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type0_V16_GameObject { get; set; }

        public Jade_Reference<OBJ_GameObject> Type1_GameObject1 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type1_GameObject2 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type1_GameObject3 { get; set; }
        public uint Type1_UInt1 { get; set; }
        public Jade_Vector Type1_Vector1 { get; set; }
        public Jade_Vector Type1_Vector2 { get; set; }
        public Jade_Vector Type1_Vector3 { get; set; }
        public Jade_Reference<OBJ_GameObject> Type1_GameObject4 { get; set; }
        public Jade_Vector Type1_Vector4 { get; set; }

        public Jade_Vector Type2_Vector { get; set; }
        public float Type2_Float { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
            Version = s.Serialize<uint>(Version, name: nameof(Version));
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Type = s.Serialize<uint>(Type, name: nameof(Type));
            switch (Type) {
                case 0:
					Type0_UInt0 = s.Serialize<uint>(Type0_UInt0, name: nameof(Type0_UInt0));
					Type0_Float1 = s.Serialize<float>(Type0_Float1, name: nameof(Type0_Float1));
					if (Version >= 3) Type0_V3_UInt = s.Serialize<float>(Type0_V3_UInt, name: nameof(Type0_V3_UInt));
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
						Type0_V7_UInt1 = s.Serialize<uint>(Type0_V7_UInt1, name: nameof(Type0_V7_UInt1));
						Type0_V7_UInt2 = s.Serialize<uint>(Type0_V7_UInt2, name: nameof(Type0_V7_UInt2));
					}
                    if (Version >= 8) {
						Type0_V8_UInt1 = s.Serialize<uint>(Type0_V8_UInt1, name: nameof(Type0_V8_UInt1));
						Type0_V8_UInt2 = s.Serialize<uint>(Type0_V8_UInt2, name: nameof(Type0_V8_UInt2));
						Type0_V8_UInt3 = s.Serialize<uint>(Type0_V8_UInt3, name: nameof(Type0_V8_UInt3));
						Type0_V8_UInt4 = s.Serialize<uint>(Type0_V8_UInt4, name: nameof(Type0_V8_UInt4));
						Type0_V8_UInt5 = s.Serialize<uint>(Type0_V8_UInt5, name: nameof(Type0_V8_UInt5));
					}
                    if (Version >= 10) {
                        Type0_V10_UInt1 = s.Serialize<uint>(Type0_V10_UInt1, name: nameof(Type0_V10_UInt1));
                        if (Version >= 11) Type0_V11_UInt1 = s.Serialize<uint>(Type0_V11_UInt1, name: nameof(Type0_V11_UInt1));
						Type0_V10_Count = s.Serialize<uint>(Type0_V10_Count, name: nameof(Type0_V10_Count));
						Type0_V10_Structs = s.SerializeObjectArray<Type0_Struct>(Type0_V10_Structs, Type0_V10_Count,
                            onPreSerialize: str => str.Pre_Modifier = this, name: nameof(Type0_V10_Structs));
					}
					if (Version >= 12) Type0_V12_UInt1 = s.Serialize<uint>(Type0_V12_UInt1, name: nameof(Type0_V12_UInt1));
					if (Version >= 16) Type0_V16_GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type0_V16_GameObject, name: nameof(Type0_V16_GameObject))?.Resolve();
					break;
                case 1:
					Type1_GameObject1 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type1_GameObject1, name: nameof(Type1_GameObject1))?.Resolve();
					Type1_GameObject2 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type1_GameObject2, name: nameof(Type1_GameObject2))?.Resolve();
					Type1_GameObject3 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Type1_GameObject3, name: nameof(Type1_GameObject3))?.Resolve();
					Type1_UInt1 = s.Serialize<uint>(Type1_UInt1, name: nameof(Type1_UInt1));
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

            public uint V15_Uint1 { get; set; }
            public uint V15_Uint2 { get; set; }


            public uint Count => Pre_Modifier.Version >= 14 ? (uint)7 : 3;
            
			public override void SerializeImpl(SerializerObject s) {
				Float_0 = s.Serialize<float>(Float_0, name: nameof(Float_0));
				Floats0 = s.SerializeArray<float>(Floats0, Count, name: nameof(Floats0));
				Floats1 = s.SerializeArray<float>(Floats1, Count, name: nameof(Floats1));
				Byte_1 = s.Serialize<byte>(Byte_1, name: nameof(Byte_1));
				Bytes2 = s.SerializeArray<byte>(Bytes2, Count, name: nameof(Bytes2));
				V13_Byte = s.Serialize<byte>(V13_Byte, name: nameof(V13_Byte));
				if (Pre_Modifier.Version >= 15) {
					V15_Uint1 = s.Serialize<uint>(V15_Uint1, name: nameof(V15_Uint1));
					V15_Uint2 = s.Serialize<uint>(V15_Uint2, name: nameof(V15_Uint2));
				}
			}
		}
	}
}
