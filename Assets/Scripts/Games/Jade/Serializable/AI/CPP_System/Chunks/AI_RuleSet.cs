using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_RuleSet : AI_Chunk {
		public uint SetsCount { get; set; }
		public int[] SetIDs { get; set; }
		public uint IncludedRuleSetsCount { get; set; }
		public int[] IncludedRuleSetIDs { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			SetsCount = s.Serialize<uint>(SetsCount, name: nameof(SetsCount));
			SetIDs = s.SerializeArray<int>(SetIDs, SetsCount, name: nameof(SetIDs));
			IncludedRuleSetsCount = s.Serialize<uint>(IncludedRuleSetsCount, name: nameof(IncludedRuleSetsCount));
			IncludedRuleSetIDs = s.SerializeArray<int>(IncludedRuleSetIDs, System.Math.Min(4, IncludedRuleSetsCount), name: nameof(IncludedRuleSetIDs));
		}
	}
}
