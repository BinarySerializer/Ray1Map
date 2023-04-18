using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierInteractivePlant : MDF_Modifier {
		public uint Version { get; set; }
		public float InteractionSphereRadius { get; set; }
		public float InteractionMaxForce { get; set; }
		public float InteractionMaxForceRatio { get; set; }
		public Jade_Vector ModifierOffset { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			InteractionSphereRadius = s.Serialize<float>(InteractionSphereRadius, name: nameof(InteractionSphereRadius));
			InteractionMaxForce = s.Serialize<float>(InteractionMaxForce, name: nameof(InteractionMaxForce));
			InteractionMaxForceRatio = s.Serialize<float>(InteractionMaxForceRatio, name: nameof(InteractionMaxForceRatio));
			ModifierOffset = s.SerializeObject<Jade_Vector>(ModifierOffset, name: nameof(ModifierOffset));
		}

	}
}
