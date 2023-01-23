using BinarySerializer;

namespace Ray1Map.Jade {
	public class SND_ModifierSoundVol : MDF_Modifier {
		public uint Version { get; set; }
		public uint Flags { get; set; }
		public uint GroupId { get; set; }
		public float NearVolume { get; set; } = 0.5f;
		public float[] Near { get; set; } = new float[] { 30, 30, 30 };
		public float FarVolume { get; set; } = 1.0f;
		public float[] Far { get; set; } = new float[] { 100, 100, 100 };
		public float ActivationRadius { get; set; } = 120f;
		public uint VolumeRequestId { get; set; } = uint.MaxValue;
		public byte[] Reserved { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			GroupId = s.Serialize<uint>(GroupId, name: nameof(GroupId));
			NearVolume = s.Serialize<float>(NearVolume, name: nameof(NearVolume));
			Near = s.SerializeArray<float>(Near, 3, name: nameof(Near));
			FarVolume = s.Serialize<float>(FarVolume, name: nameof(FarVolume));
			Far = s.SerializeArray<float>(Far, 3, name: nameof(Far));

			if (!Loader.IsBinaryData) {
				ActivationRadius = s.Serialize<float>(ActivationRadius, name: nameof(ActivationRadius));
				VolumeRequestId = s.Serialize<uint>(VolumeRequestId, name: nameof(VolumeRequestId));
				Reserved = s.SerializeArray<byte>(Reserved, 128, name: nameof(Reserved));
			}
		}
	}
}
