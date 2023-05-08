using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_Mind : AI_Chunk {
		public uint AttributesCount { get; set; }
		public int[] LoadingAttributesId { get; set; }
		public uint PredicateValueContentCount { get; set; }
		public uint PredicateValuesCount { get; set; }
		public PredicateValue[] PredicateValues { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			AttributesCount = s.Serialize<uint>(AttributesCount, name: nameof(AttributesCount));
			LoadingAttributesId = s.SerializeArray<int>(LoadingAttributesId, AttributesCount, name: nameof(LoadingAttributesId));
			PredicateValueContentCount = s.Serialize<uint>(PredicateValueContentCount, name: nameof(PredicateValueContentCount));
			PredicateValuesCount = s.Serialize<uint>(PredicateValuesCount, name: nameof(PredicateValuesCount));
			PredicateValues = s.SerializeObjectArray<PredicateValue>(PredicateValues, PredicateValuesCount, onPreSerialize: pv => pv.Pre_Mind = this, name: nameof(PredicateValues));

		}

		public class PredicateValue : BinarySerializable {
			public AI_Mind Pre_Mind { get; set; }

			public uint[] NameCrc32 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				NameCrc32 = s.SerializeArray<uint>(NameCrc32, System.Math.Min(64, Pre_Mind.PredicateValueContentCount), name: nameof(NameCrc32));
			}
		}
	}
}
