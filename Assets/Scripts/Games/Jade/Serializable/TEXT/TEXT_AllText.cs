using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class TEXT_AllText : Jade_File {
		public override string Export_Extension => "txt";
		public Jade_GenericReference[] Text { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Text = s.SerializeObjectArray<Jade_GenericReference>(Text, FileSize / 8, name: nameof(Text));
			if (!Loader.IsBinaryData) {
				foreach (var txg in Text) txg?.Resolve();
			}
		}
	}
}
