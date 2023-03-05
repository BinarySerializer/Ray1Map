
using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class Jade_TextReference : BinarySerializable, ISerializerShortLog {
		public Jade_Key Key { get; set; }
		public bool ForceResolve { get; set; }
		public bool IsNull => Key.IsNull;

		public Dictionary<int, TEXT_AllText> Text { get; set; } = new Dictionary<int, TEXT_AllText>();
		public Dictionary<int, TEXT_AllText> TextSound { get; set; } = new Dictionary<int, TEXT_AllText>();

		public override void SerializeImpl(SerializerObject s) {
			Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
		}

		public Jade_TextReference() { }
		public Jade_TextReference(Context c, Jade_Key key) {
			Context = c;
			Key = key;
		}
		
		// Dummy resolve method for now
		public Jade_TextReference Resolve() {
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (!loader.IsBinaryData) LoadEditor();
			return this;
		}

		public Jade_TextReference LoadEditor(
			Action<SerializerObject, TEXT_AllText> onPreSerialize = null,
			Action<SerializerObject, TEXT_AllText> onPostSerialize = null) {
			for (int languageID = 0; languageID < 32; languageID++) {
				if (Text.ContainsKey(languageID)) {
					LoadText(languageID, false, onPreSerialize, onPostSerialize);
				}
			}
			return this;
		}

		public TEXT_AllText GetTextForLanguage(int languageID, bool sound) {
			var textDict = sound ? TextSound : Text;
			return textDict.TryGetValue(languageID, out TEXT_AllText value) ? value : null;
		}

		public Jade_TextReference LoadText(int languageID, bool sound,
			Action<SerializerObject, TEXT_AllText> onPreSerialize = null,
			Action<SerializerObject, TEXT_AllText> onPostSerialize = null) {
			if (IsNull && !ForceResolve) return this;
			var textDict = sound ? TextSound : Text;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			loader.RequestFile(Key, GetTextForLanguage(languageID, sound), (s, configureAction) => {
				textDict[languageID] = s.SerializeObject<TEXT_AllText>(GetTextForLanguage(languageID, sound), onPreSerialize: f => {
					configureAction(f); onPreSerialize?.Invoke(s, f);
				}, name: sound ? nameof(TextSound) : nameof(Text));
				onPostSerialize?.Invoke(s, textDict[languageID]);
			}, (f) => {
				textDict[languageID] = f?.ConvertType<TEXT_AllText>();
			}, immediate: false,
			queue: LOA_Loader.QueueType.Current,
			cache: LOA_Loader.CacheType.Current,
			flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile,
			name: typeof(TEXT_AllText).Name);
			return this;
		}

		protected override void OnChangeContext(Context oldContext, Context newContext) {
			base.OnChangeContext(oldContext, newContext);
			var loader = newContext.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (!loader.IsBinaryData) {
				var firstLanguage = 0;
				for (int i = 0; i < 32; i++) {
					if (Text.ContainsKey(i)) {
						firstLanguage = i;
						break;
					}
				}
				if(!Text.ContainsKey(firstLanguage)) return;
				for (int i = firstLanguage + 1; i < 32; i++) {
					if(!Text.ContainsKey(i)) continue;
					Text[firstLanguage].MergeText(Text[i]);
					Text[i] = Text[firstLanguage];
				}
			}
		}

		public string ShortLog => Key.ToString();
	}
}
