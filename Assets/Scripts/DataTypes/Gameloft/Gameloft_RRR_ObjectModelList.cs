namespace R1Engine
{
	public class Gameloft_RRR_ObjectModelList : Gameloft_Resource {
		public ObjectModel[] Models { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Models = s.SerializeObjectArrayUntil<ObjectModel>(Models,
				(o) => (s.CurrentPointer.AbsoluteOffset) >= (Offset + ResourceSize).AbsoluteOffset,
				includeLastObj: true, name: nameof(Models));
		}

		public class ObjectModel : R1Serializable {
			public byte ObjectID { get; set; }
			public byte Byte1 { get; set; }
			public byte Byte2 { get; set; }
			public sbyte ResourceReferenceID { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				ObjectID = s.Serialize<byte>(ObjectID, name: nameof(ObjectID));
				Byte1 = s.Serialize<byte>(Byte1, name: nameof(Byte1));
				Byte2 = s.Serialize<byte>(Byte2, name: nameof(Byte2));
				ResourceReferenceID = s.Serialize<sbyte>(ResourceReferenceID, name: nameof(ResourceReferenceID));
			}
		}
	}
}