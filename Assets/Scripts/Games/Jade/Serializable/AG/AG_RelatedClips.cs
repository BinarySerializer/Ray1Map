using BinarySerializer;

namespace Ray1Map.Jade
{
	public class AG_RelatedClips : BinarySerializable {
		public ushort[] Clips { get; set; }
		public byte[] RelatedClipsActionTrack { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Clips = s.SerializeArray<ushort>(Clips, 32, name: nameof(Clips));
			RelatedClipsActionTrack = s.SerializeArray<byte>(RelatedClipsActionTrack, 32, name: nameof(RelatedClipsActionTrack));
		}
	}
}