using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_LoadGCGeoObject
	public class GEO_GeoObject_GC : BinarySerializable {
		public GEO_GeometricObject GeometricObject { get; set; }

		public uint ElementsCount { get; set; }
		public uint[] Elements_MaterialId { get; set; }

		public Jade_Reference<GEO_GeoObject_GC_Content> Content { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if (Loader.IsBinaryData) {
				ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
				Elements_MaterialId = s.SerializeArray<uint>(Elements_MaterialId, ElementsCount, name: nameof(Elements_MaterialId));
			} else {
				ElementsCount = GeometricObject.ElementsCount;
			}

			if(s.CurrentAbsoluteOffset >= (GeometricObject.GRO.Object.Offset + GeometricObject.GRO.Object.FileSize).AbsoluteOffset) return;

			if(Content == null)
				Content = new Jade_Reference<GEO_GeoObject_GC_Content>(Context, GeometricObject.OptimizedGeoObjectKey_GC ?? new Jade_Key(Context, 0xFFFFFFFF));


			if (Loader.IsBinaryData) {
				Content.ForceResolve = true;
				Content.ResolveEmbedded(s, onPreSerialize: (_,c) => c.Header = this, flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile | LOA_Loader.ReferenceFlags.MustExist, unknownFileSize: true);
			} else {
				Content.Resolve((_,c)=> c.Header = this);
			}

		}
	}
}
