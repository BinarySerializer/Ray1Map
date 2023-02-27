using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa.DTP;

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

			protected override void OnChangeContext(Context oldContext, Context newContext) {
				base.OnChangeContext(oldContext, newContext);
				if (newContext.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
					if (Text == null && Ids != null && !Ids.IsNull) {
						var txl = new TEXT_TextList();
						txl.UnknownFileSize = true;
						var ids = (TEXT_Ids)(Ids.Value);
						var strings = ((TEXT_Strings)Strings.Value);
						txl.HasSound = false;
						txl.Count = (uint)(ids?.Ids?.Length ?? 0);
						txl.Text = new TEXT_OneText[txl.Count];
						var newLoader = newContext.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
						txl.Key = new Jade_Key(newContext, newLoader.Raw_GetNewKey());

						Text = new Jade_GenericReference(newContext,
							txl.Key,
							new Jade_FileType() {
								Extension = ".txl"
							}
						) {
							Value = txl
						};

						if (newLoader.Raw_RelocateKeys) {
							for (int i = 0; i < txl.Count; i++) {
								// TODO: Complete
								txl.Text[i] = new TEXT_OneText() {
									IdKey = new Jade_Key(newContext, newLoader.Raw_GetNewKey()),
									OffsetInBuffer = (int)ids.Ids[i].StringOffset,
									IDString = ids.Ids[i].Name,
									Text = strings.Strings.FirstOrDefault(str => (str.Offset - strings.Offset) == ids.Ids[i].StringOffset)?.String ?? "",
									CommentLength = 4,
									Comments = new string[0]
								};
							}
						}
					}
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
