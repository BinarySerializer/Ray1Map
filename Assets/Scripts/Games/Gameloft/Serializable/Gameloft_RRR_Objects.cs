﻿using BinarySerializer;

namespace Ray1Map.Gameloft
{
	public class Gameloft_RRR_Objects : Gameloft_Resource {
		public Object[] Objects { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Objects = s.SerializeObjectArrayUntil<Object>(Objects,
				(o) => (s.CurrentPointer.AbsoluteOffset) >= (Offset + ResourceSize).AbsoluteOffset, name: nameof(Objects));
		}

		public class Object : BinarySerializable {
			public byte Length { get; set; }

			public short Type { get; set; }
			public short XPosition { get; set; }
			public short YPosition { get; set; }
			public short ObjectID { get; set; }
			public short AnimationIndex { get; set; }
			public short Flags { get; set; }

			public short[] Shorts { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Length = s.Serialize<byte>(Length, name: nameof(Length));

				Type = s.Serialize<short>(Type, name: nameof(Type));
				XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
				YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
				ObjectID = s.Serialize<short>(ObjectID, name: nameof(ObjectID));
				AnimationIndex = s.Serialize<short>(AnimationIndex, name: nameof(AnimationIndex));
				Flags = s.Serialize<short>(Flags, name: nameof(Flags));

				Shorts = s.SerializeArray<short>(Shorts, Length - 6, name: nameof(Shorts));
			}
		}
	}
}