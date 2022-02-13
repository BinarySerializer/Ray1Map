using System.Linq;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class TEXT_TextGroup : Jade_File {
		public override string Export_Extension => "txg";
		public TextRef[] Text { get; set; } // Only resolve the one with the current language ID

		public int LanguageID { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Text = s.SerializeObjectArrayUntil<TextRef>(Text, t => s.CurrentAbsoluteOffset >= Offset.AbsoluteOffset + FileSize, name: nameof(Text));
			if (!Loader.IsBinaryData) {
				foreach (var txl in Text) txl?.Resolve();
			} else if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
				GetUsedReference(LanguageID)?.ResolveBinarizedNoSound();
			}
		}

		public TextRef GetUsedReference(int languageID) {
			if(languageID >= 0 && Text.Length > languageID && !Text[languageID].IsNull) return Text[languageID];
			return Text.FirstOrDefault(t => !t.IsNull);
		}

		public class TextRef : BinarySerializable {
			public Jade_GenericReference Text { get; set; }

			public Jade_GenericReference Ids { get; set; }
			public Jade_GenericReference Strings { get; set; }
			public uint IdsFileSize { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
					Text = s.SerializeObject<Jade_GenericReference>(Text, name: nameof(Text));
				} else {
					Ids = s.SerializeObject<Jade_GenericReference>(Ids, name: nameof(Ids));
					Strings = s.SerializeObject<Jade_GenericReference>(Strings, name: nameof(Strings));
				}
			}

			public void Resolve() {
				if (Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
					Text?.Resolve();
				} else {
					Ids?.Resolve();
					Strings?.Resolve();
				}
			}

			public void ResolveBinarizedNoSound() {
				if (Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
					TextListRef?.Resolve(onPreSerialize: (_, txl) => {
						((TEXT_TextList)txl).HasSound = false;
					}, cache: LOA_Loader.CacheType.Text);
				} else {
					ResolveFileSize();
					Ids?.Resolve(cache: LOA_Loader.CacheType.Text);
					Strings?.Resolve(cache: LOA_Loader.CacheType.Text);
				}
			}
			public void ResolveFileSize() {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				Loader.RequestFileSize(Ids.Key, IdsFileSize, (size) => IdsFileSize = size, name: nameof(IdsFileSize));
			}

			public bool IsNull {
				get {
					if (Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
						return Text.IsNull;
					} else {
						return Ids.IsNull && Strings.IsNull;
					}
				}
			}

			public Jade_GenericReference TextListRef => Text;
			public TEXT_TextList TextList => (TEXT_TextList)Text?.Value;
		}
	}
}
