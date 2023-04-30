using BinarySerializer;

namespace Ray1Map.Jade
{
	public class AG_TagClip : BinarySerializable {
		public AG_AnimGraph Pre_AnimGraph { get; set; }

		public int NameLength { get; set; } = 0x40;
		public int UnknownBytesCount { get; set; } = 8;
		public int ReceiveInfoCount { get; set; } = 4;

		public uint TagInfoNameHash { get; set; }
		public string Name { get; set; }
		public byte[] UnknownBytes { get; set; }

		public ushort ActionId { get; set; }
		public ushort Status { get; set; }
		public float ClipLength { get; set; }

		public float Float_PreV13_00 { get; set; }
		public float Float_PreV13_04 { get; set; }
		public uint UInt_PreV4_00 { get; set; }
		public uint UInt_PreV4_04 { get; set; }

		public int ClipType { get; set; }

		public AG_ReceiveInfo[] ReceiveInfo { get; set; }
		public byte ReceiveInfoNb { get; set; }
		public ushort PoseStart { get; set; }
		public ushort PoseEnd { get; set; }
		public ushort Priority { get; set; }
		public ushort MaxLoopingTime { get; set; }
		public ushort UShort_PreV3_00 { get; set; }
		public float AnimLength { get; set; }
		public uint UInt_PreV2_00 { get; set; }
		public uint BlendingMode { get; set; }
		public ushort BlendingTime { get; set; }
		public ushort BlendingProgressive { get; set; }
		public ushort OutBlendingTime { get; set; }
		public ushort NextClip { get; set; }
		public uint UInt_PreV6_00 { get; set; }
		public float Float_PreV6_04 { get; set; }
		public float Float_PreV6_08 { get; set; }
		public float Float_PreV6_0C { get; set; }
		public float Float_PreV6_10 { get; set; }
		public uint TriggersCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if (Pre_AnimGraph.Version >= 4) {
				NameLength = s.Serialize<int>(NameLength, name: nameof(NameLength));
				UnknownBytesCount = s.Serialize<int>(UnknownBytesCount, name: nameof(UnknownBytesCount));
				ReceiveInfoCount = s.Serialize<int>(ReceiveInfoCount, name: nameof(ReceiveInfoCount));
			}
			TagInfoNameHash = s.Serialize<uint>(TagInfoNameHash, name: nameof(TagInfoNameHash));
			if (!Loader.IsBinaryData) Name = s.SerializeString(Name, length: NameLength, name: nameof(Name));
			UnknownBytes = s.SerializeArray<byte>(UnknownBytes, UnknownBytesCount, name: nameof(UnknownBytes));

			ActionId = s.Serialize<ushort>(ActionId, name: nameof(ActionId));
			Status = s.Serialize<ushort>(Status, name: nameof(Status));
			ClipLength = s.Serialize<float>(ClipLength, name: nameof(ClipLength));
			if (Pre_AnimGraph.Version < 13) {
				Float_PreV13_00 = s.Serialize<float>(Float_PreV13_00, name: nameof(Float_PreV13_00));
				Float_PreV13_04 = s.Serialize<float>(Float_PreV13_04, name: nameof(Float_PreV13_04));
			}
			if (Pre_AnimGraph.Version < 4) {
				UInt_PreV4_00 = s.Serialize<uint>(UInt_PreV4_00, name: nameof(UInt_PreV4_00));
				UInt_PreV4_04 = s.Serialize<uint>(UInt_PreV4_04, name: nameof(UInt_PreV4_04));
			}
			ClipType = s.Serialize<int>(ClipType, name: nameof(ClipType));
			ReceiveInfo = s.SerializeObjectArray<AG_ReceiveInfo>(ReceiveInfo, ReceiveInfoCount, name: nameof(ReceiveInfo));
			ReceiveInfoNb = s.Serialize<byte>(ReceiveInfoNb, name: nameof(ReceiveInfoNb));
			PoseStart = s.Serialize<ushort>(PoseStart, name: nameof(PoseStart));
			PoseEnd = s.Serialize<ushort>(PoseEnd, name: nameof(PoseEnd));
			Priority = s.Serialize<ushort>(Priority, name: nameof(Priority));
			MaxLoopingTime = s.Serialize<ushort>(MaxLoopingTime, name: nameof(MaxLoopingTime));
			if (Pre_AnimGraph.Version < 3) UShort_PreV3_00 = s.Serialize<ushort>(UShort_PreV3_00, name: nameof(UShort_PreV3_00));
			AnimLength = s.Serialize<float>(AnimLength, name: nameof(AnimLength));
			if (Pre_AnimGraph.Version >= 2) {
				BlendingMode = s.Serialize<uint>(BlendingMode, name: nameof(BlendingMode));
				BlendingTime = s.Serialize<ushort>(BlendingTime, name: nameof(BlendingTime));
				BlendingProgressive = s.Serialize<ushort>(BlendingProgressive, name: nameof(BlendingProgressive));
				if (Pre_AnimGraph.Version >= 10) OutBlendingTime = s.Serialize<ushort>(OutBlendingTime, name: nameof(OutBlendingTime));
				if (Pre_AnimGraph.Version >= 11) NextClip = s.Serialize<ushort>(NextClip, name: nameof(NextClip));
			} else {
				UInt_PreV2_00 = s.Serialize<uint>(UInt_PreV2_00, name: nameof(UInt_PreV2_00));
			}
			if (Pre_AnimGraph.Version < 6) {
				UInt_PreV6_00 = s.Serialize<uint>(UInt_PreV6_00, name: nameof(UInt_PreV6_00));
				Float_PreV6_04 = s.Serialize<float>(Float_PreV6_04, name: nameof(Float_PreV6_04));
				Float_PreV6_08 = s.Serialize<float>(Float_PreV6_08, name: nameof(Float_PreV6_08));
				Float_PreV6_0C = s.Serialize<float>(Float_PreV6_0C, name: nameof(Float_PreV6_0C));
				Float_PreV6_10 = s.Serialize<float>(Float_PreV6_10, name: nameof(Float_PreV6_10));
			}
			TriggersCount = s.Serialize<uint>(TriggersCount, name: nameof(TriggersCount));

			throw new System.NotImplementedException($"TODO: Implement rest of {GetType()}");
		}
	}
}