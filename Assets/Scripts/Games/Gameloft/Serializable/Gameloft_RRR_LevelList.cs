﻿using BinarySerializer;

namespace Ray1Map.Gameloft
{
	public class Gameloft_RRR_LevelList : Gameloft_Resource {
		public Level[] Levels { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Levels = s.SerializeObjectArrayUntil<Level>(Levels,
				(o) => (s.CurrentPointer.AbsoluteOffset) >= (Offset + ResourceSize).AbsoluteOffset, name: nameof(Levels));
		}

		public class Level : BinarySerializable {
			public byte World { get; set; }
			public byte[] Types { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				World = s.Serialize<byte>(World, name: nameof(World));
				Types = s.SerializeArraySize<byte, byte>(Types, name: nameof(Types));
				Types = s.SerializeArray<byte>(Types, Types.Length, name: nameof(Types));
			}
		}
	}
}