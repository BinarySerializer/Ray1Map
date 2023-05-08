using BinarySerializer;

namespace Ray1Map.Jade {
	public abstract class AI_Behaviour : AI_ChunkPredicated {
		public int BehaviourSetId { get; set; }
		public ushort BehaviourId { get; set; }
		public uint ControllersCount { get; set; }
		public int[] ControllerIds { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			BehaviourSetId = s.Serialize<int>(BehaviourSetId, name: nameof(BehaviourSetId));
			if (Version >= 2) BehaviourId = s.Serialize<ushort>(BehaviourId, name: nameof(BehaviourId));
			if (Version >= 5) {
				ControllersCount = s.Serialize<uint>(ControllersCount, name: nameof(ControllersCount));
				ControllerIds = s.SerializeArray<int>(ControllerIds, ControllersCount, name: nameof(ControllerIds));
			}
		}
	}
}
