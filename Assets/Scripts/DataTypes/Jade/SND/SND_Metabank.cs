using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class SND_Metabank : Jade_File {
        public Jade_Reference<SND_Bank>[] References { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            References = s.SerializeObjectArray<Jade_Reference<SND_Bank>>(References, FileSize / 4, name: nameof(References));
            if (References.Length > 0) {
                // Choose a key. Either the first valid one or the one with the current language
                var firstValidRef = References.FindItem(r => !r.IsNull);

                if (s.GetR1Settings().EngineVersion < EngineVersion.Jade_KingKong) {
                    firstValidRef.Resolve(immediate: true, flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount
                        | LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.DontUseCachedFile);
                } else {
                    firstValidRef.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
                }
            }
        }
    }
}