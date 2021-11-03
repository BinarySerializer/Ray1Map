using System;
using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GAO_ModifierAnimatedPAG : MDF_Modifier {
        public uint Version { get; set; }
        public AnimatedPAGFlags Flags { get; set; }
        public float TimeStartMin { get; set; }
        public float TimeStartMax { get; set; }
        public float TimeStopMin { get; set; }
        public float TimeStopMax { get; set; }
        public float TimeTotalMin { get; set; }
        public float TimeTotalMax { get; set; }
        public Jade_Reference<OBJ_GameObject> PAG { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            Version = s.Serialize<uint>(Version, name: nameof(Version));;
			Flags = s.Serialize<AnimatedPAGFlags>(Flags, name: nameof(Flags));
			TimeStartMin = s.Serialize<float>(TimeStartMin, name: nameof(TimeStartMin));
			TimeStartMax = s.Serialize<float>(TimeStartMax, name: nameof(TimeStartMax));
			TimeStopMin = s.Serialize<float>(TimeStopMin, name: nameof(TimeStopMin));
			TimeStopMax = s.Serialize<float>(TimeStopMax, name: nameof(TimeStopMax));
			TimeTotalMin = s.Serialize<float>(TimeTotalMin, name: nameof(TimeTotalMin));
			TimeTotalMax = s.Serialize<float>(TimeTotalMax, name: nameof(TimeTotalMax));
			PAG = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(PAG, name: nameof(PAG))?.Resolve();
        }

        [Flags]
        public enum AnimatedPAGFlags : uint {
            None = 0,
            ApplyAroundX = 1 << 0,
            ApplyAroundY = 1 << 1,
            ApplyAroundZ = 1 << 2,
            SyncWithGameTime = 1 << 3,
        }
    }
}
