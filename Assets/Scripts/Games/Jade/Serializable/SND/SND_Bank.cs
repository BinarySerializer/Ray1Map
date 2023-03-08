using BinarySerializer;

namespace Ray1Map.Jade {
    public class SND_Bank : Jade_File {
        public uint Count { get; set; }
        public SoundRef[] References { get; set; }
		public override string Export_Extension => "snk";

		protected override void SerializeFile(SerializerObject s) {
            Count = FileSize / 8;
            if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong) && Loader.IsBinaryData) {
                Count = s.Serialize<uint>(Count, name: nameof(Count));
            }
            var endPtr = Offset + FileSize;
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR2)) {
                References = s.SerializeObjectArrayUntil<SoundRef>(References,
                    sr => s.CurrentAbsoluteOffset >= endPtr.AbsoluteOffset,
                    name: nameof(References));
                Count = (uint)(References?.Length ?? 0);
            } else {
                References = s.SerializeObjectArray<SoundRef>(References, Count, name: nameof(References));
            }
        }

		public class SoundRef : BinarySerializable {
            public Jade_GenericReference Reference { get; set; }
            public bool RRR2_Bool { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                Reference = s.SerializeObject<Jade_GenericReference>(Reference, name: nameof(Reference));

                if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
                    if (Reference.Type == Jade_FileType.FileType.SND_SModifier) {
                        Reference.ResolveEmbedded(s, flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter);
                    } /*else {
                        References[i].Resolve(queue: LOA_Loader.QueueType.Sound, flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
                    }*/
                } else {
                    Reference.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter);

                    if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR2)) {
                        LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
                        if (!Reference.IsNull && Loader.IsBinaryData && IsSound) {
                            RRR2_Bool = s.Serialize<bool>(RRR2_Bool, name: nameof(RRR2_Bool));
                        }
                    }
                }
            }

            bool IsSound {
                get {
                    switch (Reference.Type) {
                        case Jade_FileType.FileType.SND_Ambience:
                        case Jade_FileType.FileType.SND_Dialog:
                        case Jade_FileType.FileType.SND_LoadingSound:
                        case Jade_FileType.FileType.SND_Music:
                        case Jade_FileType.FileType.SND_Sound:
                            return true;
                        default:
                            return false;
                    }
                }
            }
		}
	}
}