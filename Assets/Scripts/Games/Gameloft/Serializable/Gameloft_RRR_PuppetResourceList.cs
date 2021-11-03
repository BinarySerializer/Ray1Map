﻿using BinarySerializer;

namespace Ray1Map.Gameloft
{
	public class Gameloft_RRR_PuppetResourceList : Gameloft_Resource {
		public ResourceReference[] ResourceList { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ResourceList = s.SerializeObjectArrayUntil<ResourceReference>(ResourceList,
				(o) => (s.CurrentPointer.AbsoluteOffset) >= (Offset + ResourceSize).AbsoluteOffset, name: nameof(ResourceList));
		}

		public class ResourceReference : BinarySerializable {
			public sbyte FileID { get; set; }
			public byte ResourceID { get; set; }
			public byte Byte2 { get; set; }
			public sbyte Byte3 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				FileID = s.Serialize<sbyte>(FileID, name: nameof(FileID));
				ResourceID = s.Serialize<byte>(ResourceID, name: nameof(ResourceID));
				Byte2 = s.Serialize<byte>(Byte2, name: nameof(Byte2));
				Byte3 = s.Serialize<sbyte>(Byte3, name: nameof(Byte3));
			}
		}
	}
}