using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_Node_DebugLink : BinarySerializable {
		public int LineNumber { get; set; }
		public Jade_Key FileKey { get; set; } // in the original .fct file

		public bool IsNull => LineNumber == -1 && FileKey.IsNull;

		public override void SerializeImpl(SerializerObject s) {
			if (FileKey == null) {
				LineNumber = -1;
				FileKey = new Jade_Key(Context, 0xFFFFFFFF);
			}
			LineNumber = s.Serialize<int>(LineNumber, name: nameof(LineNumber));
			FileKey = s.SerializeObject<Jade_Key>(FileKey, name: nameof(FileKey));
		}

		public override bool IsShortLog => true;
		public override string ShortLog => $"DebugLink({DebugLinkContent})";

		public string DebugLinkContent => IsNull ? "" : $"{FileKey}:{LineNumber}";
	}
}
