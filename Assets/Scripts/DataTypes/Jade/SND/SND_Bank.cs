using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class SND_Bank : Jade_File {
        public uint Count { get; set; }
        public Jade_GenericReference[] References { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Count = FileSize / 8;
            if (s.GetR1Settings().Game == Game.Jade_BGE && Loader.IsBinaryData) {
                Count = s.Serialize<uint>(Count, name: nameof(Count));
            }
            if (s.GetR1Settings().Game == Game.Jade_BGE) {
                References = new Jade_GenericReference[Count];
                for (int i = 0; i < References.Length; i++) {
                    References[i] = s.SerializeObject<Jade_GenericReference>(References[i], name: $"{nameof(References)}[{i}]");
                    if (References[i].Type == Jade_FileType.FileType.SND_SModifier) {
                        References[i].ResolveEmbedded(s, flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
                    } /*else {
                        References[i].Resolve(queue: LOA_Loader.QueueType.Sound, flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
                    }*/
                }
            } else {
                References = s.SerializeObjectArray<Jade_GenericReference>(References, Count, name: nameof(References));
                foreach (var reference in References) {
                    reference.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
                }
            }
        }
    }
}