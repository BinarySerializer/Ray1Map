using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GEN_ModifierSoundFx : MDF_Modifier {
        public uint Version { get; set; }
        public uint Flags { get; set; }
        public float[] Distance { get; set; }

        public float Float_08 { get; set; }
        public float Float_0C { get; set; }
        public float Float_10 { get; set; }
        public float Delta { get; set; }
        public int CoreID { get; set; }
        public int Mode { get; set; }
        public int Delay { get; set; }
        public uint Feedback { get; set; }
        public float WetVol { get; set; }
        public float[] Far { get; set; }
        public uint NetIdx { get; set; }
        public uint NextPointer { get; set; }
        public byte[] Reserved { get; set; }

        public uint BGE_Flags { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
                BGE_Flags = s.Serialize<uint>(BGE_Flags, name: nameof(BGE_Flags));
                if (BGE_Flags == 0x100) {
                    Version = s.Serialize<uint>(Version, name: nameof(Version));
                    Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
                    Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
                    Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
                    CoreID = s.Serialize<int>(CoreID, name: nameof(CoreID));
                    Mode = s.Serialize<int>(Mode, name: nameof(Mode));
                    Delay = s.Serialize<int>(Delay, name: nameof(Delay));
                    WetVol = s.Serialize<float>(WetVol, name: nameof(WetVol));
                    NetIdx = s.Serialize<uint>(NetIdx, name: nameof(NetIdx));
                    if (!Loader.IsBinaryData) Reserved = s.SerializeArray(Reserved, 0x100, name: nameof(Reserved));
                }
            } else {
                Version = s.Serialize<uint>(Version, name: nameof(Version));
                Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
                
                Distance = s.SerializeArray<float>(Distance, 3, name: nameof(Distance));
                Delta = s.Serialize<float>(Delta, name: nameof(Delta));

                CoreID = s.Serialize<int>(CoreID, name: nameof(CoreID));
                Mode = s.Serialize<int>(Mode, name: nameof(Mode));
                Delay = s.Serialize<int>(Delay, name: nameof(Delay));
                Feedback = s.Serialize<uint>(Feedback, name: nameof(Feedback));

                WetVol = s.Serialize<float>(WetVol, name: nameof(WetVol));
                Far = s.SerializeArray<float>(Far, 3, name: nameof(Far));

                NetIdx = s.Serialize<uint>(NetIdx, name: nameof(NetIdx));

                if (!Loader.IsBinaryData) {
                    NextPointer = s.Serialize<uint>(NextPointer, name: nameof(NextPointer));
                    Reserved = s.SerializeArray(Reserved, 48, name: nameof(Reserved));
                }
            }
        }
    }
}
