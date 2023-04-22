using BinarySerializer;
using System;

namespace Ray1Map.Jade
{
    public class AG_FightMatrix : Jade_File {
		//public override string Export_Extension => "aig"; // Unknown
		public override bool HasHeaderBFFile => true;

		public uint Version { get; set; }
		public uint FightMatrixCount { get; set; }
		public int DefaultMatrixId { get; set; }
		public int DefaultArialMatrixId { get; set; }
		public uint RangeCount { get; set; }
		public uint[] Range { get; set; }
		public FightMatrix[] Matrices { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			FightMatrixCount = s.Serialize<uint>(FightMatrixCount, name: nameof(FightMatrixCount));
			DefaultMatrixId = s.Serialize<int>(DefaultMatrixId, name: nameof(DefaultMatrixId));
			if (Version >= 3) DefaultArialMatrixId = s.Serialize<int>(DefaultArialMatrixId, name: nameof(DefaultArialMatrixId));
			RangeCount = s.Serialize<uint>(RangeCount, name: nameof(RangeCount));
			Range = s.SerializeArray<uint>(Range, RangeCount, name: nameof(Range));
			Matrices = s.SerializeObjectArray<FightMatrix>(Matrices, FightMatrixCount, onPreSerialize: fm => fm.Pre_Header = this, name: nameof(Matrices));
		}

		public class FightMatrix : BinarySerializable {
			public AG_FightMatrix Pre_Header { get; set; }

			public string Name { get; set; }
			public Dir Direction { get; set; }
			public int LookingSide { get; set; }
			public float Damage { get; set; }

			public int VulneX { get; set; }
			public int VulneY { get; set; }
			public int OffenceX { get; set; }
			public int OffenceY { get; set; }

			public uint OffensiveBitfield { get; set; }
			public int OffensiveMatrix { get; set; }
			public int ReceivingMatrix { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Name = s.SerializeString(Name, length: 8, name: nameof(Name));
				Direction = s.Serialize<Dir>(Direction, name: nameof(Direction));
				LookingSide = s.Serialize<int>(LookingSide, name: nameof(LookingSide));
				if (Pre_Header.Version >= 1) Damage = s.Serialize<float>(Damage, name: nameof(Damage));
				VulneX = s.Serialize<int>(VulneX, name: nameof(VulneX));
				VulneY = s.Serialize<int>(VulneY, name: nameof(VulneY));

				if (Pre_Header.Version >= 3) {
					OffenceX = s.Serialize<int>(OffenceX, name: nameof(OffenceX));
					OffenceY = s.Serialize<int>(OffenceY, name: nameof(OffenceY));
				}
				OffensiveBitfield = s.Serialize<uint>(OffensiveBitfield, name: nameof(OffensiveBitfield));
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRTVParty)) {
					OffensiveMatrix = s.Serialize<byte>((byte)OffensiveMatrix, name: nameof(OffensiveMatrix));
					ReceivingMatrix = s.Serialize<byte>((byte)ReceivingMatrix, name: nameof(ReceivingMatrix));
				} else {
					OffensiveMatrix = s.Serialize<int>(OffensiveMatrix, name: nameof(OffensiveMatrix));
					ReceivingMatrix = s.Serialize<int>(ReceivingMatrix, name: nameof(ReceivingMatrix));
				}
			}

			[Flags]
			public enum Dir : uint {
				None  = 0,
				Front = 1 << 0,
				Back  = 1 << 1,
				Left  = 1 << 2,
				Right = 1 << 3,
				All = Right | Left | Back | Front
			}
		}
	}
}