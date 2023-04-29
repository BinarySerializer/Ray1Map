using BinarySerializer;

namespace Ray1Map.Jade {
	public class AUDIO_ModifierVolumetricSound : MDF_Modifier {
		public uint DataSize { get; set; }
		public uint Version { get; set; }

		public byte ActivationShape { get; set; }
		public float Thickness { get; set; }
		public uint FollowersCount { get; set; }
		public float Speed { get; set; }
		public float Theta { get; set; }
		public int IsMultiTrack { get; set; } // Boolean
		public uint MultiTrackEvent { get; set; }
		public uint[] Events { get; set; }
		public int LinkFollowersToMicro { get; set; } // Boolean
		public float[] FollowerAngle { get; set; }
		public Zone FollowersActivationVolume { get; set; }
		public Zone FollowersDeactivationVolume { get; set; }

		public uint RandomFXEvent { get; set; }
		public int Polyphony { get; set; }
		public float DelayMin { get; set; }
		public float DelayMax { get; set; }
		public float MinRadius { get; set; }
		public float MaxRadius { get; set; }
		public Zone RandomFXActivationVolume { get; set; }
		public Zone RandomFXDeactivationVolume { get; set; }

		public float ListenerMinRadius { get; set; }
		public float Volume { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
			Version = s.Serialize<uint>(Version, name: nameof(Version));

			ActivationShape = s.Serialize<byte>(ActivationShape, name: nameof(ActivationShape));
			Thickness = s.Serialize<float>(Thickness, name: nameof(Thickness));
			FollowersCount = s.Serialize<uint>(FollowersCount, name: nameof(FollowersCount));
			Speed = s.Serialize<float>(Speed, name: nameof(Speed));
			Theta = s.Serialize<float>(Theta, name: nameof(Theta));
			IsMultiTrack = s.Serialize<int>(IsMultiTrack, name: nameof(IsMultiTrack));
			MultiTrackEvent = s.Serialize<uint>(MultiTrackEvent, name: nameof(MultiTrackEvent));

			if(IsMultiTrack == 0)
				Events = s.SerializeArray<uint>(Events, FollowersCount, name: nameof(Events));

			LinkFollowersToMicro = s.Serialize<int>(LinkFollowersToMicro, name: nameof(LinkFollowersToMicro));
			if(LinkFollowersToMicro != 0)
				FollowerAngle = s.SerializeArray<float>(FollowerAngle, FollowersCount, name: nameof(FollowerAngle));

			FollowersActivationVolume = s.SerializeObject<Zone>(FollowersActivationVolume, name: nameof(FollowersActivationVolume));
			FollowersDeactivationVolume = s.SerializeObject<Zone>(FollowersDeactivationVolume, name: nameof(FollowersDeactivationVolume));
			
			RandomFXEvent = s.Serialize<uint>(RandomFXEvent, name: nameof(RandomFXEvent));
			Polyphony = s.Serialize<int>(Polyphony, name: nameof(Polyphony));
			DelayMin = s.Serialize<float>(DelayMin, name: nameof(DelayMin));
			if (Version >= 2) DelayMax = s.Serialize<float>(DelayMax, name: nameof(DelayMax));

			MinRadius = s.Serialize<float>(MinRadius, name: nameof(MinRadius));
			MaxRadius = s.Serialize<float>(MaxRadius, name: nameof(MaxRadius));
			RandomFXActivationVolume = s.SerializeObject<Zone>(RandomFXActivationVolume, name: nameof(RandomFXActivationVolume));
			RandomFXDeactivationVolume = s.SerializeObject<Zone>(RandomFXDeactivationVolume, name: nameof(RandomFXDeactivationVolume));

			ListenerMinRadius = s.Serialize<float>(ListenerMinRadius, name: nameof(ListenerMinRadius));
			Volume = s.Serialize<float>(Volume, name: nameof(Volume));

			uint readSize = (uint)(s.CurrentPointer - Offset);
			if (readSize != DataSize) {
				s.SystemLogger?.LogWarning($"{Offset}: {GetType()} was not fully serialized: Data Size: {DataSize:X8} / Serialized: {readSize:X8}");
				s.Goto(Offset + DataSize);
			}
		}

		public class Zone : BinarySerializable {
			public DARE_ModifierSound.Zone Volume { get; set; }
			public float FadeTime { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Volume = s.SerializeObject<DARE_ModifierSound.Zone>(Volume, name: nameof(Volume));
				FadeTime = s.Serialize<float>(FadeTime, name: nameof(FadeTime));
			}
		}
	}
}
