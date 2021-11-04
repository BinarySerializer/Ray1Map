using System;
using System.IO;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GEO_GaoVisu_PS2 : BinarySerializable {
		public uint ElementsCount { get; set; }
		public Element[] Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			Elements = s.SerializeObjectArray<Element>(Elements, ElementsCount, name: nameof(Elements));
		}

		public MemoryStream CreateStream(GEO_GeoObject_PS2 obj, int elementIndex) {
			MemoryStream ms = new MemoryStream();
			if (Elements.Length > 0) {
				//var commands = Elements[elementIndex].
				/*if (Commands.Length > 0) {
					using (StreamWriter w = new StreamWriter(ms)) {
						foreach (var c in Commands) {
							switch (c.DMATag.ID) {
								case BinarySerializer.PS2.Chain_DMAtag.TagID.REF:
									break;
							}
						}
					}
				}*/
			} else {
			}
			return ms;
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
			public PS2_DMAChainProgram[] DMAChainPrograms { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				InstanceVIFProgramsCount = s.Serialize<uint>(InstanceVIFProgramsCount, name: nameof(InstanceVIFProgramsCount));
				InstanceVIFPrograms = s.SerializeObjectArray<InstanceVIFProgram>(InstanceVIFPrograms, InstanceVIFProgramsCount, name: nameof(InstanceVIFPrograms));
				if (s.GetR1Settings().Platform == Platform.PS2) {
					DMAChainProgramsCount = s.Serialize<uint>(DMAChainProgramsCount, name: nameof(DMAChainProgramsCount));
					DMAChainPrograms = s.SerializeObjectArray<PS2_DMAChainProgram>(DMAChainPrograms, DMAChainProgramsCount, name: nameof(DMAChainPrograms));
				}
			}

			public class InstanceVIFProgram : BinarySerializable {
				public int ID { get; set; } // Used as address (ADDR = ID << 24)
				public uint DataSize { get; set; }
				public byte[] Bytes { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					ID = s.Serialize<int>(ID, name: nameof(ID));
					DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
					Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
				}
			}
		}
	}
}
