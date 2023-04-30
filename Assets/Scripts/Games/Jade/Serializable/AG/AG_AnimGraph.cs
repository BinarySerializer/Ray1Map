using BinarySerializer;
using System;

namespace Ray1Map.Jade
{
	public class AG_AnimGraph : BinarySerializable {
		public uint Version { get; set; }
		public uint NameLength { get; set; } = 12;
		public string Name { get; set; }
		public Jade_Reference<AG_FightMatrix> FightMatrix { get; set; }
		public float TimeUnit { get; set; }
		public float OneOnTimeUnit { get; set; }
		public uint V5_UInt { get; set; }

		public int DynamicEnumsCount { get; set; } = 3;
		public int ClipTypesCount { get; set; } = 26;
		public int ClipRelationsCount { get; set; } = 9;
		public int PropertyRegistersCount { get; set; } = 16;

		public AG_DynamicEnum[] DynamicEnums { get; set; }
		public short[] ClipTypes { get; set; }
		public short[] ClipRelations { get; set; }
		public ClipCtrl[] ClipCtrlLists { get; set; }
		public PropertyRegister[] PropertyRegisters { get; set; }
		public ushort MaxTagClipsCount { get; set; }
		public ushort MaxTagPosesCount { get; set; }
		public int MaxClipEntriesCount { get; set; }
		public ushort AllPoseClipsCount { get; set; }

		public int Unknown_0 { get; set; }
		public int Unknown_1 { get; set; }
		public int Unknown_2 { get; set; }
		public int Unknown_3 { get; set; }
		public int Unknown_4 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 9) NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
			if (!Loader.IsBinaryData) Name = s.SerializeString(Name, length: Version >= 9 ? NameLength : 12, name: nameof(Name));

			FightMatrix = s.SerializeObject<Jade_Reference<AG_FightMatrix>>(FightMatrix, name: nameof(FightMatrix));
			TimeUnit = s.Serialize<float>(TimeUnit, name: nameof(TimeUnit));
			OneOnTimeUnit = s.Serialize<float>(OneOnTimeUnit, name: nameof(OneOnTimeUnit));
			if (Version >= 5) V5_UInt = s.Serialize<uint>(V5_UInt, name: nameof(V5_UInt));
			if (Version >= 4) {
				DynamicEnumsCount = s.Serialize<int>(DynamicEnumsCount, name: nameof(DynamicEnumsCount));
				ClipTypesCount = s.Serialize<int>(ClipTypesCount, name: nameof(ClipTypesCount));
				ClipRelationsCount = s.Serialize<int>(ClipRelationsCount, name: nameof(ClipRelationsCount));
				PropertyRegistersCount = s.Serialize<int>(PropertyRegistersCount, name: nameof(PropertyRegistersCount));
			}
			DynamicEnums = s.SerializeObjectArray<AG_DynamicEnum>(DynamicEnums, DynamicEnumsCount, name: nameof(DynamicEnums));
			ClipTypes = s.SerializeArray<short>(ClipTypes, ClipTypesCount, name: nameof(ClipTypes));
			ClipRelations = s.SerializeArray<short>(ClipRelations, ClipRelationsCount, name: nameof(ClipRelations));
			if (Version < 4) ClipCtrlLists = s.SerializeObjectArray<ClipCtrl>(ClipCtrlLists, 2, name: nameof(ClipCtrlLists));
			PropertyRegisters = s.SerializeObjectArray<PropertyRegister>(PropertyRegisters, PropertyRegistersCount, name: nameof(PropertyRegisters));
			MaxTagClipsCount = s.Serialize<ushort>(MaxTagClipsCount, name: nameof(MaxTagClipsCount));
			MaxTagPosesCount = s.Serialize<ushort>(MaxTagPosesCount, name: nameof(MaxTagPosesCount));
			MaxClipEntriesCount = s.Serialize<int>(MaxClipEntriesCount, name: nameof(MaxClipEntriesCount));
			AllPoseClipsCount = s.Serialize<ushort>(AllPoseClipsCount, name: nameof(AllPoseClipsCount));
			if (Version < 14) Unknown_0 = s.Serialize<int>(Unknown_0, name: nameof(Unknown_0));
			Unknown_1 = s.Serialize<int>(Unknown_1, name: nameof(Unknown_1));
			Unknown_2 = s.Serialize<int>(Unknown_2, name: nameof(Unknown_2));
			Unknown_3 = s.Serialize<int>(Unknown_3, name: nameof(Unknown_3));
			Unknown_4 = s.Serialize<int>(Unknown_4, name: nameof(Unknown_4));
			//throw new NotImplementedException();
			s.SystemLogger?.LogWarning($"TODO: Serialize rest of AnimGraph!");
		}

		public class ClipCtrl : BinarySerializable {
			public uint Count { get; set; }
			public byte[] CtrlList { get; set; } // 10 arrays of 16 chars

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				CtrlList = s.SerializeArray<byte>(CtrlList, 10 * 16, name: nameof(CtrlList));
			}
		}
		public class PropertyRegister : BinarySerializable {
			public uint Count { get; set; }
			public uint[] PropertyCRC32 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				PropertyCRC32 = s.SerializeArray<uint>(PropertyCRC32, 64, name: nameof(PropertyCRC32));
			}
		}

		public class Pose : BinarySerializable {
			public override void SerializeImpl(SerializerObject s) {
				throw new NotImplementedException();
			}
		}
	}
}