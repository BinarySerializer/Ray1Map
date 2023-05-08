using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_Rule : AI_ChunkPredicated {
		public uint BehaviourSetsCount { get; set; }
		public int[] BehaviourSetIds { get; set; }
		public int Active { get; set; }
		public int WeightId { get; set; }

		public ushort RuleId { get; set; }
		public int CancelBuild { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			BehaviourSetsCount = s.Serialize<uint>(BehaviourSetsCount, name: nameof(BehaviourSetsCount));
			BehaviourSetIds = s.SerializeArray<int>(BehaviourSetIds, BehaviourSetsCount, name: nameof(BehaviourSetIds));
			Active = s.Serialize<int>(Active, name: nameof(Active));
			WeightId = s.Serialize<int>(WeightId, name: nameof(WeightId));

			if (Version >= 2) {
				RuleId = s.Serialize<ushort>(RuleId, name: nameof(RuleId));
				CancelBuild = s.Serialize<int>(CancelBuild, name: nameof(CancelBuild));
			}
		}
	}
}
