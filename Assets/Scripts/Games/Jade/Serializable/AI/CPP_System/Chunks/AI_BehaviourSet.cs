using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_BehaviourSet : AI_ChunkPredicated {
		public uint BehavioursCount { get; set; }
		public int[] BehaviourIDs { get; set; }
		public uint NextBehaviourSetsCount { get; set; }
		public int[] NextBehaviourSetIDs { get; set; }
		public uint WeightSum { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			BehavioursCount = s.Serialize<uint>(BehavioursCount, name: nameof(BehavioursCount));
			BehaviourIDs = s.SerializeArray<int>(BehaviourIDs, BehavioursCount, name: nameof(BehaviourIDs));
			NextBehaviourSetsCount = s.Serialize<uint>(NextBehaviourSetsCount, name: nameof(NextBehaviourSetsCount));
			NextBehaviourSetIDs = s.SerializeArray<int>(NextBehaviourSetIDs, NextBehaviourSetsCount, name: nameof(NextBehaviourSetIDs));
			WeightSum = s.Serialize<uint>(WeightSum, name: nameof(WeightSum));
		}
	}
}
