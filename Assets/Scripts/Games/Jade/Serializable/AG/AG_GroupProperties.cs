using BinarySerializer;

namespace Ray1Map.Jade
{
	public class AG_GroupProperties : BinarySerializable {
		public int Id { get; set; }
		public AG_Bitfield PropertiesBitfield { get; set; }
		public uint PropertiesCount { get; set; }
		public AG_Property[] Properties { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Id = s.Serialize<int>(Id, name: nameof(Id));
			PropertiesBitfield = s.SerializeObject<AG_Bitfield>(PropertiesBitfield, name: nameof(PropertiesBitfield));
			PropertiesCount = s.Serialize<uint>(PropertiesCount, name: nameof(PropertiesCount));
			Properties = s.SerializeObjectArray<AG_Property>(Properties, PropertiesCount, name: nameof(Properties));
		}
	}
}