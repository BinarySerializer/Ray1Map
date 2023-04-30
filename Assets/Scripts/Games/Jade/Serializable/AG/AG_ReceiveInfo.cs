using BinarySerializer;

namespace Ray1Map.Jade
{
	public class AG_ReceiveInfo : BinarySerializable {
		public uint Pre_Version { get; set; }
		public int HitType { get; set; }
		public int AttackerLocation { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (Pre_Version < 12) {
				HitType = s.Serialize<short>((short)HitType, name: nameof(HitType));
			} else {
				HitType = s.Serialize<int>(HitType, name: nameof(HitType));
			}
			AttackerLocation = s.Serialize<int>(AttackerLocation, name: nameof(AttackerLocation));
		}
	}
}