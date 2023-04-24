using System;
using BinarySerializer;

namespace Ray1Map.Jade {
    public class GRO_Struct : BinarySerializable {
        public GEO_Object Object { get; set; }

        public GRO_Type Type { get; set; }
        public uint ObjectVersion { get; set; }
        public uint Count_Editor { get; set; }
        public byte[] Bytes_Editor { get; set; }

        public GRO_GraphicRenderObject Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Type = s.Serialize<GRO_Type>(Type, name: nameof(Type));
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                ObjectVersion = s.Serialize<uint>(ObjectVersion, name: nameof(ObjectVersion));
            } else {
                ObjectVersion = 0;
            }
            if (!Loader.IsBinaryData && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
                Count_Editor = s.Serialize<uint>(Count_Editor, name: nameof(Count_Editor));
                Bytes_Editor = s.SerializeArray<byte>(Bytes_Editor, Count_Editor, name: nameof(Bytes_Editor));
            }

            T SerializeStruct<T>() where T : GRO_GraphicRenderObject, new()
                => s.SerializeObject<T>((T)Value, onPreSerialize: o => o.GRO = this, name: nameof(Value));


			Value = Type switch
            {
                GRO_Type.None => null,
                GRO_Type.Unknown => null,
                GRO_Type.GEO => SerializeStruct<GEO_GeometricObject>(),
				GRO_Type.GEO_StaticLOD => SerializeStruct<GEO_StaticLOD>(),
				GRO_Type.GEO_SubGeometry => SerializeStruct<GEO_SubGeometry>(),
				GRO_Type.MAT_SIN => SerializeStruct<MAT_SIN_SingleMaterial>(),
				GRO_Type.MAT_MSM => SerializeStruct<MAT_MSM_MultiSingleMaterial>(),
				GRO_Type.MAT_MTT => SerializeStruct<MAT_MTT_MultiTextureMaterial>(),
				GRO_Type.STR => SerializeStruct<STR_StringRenderObject>(),
				GRO_Type.LIGHT => SerializeStruct<LIGHT_Light>(),
				GRO_Type.CAM => SerializeStruct<CAM_Camera>(),
				GRO_Type.PAG => SerializeStruct<PAG_ParticleGeneratorObject>(),
				_ => throw new NotImplementedException($"TODO: Implement GRO Struct Type {Type}")
            };
        }
	}
}
