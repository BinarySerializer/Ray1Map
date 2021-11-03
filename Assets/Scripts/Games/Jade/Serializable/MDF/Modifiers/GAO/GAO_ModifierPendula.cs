using System;
using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GAO_ModifierPendula : MDF_Modifier {
        public uint Version { get; set; }
        public PendulaFlags Flags { get; set; }
        public PendulaParameters RopeX { get; set; }
        public PendulaParameters RopeY { get; set; }
        public PendulaParameters RopeZ { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Version = s.Serialize<uint>(Version, name: nameof(Version));;
            if (Version >= 1) {
                Flags = s.Serialize<PendulaFlags>(Flags, name: nameof(Flags));
				if (Flags.HasFlag(PendulaFlags.ApplyAroundX)) RopeX = s.SerializeObject<PendulaParameters>(RopeX, name: nameof(RopeX));
                if (Flags.HasFlag(PendulaFlags.ApplyAroundY)) RopeY = s.SerializeObject<PendulaParameters>(RopeY, name: nameof(RopeY));
                if (Flags.HasFlag(PendulaFlags.ApplyAroundZ)) RopeZ = s.SerializeObject<PendulaParameters>(RopeZ, name: nameof(RopeZ));
            }
		}

		public class PendulaParameters : BinarySerializable {
            public float MaxTheta { get; set; }
            public float AccelerationTime { get; set; }
            public float RatioOfAcceleration { get; set; }
            public float Frequency { get; set; }
            
			public override void SerializeImpl(SerializerObject s) {
				MaxTheta = s.Serialize<float>(MaxTheta, name: nameof(MaxTheta));
				AccelerationTime = s.Serialize<float>(AccelerationTime, name: nameof(AccelerationTime));
				RatioOfAcceleration = s.Serialize<float>(RatioOfAcceleration, name: nameof(RatioOfAcceleration));
				Frequency = s.Serialize<float>(Frequency, name: nameof(Frequency));
			}
		}

        [Flags]
        public enum PendulaFlags : uint {
            None = 0,
            ApplyAroundX = 1 << 0,
            ApplyAroundY = 1 << 1,
            ApplyAroundZ = 1 << 2,
            SyncWithGameTime = 1 << 3,
        }
	}
}
