using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GAO_ModifierSoftBody : MDF_Modifier {
        public uint UInt_00_Editor { get; set; }
        public uint Version { get; set; }
        public uint V3_UInt { get; set; }
        public uint UInt_00 { get; set; }
        public uint UInt_01 { get; set; }
        public uint VerticesCount { get; set; }
        public uint LengthConstraintsCount { get; set; }
        public uint PlaneConstraintsCount { get; set; }
        public Jade_Vector Vector_05 { get; set; }
        public Jade_Vector V10_Vector_06 { get; set; }
        public float Float_06 { get; set; }
        public Jade_Vector Vector_07 { get; set; }
        public Jade_Vector Vector_08 { get; set; }
        public float Float_09 { get; set; }
        public float Float_10 { get; set; }
        public float Float_11 { get; set; }
        public float Float_12 { get; set; }
        public float V2_Float_13 { get; set; }
        public float V2_Float_14 { get; set; }
        public float V5_Float_15 { get; set; }
        public float V5_Float_16 { get; set; }
        public float V5_Float_17 { get; set; }
        public float V5_Float_18 { get; set; }
        public float V5_Float_19 { get; set; }
        public float V5_Float_20 { get; set; }
        public uint V6_UInt_21 { get; set; }
        public Jade_Reference<OBJ_GameObject> V6_GameObject_22 { get; set; }
        public float V6_Float_23 { get; set; }
        public float V6_Float_24 { get; set; }
        public uint V7_UInt_25_Count { get; set; }
        public float V8_Float_26 { get; set; }
        public float V8_Float_27 { get; set; }
        public float V9_Float_28 { get; set; }
        public SoftBodyVertex[] Vertices { get; set; }
        public SoftBodyLengthConstraint[] LengthConstraints { get; set; }
        public SoftBodyPlaneConstraint[] PlaneConstraints { get; set; }
        public uint[] V7_UInts { get; set; }


        public override void SerializeImpl(SerializerObject s) 
        {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            if (!Loader.IsBinaryData) UInt_00_Editor = s.Serialize<uint>(UInt_00_Editor, name: nameof(UInt_00_Editor));
            Version = s.Serialize<uint>(Version, name: nameof(Version));
            if (Version >= 3) V3_UInt = s.Serialize<uint>(V3_UInt, name: nameof(V3_UInt));
            if (Version >= 1) {
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
				UInt_01 = s.Serialize<uint>(UInt_01, name: nameof(UInt_01));
				VerticesCount = s.Serialize<uint>(VerticesCount, name: nameof(VerticesCount));
				LengthConstraintsCount = s.Serialize<uint>(LengthConstraintsCount, name: nameof(LengthConstraintsCount));
				PlaneConstraintsCount = s.Serialize<uint>(PlaneConstraintsCount, name: nameof(PlaneConstraintsCount));
				Vector_05 = s.SerializeObject<Jade_Vector>(Vector_05, name: nameof(Vector_05));
				if (Version >= 10) V10_Vector_06 = s.SerializeObject<Jade_Vector>(V10_Vector_06, name: nameof(V10_Vector_06));
				Float_06 = s.Serialize<float>(Float_06, name: nameof(Float_06));
				Vector_07 = s.SerializeObject<Jade_Vector>(Vector_07, name: nameof(Vector_07));
				Vector_08 = s.SerializeObject<Jade_Vector>(Vector_08, name: nameof(Vector_08));
				Float_09 = s.Serialize<float>(Float_09, name: nameof(Float_09));
				Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
				Float_11 = s.Serialize<float>(Float_11, name: nameof(Float_11));
				Float_12 = s.Serialize<float>(Float_12, name: nameof(Float_12));
                if (Version >= 2) {
					V2_Float_13 = s.Serialize<float>(V2_Float_13, name: nameof(V2_Float_13));
					V2_Float_14 = s.Serialize<float>(V2_Float_14, name: nameof(V2_Float_14));
				}
                if (Version >= 5) {
					V5_Float_15 = s.Serialize<float>(V5_Float_15, name: nameof(V5_Float_15));
                    V5_Float_16 = s.Serialize<float>(V5_Float_16, name: nameof(V5_Float_16));
                    V5_Float_17 = s.Serialize<float>(V5_Float_17, name: nameof(V5_Float_17));
                    V5_Float_18 = s.Serialize<float>(V5_Float_18, name: nameof(V5_Float_18));
                    V5_Float_19 = s.Serialize<float>(V5_Float_19, name: nameof(V5_Float_19));
                    V5_Float_20 = s.Serialize<float>(V5_Float_20, name: nameof(V5_Float_20));
                }
                if (Version >= 6) {
					V6_UInt_21 = s.Serialize<uint>(V6_UInt_21, name: nameof(V6_UInt_21));
					V6_GameObject_22 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(V6_GameObject_22, name: nameof(V6_GameObject_22))?.Resolve();
					V6_Float_23 = s.Serialize<float>(V6_Float_23, name: nameof(V6_Float_23));
					V6_Float_24 = s.Serialize<float>(V6_Float_24, name: nameof(V6_Float_24));
				}
				if (Version >= 7) V7_UInt_25_Count = s.Serialize<uint>(V7_UInt_25_Count, name: nameof(V7_UInt_25_Count));
                if (Version >= 8) {
					V8_Float_26 = s.Serialize<float>(V8_Float_26, name: nameof(V8_Float_26));
					V8_Float_27 = s.Serialize<float>(V8_Float_27, name: nameof(V8_Float_27));
				}
				if (Version >= 9) V9_Float_28 = s.Serialize<float>(V9_Float_28, name: nameof(V9_Float_28));
				Vertices = s.SerializeObjectArray<SoftBodyVertex>(Vertices, VerticesCount, onPreSerialize: st => st.Modifier = this, name: nameof(Vertices));
				LengthConstraints = s.SerializeObjectArray<SoftBodyLengthConstraint>(LengthConstraints, LengthConstraintsCount, onPreSerialize: st => st.Modifier = this, name: nameof(LengthConstraints));
				PlaneConstraints = s.SerializeObjectArray<SoftBodyPlaneConstraint>(PlaneConstraints, PlaneConstraintsCount, onPreSerialize: st => st.Modifier = this, name: nameof(PlaneConstraints));
				V7_UInts = s.SerializeArray<uint>(V7_UInts, V7_UInt_25_Count, name: nameof(V7_UInts));
			}
        }


		public class SoftBodyVertex : BinarySerializable {
            public GAO_ModifierSoftBody Modifier { get; set; }
            public uint Flags { get; set; }
            public uint UInt_1 { get; set; }
            public float Float_2 { get; set; }
            public float Float_3 { get; set; }
            public uint V4_UInt { get; set; }

            public override void SerializeImpl(SerializerObject s) {
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				UInt_1 = s.Serialize<uint>(UInt_1, name: nameof(UInt_1));
				Float_2 = s.Serialize<float>(Float_2, name: nameof(Float_2));
				Float_3 = s.Serialize<float>(Float_3, name: nameof(Float_3));
				if (Modifier.Version >= 4 && (Flags & 2) != 0) V4_UInt = s.Serialize<uint>(V4_UInt, name: nameof(V4_UInt));
			}
        }

        public class SoftBodyLengthConstraint : BinarySerializable {
            public GAO_ModifierSoftBody Modifier { get; set; }
            public uint UInt_0 { get; set; }
            public uint UInt_1 { get; set; }
            public float Float_2 { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                UInt_0 = s.Serialize<uint>(UInt_0, name: nameof(UInt_0));
                UInt_1 = s.Serialize<uint>(UInt_1, name: nameof(UInt_1));
                Float_2 = s.Serialize<float>(Float_2, name: nameof(Float_2));
            }
        }

        public class SoftBodyPlaneConstraint : BinarySerializable {
            public GAO_ModifierSoftBody Modifier { get; set; }
            public float Float_0 { get; set; }
            public float Float_1 { get; set; }
            public float Float_2 { get; set; }
            public float Float_3 { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                Float_0 = s.Serialize<float>(Float_0, name: nameof(Float_0));
                Float_1 = s.Serialize<float>(Float_1, name: nameof(Float_1));
                Float_2 = s.Serialize<float>(Float_2, name: nameof(Float_2));
                Float_3 = s.Serialize<float>(Float_3, name: nameof(Float_3));
            }
        }
    }
}
