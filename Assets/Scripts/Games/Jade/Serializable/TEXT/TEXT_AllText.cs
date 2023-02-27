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

		public void MergeText(TEXT_AllText text) {
			if(Text == null || text?.Text == null) return;
			for (int i = 0; i < Text.Length; i++) {
				if ((Text[i].Key == text.Text[i].Key) && (Text[i].Value != text.Text[i].Value)) {
					// Merge
					var txg1 = (TEXT_TextGroup)Text[i].Value;
					var txg2 = (TEXT_TextGroup)text.Text[i].Value;
					if (txg1 == null) {
						Text[i].Value = txg2;
						return;
					} else if(txg2 == null) return;

					for (int j = 0; j < txg1.Text.Length; j++) {
						var txl1 = txg1.Text[j];
						var txl2 = txg2.Text[j];
						if (txl1.TextList == null && txl1.Ids?.Value == null && txl1.Strings?.Value == null) {
							if (txl2.TextList != null || txl2.Ids?.Value != null || txl2.Strings?.Value != null) {
								txg1.Text[j] = txl2;
							}
						}
					}
				}
			}
		}
	}
}
