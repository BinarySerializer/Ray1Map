using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class TEXT_AllText : Jade_File {
		public override string Export_Extension => "txt";
		public Jade_GenericReference[] Text { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Text = s.SerializeObjectArray<Jade_GenericReference>(Text, FileSize / 8, name: nameof(Text));
			//foreach(var txg in Text) txg?.Resolve();
		}
	}
}
