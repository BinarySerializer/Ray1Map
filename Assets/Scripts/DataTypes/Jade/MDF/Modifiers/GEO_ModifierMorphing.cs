using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GEO_ModifierMorphing : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Flags { get; set; }
		public uint PointsCount { get; set; }
		public uint MorphDataCount { get; set; }
		public uint ChannelsCount { get; set; }
		public Data[] MorphData { get; set; }
		public Channel[] MorphChannels { get; set; }

		public uint Xenon_Type { get; set; } = 1;
		public uint Xenon_Count { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && UInt_00 == 0) {
				Xenon_Type = s.Serialize<uint>(Xenon_Type, name: nameof(Xenon_Type));
			}
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			PointsCount = s.Serialize<uint>(PointsCount, name: nameof(PointsCount));
			MorphDataCount = s.Serialize<uint>(MorphDataCount, name: nameof(MorphDataCount));
			ChannelsCount = s.Serialize<uint>(ChannelsCount, name: nameof(ChannelsCount));
			if (Xenon_Type == 2) Xenon_Count = s.Serialize<uint>(Xenon_Count, name: nameof(Xenon_Count));
			MorphData = s.SerializeObjectArray<Data>(MorphData, MorphDataCount, v => {
				v.Xenon_Type = Xenon_Type;
				v.Xenon_Count = Xenon_Count;
			}, name: nameof(MorphData));
			MorphChannels = s.SerializeObjectArray<Channel>(MorphChannels, ChannelsCount, name: nameof(MorphChannels));
		}

		public class Data : BinarySerializable {
			public uint Xenon_Type { get; set; }
			public uint Xenon_Count { get; set; } // Set in onPreSerialize

			public uint VectorsCount { get; set; }
			public string Name { get; set; }
			public uint[] Indices { get; set; }
			public Jade_Vector[] Vectors { get; set; }

			public XenonData[] XenonDatas { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				VectorsCount = s.Serialize<uint>(VectorsCount, name: nameof(VectorsCount));
				if(!Loader.IsBinaryData) Name = s.SerializeString(Name, 64, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				Indices = s.SerializeArray<uint>(Indices, VectorsCount, name: nameof(Indices));
				Vectors = s.SerializeObjectArray<Jade_Vector>(Vectors, VectorsCount, name: nameof(Vectors));
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

		public class Channel : BinarySerializable {
			public uint DataCount { get; set; }
			public float Blend { get; set; }
			public float ChannelBlend { get; set; }
			public string Name { get; set; }
			public uint[] DataIndices { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				DataCount = s.Serialize<uint>(DataCount, name: nameof(DataCount));
				Blend = s.Serialize<float>(Blend, name: nameof(Blend));
				ChannelBlend = s.Serialize<float>(ChannelBlend, name: nameof(ChannelBlend));
				if (!Loader.IsBinaryData) Name = s.SerializeString(Name, 64, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				
				DataIndices = s.SerializeArray<uint>(DataIndices, DataCount, name: nameof(DataIndices));
			}
		}
	}
}
