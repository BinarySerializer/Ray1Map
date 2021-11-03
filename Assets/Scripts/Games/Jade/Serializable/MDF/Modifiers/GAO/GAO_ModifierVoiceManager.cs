using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierVoiceManager : MDF_Modifier {
		public uint Version { get; set; }
		public uint BankID { get; set; }
		public uint EventsCount { get; set; }
		public uint[] Events { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			BankID = s.Serialize<uint>(BankID, name: nameof(BankID));
			EventsCount = s.Serialize<uint>(EventsCount, name: nameof(EventsCount));
			Events = s.SerializeArray<uint>(Events, EventsCount, name: nameof(Events));
		}
	}
}
