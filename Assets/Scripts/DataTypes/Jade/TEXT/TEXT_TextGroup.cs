using System;
using System.Linq;
using BinarySerializer;

namespace R1Engine.Jade {
	public class TEXT_TextGroup : Jade_File {
		public override string Export_Extension => "txg";
		public Jade_GenericReference[] Text { get; set; } // Only resolve the one with the current language ID

		protected override void SerializeFile(SerializerObject s) {
			Text = s.SerializeObjectArray<Jade_GenericReference>(Text, FileSize / 8, name: nameof(Text));
			if (!Loader.IsBinaryData) {
				foreach (var txl in Text) txl?.Resolve();
			}
		}

		public Jade_GenericReference GetUsedReference(int languageID) {
			if(languageID >= 0 && Text.Length > languageID && !Text[languageID].IsNull) return Text[languageID];
			return Text.FirstOrDefault(t => !t.IsNull);
		}
	}
}
