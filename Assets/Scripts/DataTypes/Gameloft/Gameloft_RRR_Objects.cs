namespace R1Engine
{
	public class Gameloft_RRR_Objects : Gameloft_Resource {
		public Object[] Objects { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Objects = s.SerializeObjectArrayUntil<Object>(Objects,
				(o) => (s.CurrentPointer.AbsoluteOffset) >= (Offset + ResourceSize).AbsoluteOffset,
				includeLastObj: true, name: nameof(Objects));
		}

		public class Object : R1Serializable {
			public byte Length { get; set; }

			public short Type { get; set; }
			public short XPosition { get; set; }
			public short YPosition { get; set; }
			public short Unknown { get; set; }
			public short AnimationIndex { get; set; }
			public short Flags { get; set; }

			public short[] Shorts { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Length = s.Serialize<byte>(Length, name: nameof(Length));

				Type = s.Serialize<short>(Type, name: nameof(Type));
				XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
				YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
				Unknown = s.Serialize<short>(Unknown, name: nameof(Unknown));
				AnimationIndex = s.Serialize<short>(AnimationIndex, name: nameof(AnimationIndex));
				Flags = s.Serialize<short>(Flags, name: nameof(Flags));

				Shorts = s.SerializeArray<short>(Shorts, Length - 6, name: nameof(Shorts));
			}
		}
	}
}