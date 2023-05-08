using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_PredicateWithParam : AI_Predicate {
		public AI_AttributeValue ParameterValue { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			ParameterValue = s.SerializeObject<AI_AttributeValue>(ParameterValue, name: nameof(ParameterValue));
		}
	}
}
