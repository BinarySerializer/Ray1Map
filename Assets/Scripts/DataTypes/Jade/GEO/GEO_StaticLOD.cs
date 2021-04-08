using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_p_StaticLOD_CreateFromBuffer
	public class GEO_StaticLOD : GRO_GraphicRenderObject {
		public byte LODLevelsCount { get; set; }
		public byte Byte_01_Editor { get; set; }
		public byte[] LODLevelsBytes { get; set; }
		public Jade_Reference<GEO_Object>[] LODLevels { get; set; }

		public const byte MaxLevels = 6;

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			LODLevelsCount = s.Serialize<byte>(LODLevelsCount, name: nameof(LODLevelsCount));
			if(!Loader.IsBinaryData) Byte_01_Editor = s.Serialize<byte>(Byte_01_Editor, name: nameof(Byte_01_Editor));
			LODLevelsBytes = s.SerializeArray<byte>(LODLevelsBytes, MaxLevels, name: nameof(LODLevelsBytes));
			LODLevels = s.SerializeObjectArray<Jade_Reference<GEO_Object>>(LODLevels, Math.Min(LODLevelsCount, MaxLevels), name: nameof(LODLevels))?.Resolve();
		}
	}
}
