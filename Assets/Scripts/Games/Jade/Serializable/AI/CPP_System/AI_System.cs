using BinarySerializer;
using System.Linq;

namespace Ray1Map.Jade {
	public class AI_System : Jade_File {
		public override bool HasHeaderBFFile => true;

		public int Version { get; set; }
		public int UseDebugString { get; set; }
		public AI_Tools_StringPool DebugStringPool { get; set; }
		public AI_Tools_StringPool StringPool { get; set; }
		public uint ChunksCount { get; set; }
		public Chunk[] Chunks { get; set; }

		public uint ControllersCount { get; set; }
		public int[] ControllerIDs { get; set; }
		public uint RuleSetsCount { get; set; }
		public int[] RuleSetIDs { get; set; }
		public uint RulesCount { get; set; }
		public uint BehavioursCount { get; set; }
		public uint LevelIndex { get; set; }
		public uint MindIndex { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Version = s.Serialize<int>(Version, name: nameof(Version));
			if (Version >= 1) {
				UseDebugString = s.Serialize<int>(UseDebugString, name: nameof(UseDebugString));
				DebugStringPool = s.SerializeObject<AI_Tools_StringPool>(DebugStringPool, name: nameof(DebugStringPool));
			}
			StringPool = s.SerializeObject<AI_Tools_StringPool>(StringPool, name: nameof(StringPool));
			ChunksCount = s.Serialize<uint>(ChunksCount, name: nameof(ChunksCount));
			Chunks = s.SerializeObjectArray<Chunk>(Chunks, ChunksCount, onPreSerialize: c => c.Pre_System = this, name: nameof(Chunks));
			if (Version >= 5) {
				ControllersCount = s.Serialize<uint>(ControllersCount, name: nameof(ControllersCount));
				ControllerIDs = s.SerializeArray<int>(ControllerIDs, ControllersCount, name: nameof(ControllerIDs));
			}
			RuleSetsCount = s.Serialize<uint>(RuleSetsCount, name: nameof(RuleSetsCount));
			RuleSetIDs = s.SerializeArray<int>(RuleSetIDs, RuleSetsCount, name: nameof(RuleSetIDs));
			if (Version >= 2) {
				RulesCount = s.Serialize<uint>(RulesCount, name: nameof(RulesCount));
				BehavioursCount = s.Serialize<uint>(BehavioursCount, name: nameof(BehavioursCount));
				LevelIndex = s.Serialize<uint>(LevelIndex, name: nameof(LevelIndex));
				MindIndex = s.Serialize<uint>(MindIndex, name: nameof(MindIndex));
			}
		}

		public class Chunk : BinarySerializable {
			public AI_System Pre_System { get; set; }

			public AI_ClassId Type { get; set; }
			public AI_Chunk Data { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Type = s.Serialize<AI_ClassId>(Type, name: nameof(Type));
				Data = Type switch {
					AI_ClassId.Undefined => null,
					AI_ClassId.RuleSet => SerializeChunk<AI_RuleSet>(s),
					AI_ClassId.Set => SerializeChunk<AI_ChunkSet>(s),
					AI_ClassId.Rule => SerializeChunk<AI_Rule>(s),
					AI_ClassId.WeightedRule => SerializeChunk<AI_ChunkWeighted<AI_Rule>>(s),
					AI_ClassId.BehaviourSet => SerializeChunk<AI_BehaviourSet>(s),
					AI_ClassId.BehaviourSetRef => SerializeChunk<AI_BehaviourSetRef>(s),
					AI_ClassId.BehaviourInline => SerializeChunk<AI_BehaviourInline>(s),
					AI_ClassId.WeightedBehaviour => SerializeChunk<AI_ChunkWeighted<AI_Behaviour>>(s),
					AI_ClassId.Mind => SerializeChunk<AI_Mind>(s),
					AI_ClassId.Attribute => SerializeChunk<AI_MindAttribute>(s),
					AI_ClassId.AttributeRef => SerializeChunk<AI_MindAttributeRef>(s),
					AI_ClassId.Predicate => SerializeChunk<AI_Predicate>(s),
					AI_ClassId.PredicateSet => SerializeChunk<AI_PredicateSet>(s),
					AI_ClassId.PredicateSetRef => SerializeChunk<AI_PredicateSetRef>(s),
					AI_ClassId.Level => SerializeChunk<AI_LevelCalibrator>(s),
					AI_ClassId.PredicateWithParam => SerializeChunk<AI_PredicateWithParam>(s),
					AI_ClassId.AIController => SerializeChunk<AI_Controller>(s),
					AI_ClassId.AIControllerRef => SerializeChunk<AI_ControllerRef>(s),
					_ => throw new System.NotImplementedException($"TODO: Serialize AI_System chunk type {Type}")
				};
			}

			private T SerializeChunk<T>(SerializerObject s) where T : AI_Chunk, new()
				=> s.SerializeObject<T>((T)Data, onPreSerialize: c => c.Pre_System = Pre_System, name: nameof(Data));
		}
	}
}
