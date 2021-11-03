using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_p_StaticLOD_CreateFromBuffer
	public class GEO_StaticLOD : GRO_GraphicRenderObject {
		public byte LODCount { get; set; }
		public byte Byte_01_Editor { get; set; }
		public byte[] EndDistance { get; set; }
		public Jade_Reference<GEO_Object>[] LODLevels { get; set; }

		public const byte MaxLevels = 6;

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			LODCount = s.Serialize<byte>(LODCount, name: nameof(LODCount));
			if(!Loader.IsBinaryData) Byte_01_Editor = s.Serialize<byte>(Byte_01_Editor, name: nameof(Byte_01_Editor));
			EndDistance = s.SerializeArray<byte>(EndDistance, MaxLevels, name: nameof(EndDistance));
			LODLevels = s.SerializeObjectArray<Jade_Reference<GEO_Object>>(LODLevels, Math.Min(LODCount, MaxLevels), name: nameof(LODLevels))?.Resolve();
		}
	}
}
