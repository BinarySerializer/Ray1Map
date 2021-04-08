using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class GRO_Struct : BinarySerializable {
        public GRO_Type Type { get; set; }
        public uint Count_Editor { get; set; }
        public byte[] Bytes_Editor { get; set; }

        public GRO_GraphicRenderObject Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Type = s.Serialize<GRO_Type>(Type, name: nameof(Type));
            if (!Loader.IsBinaryData) {
                Count_Editor = s.Serialize<uint>(Count_Editor, name: nameof(Count_Editor));
                Bytes_Editor = s.SerializeArray<byte>(Bytes_Editor, Count_Editor, name: nameof(Bytes_Editor));
            }

            Value = Type switch
            {
                GRO_Type.None => null,
                GRO_Type.GEO => Value = s.SerializeObject<GEO_GeometricObject>((GEO_GeometricObject)Value, name: nameof(Value)),
                GRO_Type.GEO_StaticLOD => Value = s.SerializeObject<GEO_StaticLOD>((GEO_StaticLOD)Value, name: nameof(Value)),
                GRO_Type.MAT_SIN => Value = s.SerializeObject<MAT_SIN_SingleMaterial>((MAT_SIN_SingleMaterial)Value, name: nameof(Value)),
                GRO_Type.MAT_MSM => Value = s.SerializeObject<MAT_MSM_MultiSingleMaterial>((MAT_MSM_MultiSingleMaterial)Value, name: nameof(Value)),
                GRO_Type.MAT_MTT => Value = s.SerializeObject<MAT_MTT_MultiTextureMaterial>((MAT_MTT_MultiTextureMaterial)Value, name: nameof(Value)),
                GRO_Type.STR => Value = s.SerializeObject<STR_StringRenderObject>((STR_StringRenderObject)Value, name: nameof(Value)),
                GRO_Type.LIGHT => Value = s.SerializeObject<LIGHT_Light>((LIGHT_Light)Value, name: nameof(Value)),
                GRO_Type.CAM => Value = s.SerializeObject<CAM_Camera>((CAM_Camera)Value, name: nameof(Value)),
                GRO_Type.PAG => Value = s.SerializeObject<PAG_ParticleGeneratorObject>((PAG_ParticleGeneratorObject)Value, name: nameof(Value)),
                _ => throw new NotImplementedException($"TODO: Implement GRO Struct Type {Type}")
            };
        }
	}
}
