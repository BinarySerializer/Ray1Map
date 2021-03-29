using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class GEO_ModifierMorphing : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_08 { get; set; }
		public uint VertexMorphsCount { get; set; }
		public uint UnknownMorphsCount { get; set; }
		public VertexMorph[] VertexMorphs { get; set; }
		public UnknownMorph[] UnknownMorphs { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			VertexMorphsCount = s.Serialize<uint>(VertexMorphsCount, name: nameof(VertexMorphsCount));
			UnknownMorphsCount = s.Serialize<uint>(UnknownMorphsCount, name: nameof(UnknownMorphsCount));
			VertexMorphs = s.SerializeObjectArray<VertexMorph>(VertexMorphs, VertexMorphsCount, name: nameof(VertexMorphs));
			UnknownMorphs = s.SerializeObjectArray<UnknownMorph>(UnknownMorphs, UnknownMorphsCount, name: nameof(UnknownMorphs));
		}

		public class VertexMorph : R1Serializable {
			public uint Count { get; set; }
			public string Name { get; set; }
			public uint[] Indices { get; set; }
			public Jade_Vector[] Values { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				if(!Loader.IsBinaryData) Name = s.SerializeString(Name, 64, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				Indices = s.SerializeArray<uint>(Indices, Count, name: nameof(Indices));
				Values = s.SerializeObjectArray<Jade_Vector>(Values, Count, name: nameof(Values));
			}
		}

		public class UnknownMorph : R1Serializable {
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
