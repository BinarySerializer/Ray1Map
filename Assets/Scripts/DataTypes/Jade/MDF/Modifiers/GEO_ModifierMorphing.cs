using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GEO_ModifierMorphing : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_08 { get; set; }
		public uint VertexMorphsCount { get; set; }
		public uint UnknownMorphsCount { get; set; }
		public VertexMorph[] VertexMorphs { get; set; }
		public UnknownMorph[] UnknownMorphs { get; set; }

		public uint Xenon_Type { get; set; } = 1;
		public uint Xenon_Count { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			if (s.GetR1Settings().Jade_Version == Jade_Version.Xenon && UInt_00 == 0) {
				Xenon_Type = s.Serialize<uint>(Xenon_Type, name: nameof(Xenon_Type));
			}
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			VertexMorphsCount = s.Serialize<uint>(VertexMorphsCount, name: nameof(VertexMorphsCount));
			UnknownMorphsCount = s.Serialize<uint>(UnknownMorphsCount, name: nameof(UnknownMorphsCount));
			if (Xenon_Type == 2) Xenon_Count = s.Serialize<uint>(Xenon_Count, name: nameof(Xenon_Count));
			VertexMorphs = s.SerializeObjectArray<VertexMorph>(VertexMorphs, VertexMorphsCount, v => {
				v.Xenon_Type = Xenon_Type;
				v.Xenon_Count = Xenon_Count;
			}, name: nameof(VertexMorphs));
			UnknownMorphs = s.SerializeObjectArray<UnknownMorph>(UnknownMorphs, UnknownMorphsCount, name: nameof(UnknownMorphs));
		}

		public class VertexMorph : BinarySerializable {
			public uint Xenon_Type { get; set; }
			public uint Xenon_Count { get; set; } // Set in onPreSerialize

			public uint Count { get; set; }
			public string Name { get; set; }
			public uint[] Indices { get; set; }
			public Jade_Vector[] Values { get; set; }

			public XenonData[] XenonDatas { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				if(!Loader.IsBinaryData) Name = s.SerializeString(Name, 64, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				Indices = s.SerializeArray<uint>(Indices, Count, name: nameof(Indices));
				Values = s.SerializeObjectArray<Jade_Vector>(Values, Count, name: nameof(Values));
				if (Xenon_Type == 2) {
					XenonDatas = s.SerializeObjectArray<XenonData>(XenonDatas, Xenon_Count, name: nameof(XenonDatas));
				} else if (Xenon_Type >= 3) {
					XenonDatas = s.SerializeObjectArray<XenonData>(XenonDatas, 1, name: nameof(XenonDatas));
				}
			}
			public class XenonData : BinarySerializable {
				public uint Count { get; set; }
				public Jade_Quaternion[] Quaternions { get; set; }
				public uint[] UInts { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					Count = s.Serialize<uint>(Count, name: nameof(Count));
					Quaternions = s.SerializeObjectArray<Jade_Quaternion>(Quaternions, Count, name: nameof(Quaternions));
					UInts = s.SerializeArray<uint>(UInts, Count, name: nameof(UInts));
				}
			}
		}

		public class UnknownMorph : BinarySerializable {
			public uint Count { get; set; }
			public float Float_04 { get; set; }
			public float Float_08 { get; set; }
			public string Name { get; set; }
			public uint[] Indices { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
				Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
				if (!Loader.IsBinaryData) Name = s.SerializeString(Name, 64, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				
				Indices = s.SerializeArray<uint>(Indices, Count, name: nameof(Indices));
			}
		}
	}
}
