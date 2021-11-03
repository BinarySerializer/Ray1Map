using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class DYN_ODE : BinarySerializable {
		public byte Version { get; set; }
		public byte Type { get; set; }
		public byte Flags { get; set; }
		public byte Sound { get; set; }

		public Jade_Vector OffsetVector { get; set; }
		public Jade_Matrix RotationMatrix { get; set; }

		public float LinearThreshold { get; set; }
		public float AngularThreshold { get; set; }

		public float MassInit { get; set; }

		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public uint SurfaceMode { get; set; }
		public float Mu { get; set; }
		public float Mu2 { get; set; }
		public float Bounce { get; set; }
		public float BounceVelocity { get; set; }
		public float Soft_erp { get; set; }
		public float Soft_cfm { get; set; }
		public float Motion1 { get; set; }
		public float Motion2 { get; set; }
		public float Slip1 { get; set; }
		public float Slip2 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<byte>(Version, name: nameof(Version));
			Type = s.Serialize<byte>(Type, name: nameof(Type));
			Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
			Sound = s.Serialize<byte>(Sound, name: nameof(Sound));
			if (Version >= 2)
				OffsetVector = s.SerializeObject<Jade_Vector>(OffsetVector, name: nameof(OffsetVector));
			if (Version >= 7)
				RotationMatrix = s.SerializeObject<Jade_Matrix>(RotationMatrix, name: nameof(RotationMatrix));
			if (Version >= 6) {
				LinearThreshold = s.Serialize<float>(LinearThreshold, name: nameof(LinearThreshold));
				AngularThreshold = s.Serialize<float>(AngularThreshold, name: nameof(AngularThreshold));
			}
			MassInit = s.Serialize<float>(MassInit, name: nameof(MassInit));
			if (Type != 0) {
				X = s.Serialize<float>(X, name: nameof(X));
				Y = s.Serialize<float>(Y, name: nameof(Y));
				Z = s.Serialize<float>(Z, name: nameof(Z));
			}
			if (Version >= 4) {
				SurfaceMode = s.Serialize<uint>(SurfaceMode, name: nameof(SurfaceMode));
				Mu = s.Serialize<float>(Mu, name: nameof(Mu));
				Mu2 = s.Serialize<float>(Mu2, name: nameof(Mu2));
				Bounce = s.Serialize<float>(Bounce, name: nameof(Bounce));
				BounceVelocity = s.Serialize<float>(BounceVelocity, name: nameof(BounceVelocity));
				Soft_erp = s.Serialize<float>(Soft_erp, name: nameof(Soft_erp));
				Soft_cfm = s.Serialize<float>(Soft_cfm, name: nameof(Soft_cfm));
				Motion1 = s.Serialize<float>(Motion1, name: nameof(Motion1));
				Motion2 = s.Serialize<float>(Motion2, name: nameof(Motion2));
				Slip1 = s.Serialize<float>(Slip1, name: nameof(Slip1));
				Slip2 = s.Serialize<float>(Slip2, name: nameof(Slip2));
			}
		}
	}
}
