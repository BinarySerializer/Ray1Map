using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GEO_GaoVisu_PS2 : BinarySerializable {
		public uint ElementsCount { get; set; }
		public Element[] Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			Elements = s.SerializeObjectArray<Element>(Elements, ElementsCount, name: nameof(Elements));
		}

		public class Element : BinarySerializable {
			public Jade_Code Foceface { get; set; }
			public uint Count { get; set; }
			public MeshElementRef[] Refs { get; set; }
			public Jade_Code Deadbeef { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Foceface = s.Serialize<Jade_Code>(Foceface, name: nameof(Foceface));
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				Refs = s.SerializeObjectArray<MeshElementRef>(Refs, Count, name: nameof(Refs));
				Deadbeef = s.Serialize<Jade_Code>(Deadbeef, name: nameof(Deadbeef));
			}
		}

		public class MeshElementRef : BinarySerializable {
			public uint InstanceVIFProgramsCount { get; set; }
			public InstanceVIFProgram[] InstanceVIFPrograms { get; set; }

			public uint DMAChainProgramsCount { get; set; }
			public DMAChainProgram[] DMAChainPrograms { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				InstanceVIFProgramsCount = s.Serialize<uint>(InstanceVIFProgramsCount, name: nameof(InstanceVIFProgramsCount));
				InstanceVIFPrograms = s.SerializeObjectArray<InstanceVIFProgram>(InstanceVIFPrograms, InstanceVIFProgramsCount, name: nameof(InstanceVIFPrograms));
				if (s.GetR1Settings().Platform == Platform.PS2) {
					DMAChainProgramsCount = s.Serialize<uint>(DMAChainProgramsCount, name: nameof(DMAChainProgramsCount));
					DMAChainPrograms = s.SerializeObjectArray<DMAChainProgram>(DMAChainPrograms, DMAChainProgramsCount, name: nameof(DMAChainPrograms));
				}
			}

			public class InstanceVIFProgram : BinarySerializable {
				public uint ID { get; set; } // Used as address (ADDR = ID << 24)
				public uint DataSize { get; set; }
				public byte[] Bytes { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					ID = s.Serialize<uint>(ID, name: nameof(ID));
					DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
					Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
				}
			}

			public class DMAChainProgram : BinarySerializable {
				public uint ID { get; set; } // Used as address (ADDR = ID << 24)
				public uint DataSize { get; set; }
				public byte[] Bytes { get; set; }
				public PS2_DMACommand[] Commands { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					ID = s.Serialize<uint>(ID, name: nameof(ID));
					DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
					//Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));

					Commands = s.SerializeObjectArrayUntil<PS2_DMACommand>(Commands, gc => s.CurrentAbsoluteOffset >= Offset.AbsoluteOffset + 8 + DataSize, name: nameof(Commands));
					if (s.CurrentAbsoluteOffset > Offset.AbsoluteOffset + 8 + DataSize) {
						s.LogWarning($"{Offset}: Read too many DMA commands");
					}
					s.Goto(Offset + 8 + DataSize);
				}
			}
		}
	}
}
