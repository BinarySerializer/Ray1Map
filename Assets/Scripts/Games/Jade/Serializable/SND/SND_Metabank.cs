using System;
using BinarySerializer;

namespace Ray1Map.Jade {
    public class SND_Metabank : Jade_File {
		public override string Export_Extension => "snk";

		public uint Count { get; set; }
        public SoundRef[] References { get; set; }

        protected override void SerializeFile(SerializerObject s) {

            var endPtr = Offset + FileSize;
            References = s.SerializeObjectArrayUntil<SoundRef>(References,
                sr => s.CurrentAbsoluteOffset >= endPtr.AbsoluteOffset,
                name: nameof(References));
            Count = (uint)(References?.Length ?? 0);
            if (Count > 0) {
                // Choose a key. Either the first valid one or the one with the current language
                var firstValidRef = References.FindItem(r => !r.IsNull);
                firstValidRef?.Resolve();
            }
        }


        public class SoundRef : BinarySerializable {
            public Jade_GenericReference ReferenceEditor { get; set; }
            public Jade_Reference<SND_Bank> ReferenceBGE { get; set; }
            public bool IsNull => ReferenceBGE?.IsNull ?? ReferenceEditor?.IsNull ?? false;

            public override void SerializeImpl(SerializerObject s) {
                LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong) || !Loader.IsBinaryData) {
                    ReferenceEditor = s.SerializeObject<Jade_GenericReference>(ReferenceEditor, name: nameof(ReferenceEditor));
                } else {
                    ReferenceBGE = s.SerializeObject<Jade_Reference<SND_Bank>>(ReferenceBGE, name: nameof(ReferenceBGE));
                }
            }

            public void Resolve() {
                if (ReferenceBGE != null) {
                    ReferenceBGE.Resolve(immediate: true, flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount
                        | LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile);
                } else {
                    ReferenceEditor.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
                }
            }
        }
    }
}