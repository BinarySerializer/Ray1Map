namespace R1Engine
{
	public class Gameloft_Objects : Gameloft_Resource {
		public Object[] Objects { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Objects = s.SerializeObjectArrayUntil<Object>(Objects,
				(o) => (s.CurrentPointer.AbsoluteOffset) >= (Offset + ResourceSize).AbsoluteOffset,
				includeLastObj: true, name: nameof(Objects));
		}

		public class Object : R1Serializable {
			public byte Length { get; set; }
			public short[] Shorts { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Length = s.Serialize<byte>(Length, name: nameof(Length));
				Shorts = s.SerializeArray<short>(Shorts, Length, name: nameof(Shorts));
			}
		}
	}
}