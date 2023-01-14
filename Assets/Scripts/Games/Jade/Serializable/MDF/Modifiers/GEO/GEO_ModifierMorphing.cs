using BinarySerializer;

namespace Ray1Map.Jade {
	public class GEO_ModifierMorphing : MDF_Modifier {
		public uint DataSize { get; set; }
		public uint Flags { get; set; }
		public uint PointsCount { get; set; }
		public uint MorphDataCount { get; set; }
		public uint ChannelsCount { get; set; }
		public Data[] MorphData { get; set; }
		public Channel[] MorphChannels { get; set; }

		public uint Xenon_Version { get; set; } = 1;
		public uint Xenon_ElementsCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
			if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && DataSize == 0) {
				Xenon_Version = s.Serialize<uint>(Xenon_Version, name: nameof(Xenon_Version));
			}
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			PointsCount = s.Serialize<uint>(PointsCount, name: nameof(PointsCount));
			MorphDataCount = s.Serialize<uint>(MorphDataCount, name: nameof(MorphDataCount));
			ChannelsCount = s.Serialize<uint>(ChannelsCount, name: nameof(ChannelsCount));
			if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && Xenon_Version == 2) Xenon_ElementsCount = s.Serialize<uint>(Xenon_ElementsCount, name: nameof(Xenon_ElementsCount));
			MorphData = s.SerializeObjectArray<Data>(MorphData, MorphDataCount, v => {
				v.Xenon_Version = Xenon_Version;
				v.Xenon_ElementsCount = Xenon_ElementsCount;
			}, name: nameof(MorphData));
			MorphChannels = s.SerializeObjectArray<Channel>(MorphChannels, ChannelsCount, name: nameof(MorphChannels));

			// Check for dummy channels
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR) && !Loader.IsBinaryData) {
				if (ChannelsCount % 4 != 0) {
					Context.SystemLogger?.LogWarning($"{GetType().Name} @ {Offset}: MorphChannels does not contain Dummy channels - this might cause errors");
				}
			}
		}

		public class Data : BinarySerializable {
			public uint Xenon_Version { get; set; }
			public uint Xenon_ElementsCount { get; set; } // Set in onPreSerialize

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
				if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon)) {
					if (Xenon_Version == 2) {
						XenonDatas = s.SerializeObjectArray<XenonData>(XenonDatas, Xenon_ElementsCount, name: nameof(XenonDatas));
					} else if (Xenon_Version >= 3) {
						XenonDatas = s.SerializeObjectArray<XenonData>(XenonDatas, 1, name: nameof(XenonDatas));
					}
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
