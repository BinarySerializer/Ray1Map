using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_p_CreateFromBuffer
	public class GEO_GeometricObject_MRM_ObjectAdditionalInfo : BinarySerializable {
		public GEO_GeometricObject GeometricObject { get; set; }

		public short[] Absorbers { get; set; }
		public short[] ReorderBuffer { get; set; }
		public uint MinimumNumberOfPoints { get; set; }
		public float[] MRMQualityCurve { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Absorbers = s.SerializeArray<short>(Absorbers, GeometricObject.VerticesCount, name: nameof(Absorbers));
			if(GeometricObject.MRM_ObjectAdditionalInfoPointer == (uint)Jade_Code.Code0008)
				ReorderBuffer = s.SerializeArray<short>(ReorderBuffer, GeometricObject.VerticesCount, name: nameof(ReorderBuffer));
			MinimumNumberOfPoints = s.Serialize<uint>(MinimumNumberOfPoints, name: nameof(MinimumNumberOfPoints));
			if (GeometricObject.Version == 0)
				MRMQualityCurve = s.SerializeArray<float>(MRMQualityCurve, 8, name: nameof(MRMQualityCurve));
		}
	}
}
