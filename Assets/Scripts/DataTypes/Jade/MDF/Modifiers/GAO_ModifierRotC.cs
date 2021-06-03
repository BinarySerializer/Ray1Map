using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierRotC : MDF_Modifier {
		public uint Version { get; set; }
		public uint Flags { get; set; }
		public float Inertia { get; set; }
		public float Damping { get; set; }
		public Jade_Vector Gravity { get; set; }
		public Jade_Vector PivotPoint { get; set; }
		public float SettleTime { get; set; }
		public float PreventSettleSpeed { get; set; }
		public uint RotConstraintsCount { get; set; }
		public RotConstraint[] RotConstraints { get; set; }
		public float ForceWind { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Inertia = s.Serialize<float>(Inertia, name: nameof(Inertia));
			Damping = s.Serialize<float>(Damping, name: nameof(Damping));
			Gravity = s.SerializeObject<Jade_Vector>(Gravity, name: nameof(Gravity));
			if (Version >= 2) {
				PivotPoint = s.SerializeObject<Jade_Vector>(PivotPoint, name: nameof(PivotPoint));
				SettleTime = s.Serialize<float>(SettleTime, name: nameof(SettleTime));
			}
			if (Version >= 3) PreventSettleSpeed = s.Serialize<float>(PreventSettleSpeed, name: nameof(PreventSettleSpeed));
			RotConstraintsCount = s.Serialize<uint>(RotConstraintsCount, name: nameof(RotConstraintsCount));
			RotConstraints = s.SerializeObjectArray<RotConstraint>(RotConstraints, RotConstraintsCount, name: nameof(RotConstraints));
			if (Version >= 1) ForceWind = s.Serialize<float>(ForceWind, name: nameof(ForceWind));

		}

		public class RotConstraint : BinarySerializable {
			public Jade_Vector NormalMin { get; set; }
			public Jade_Vector NormalMax { get; set; }
			public Jade_Vector DampingNormalMin { get; set; }
			public Jade_Vector DampingNormalMax { get; set; }
			public float Editor_Float_0 { get; set; }
			public float Editor_Float_1 { get; set; }
			public float Editor_Float_2 { get; set; }
			public float Editor_Float_3 { get; set; }
			public uint Editor_UInt_4 { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				NormalMin = s.SerializeObject<Jade_Vector>(NormalMin, name: nameof(NormalMin));
				NormalMax = s.SerializeObject<Jade_Vector>(NormalMax, name: nameof(NormalMax));
				DampingNormalMin = s.SerializeObject<Jade_Vector>(DampingNormalMin, name: nameof(DampingNormalMin));
				DampingNormalMax = s.SerializeObject<Jade_Vector>(DampingNormalMax, name: nameof(DampingNormalMax));
				if (!Loader.IsBinaryData) {
					Editor_Float_0 = s.Serialize<float>(Editor_Float_0, name: nameof(Editor_Float_0));
					Editor_Float_1 = s.Serialize<float>(Editor_Float_1, name: nameof(Editor_Float_1));
					Editor_Float_2 = s.Serialize<float>(Editor_Float_2, name: nameof(Editor_Float_2));
					Editor_Float_3 = s.Serialize<float>(Editor_Float_3, name: nameof(Editor_Float_3));
					Editor_UInt_4 = s.Serialize<uint>(Editor_UInt_4, name: nameof(Editor_UInt_4));
				}
			}
		}
	}
}
