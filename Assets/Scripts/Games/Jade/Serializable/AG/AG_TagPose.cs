using BinarySerializer;

namespace Ray1Map.Jade
{
	public class AG_TagPose : BinarySerializable {
		public AG_AnimGraph Pre_AnimGraph { get; set; }

		public int UnknownBytesCount { get; set; } = 8;
		public short TagId { get; set; }
		public short ClipCount { get; set; }
		public short ClipOffset { get; set; }

		public byte[] UnknownBytes { get; set; }
		public byte FightMatrix { get; set; }
		public PoseType Type { get; set; }
		public uint HasProperties { get; set; }
		public AG_GroupProperties Properties { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if (Pre_AnimGraph.Version >= 4) {
				UnknownBytesCount = s.Serialize<int>(UnknownBytesCount, name: nameof(UnknownBytesCount));
			}
			TagId = s.Serialize<short>(TagId, name: nameof(TagId));
			ClipCount = s.Serialize<short>(ClipCount, name: nameof(ClipCount));
			ClipOffset = s.Serialize<short>(ClipOffset, name: nameof(ClipOffset));
			if(!Loader.IsBinaryData) UnknownBytes = s.SerializeArray<byte>(UnknownBytes, UnknownBytesCount, name: nameof(UnknownBytes));
			FightMatrix = s.Serialize<byte>(FightMatrix, name: nameof(FightMatrix));
			Type = s.Serialize<PoseType>(Type, name: nameof(Type));
			HasProperties = s.Serialize<uint>(HasProperties, name: nameof(HasProperties));
			if(HasProperties != 0) Properties = s.SerializeObject<AG_GroupProperties>(Properties, name: nameof(Properties));
		}

		public enum PoseType : uint {
			Grab = 0,
			Contact = 1,
			Idle = 2,
			Down = 3
		}
	}
}