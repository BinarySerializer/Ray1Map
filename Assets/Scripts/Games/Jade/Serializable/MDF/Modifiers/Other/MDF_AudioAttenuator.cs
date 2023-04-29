using BinarySerializer;

namespace Ray1Map.Jade {
	public class MDF_AudioAttenuator : MDF_Modifier {
		public uint DataSize { get; set; }
		public uint Version { get; set; }

		public uint Flags { get; set; }
		public float InnerAttenuation { get; set; }
		public float OuterAttenuation { get; set; }
		public uint AudioTypesCount { get; set; }
		public uint[] AudioTypes { get; set; }
		public uint ObjectsCount { get; set; }
		public GameObject[] Objects { get; set; }
		public DARE_ModifierSound.Zone InnerZone { get; set; }
		public DARE_ModifierSound.Zone OuterZone { get; set; }

		public uint TriggerMode { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
			Version = s.Serialize<uint>(Version, name: nameof(Version));

			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			InnerAttenuation = s.Serialize<float>(InnerAttenuation, name: nameof(InnerAttenuation));
			OuterAttenuation = s.Serialize<float>(OuterAttenuation, name: nameof(OuterAttenuation));
			AudioTypesCount = s.Serialize<uint>(AudioTypesCount, name: nameof(AudioTypesCount));
			AudioTypes = s.SerializeArray<uint>(AudioTypes, AudioTypesCount, name: nameof(AudioTypes));
			ObjectsCount = s.Serialize<uint>(ObjectsCount, name: nameof(ObjectsCount));
			Objects = s.SerializeObjectArray<GameObject>(Objects, ObjectsCount, onPreSerialize: go => go.Pre_Modifier = this, name: nameof(Objects));
			InnerZone = s.SerializeObject<DARE_ModifierSound.Zone>(InnerZone, name: nameof(InnerZone));
			OuterZone = s.SerializeObject<DARE_ModifierSound.Zone>(OuterZone, name: nameof(OuterZone));

			TriggerMode = s.Serialize<uint>(TriggerMode, name: nameof(TriggerMode));
		}

		public class GameObject : BinarySerializable {
			public MDF_AudioAttenuator Pre_Modifier { get; set; }
			
			public Jade_Reference<OBJ_GameObject> Object { get; set; }
			public uint AudioType { get; set; }
			
			public override void SerializeImpl(SerializerObject s) {
				Object = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Object, name: nameof(Object))?.Resolve();
				if(Pre_Modifier.Version >= 3) AudioType = s.Serialize<uint>(AudioType, name: nameof(AudioType));
			}
		}
	}
}
