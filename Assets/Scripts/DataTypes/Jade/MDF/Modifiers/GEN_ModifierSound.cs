using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class GEN_ModifierSound : MDF_Modifier 
    {
        public uint Uint_00 { get; set; }
        public uint Uint_04_Editor { get; set; }
        public uint Uint_04 { get; set; }
        public Jade_Reference<SND_SModifier> SModifier { get; set; }
        public uint Uint_0C_Editor { get; set; }
        public uint Uint_10_Editor { get; set; }
        public float Float_0C { get; set; }
        public uint Uint_10 { get; set; }
        public uint Uint_14 { get; set; }
        public float Float_18 { get; set; }
        public float Float_1C { get; set; }
        public int Int_20 { get; set; }
        public byte[] Bytes_24_Editor { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
            if (s.GetR1Settings().Game == Game.Jade_BGE) {
                throw new NotImplementedException($"TODO: Implement {GetType()} for BG&E");
            } else if (Uint_00 == 274) {
                if (!Loader.IsBinaryData) Uint_04_Editor = s.Serialize<uint>(Uint_04_Editor, name: nameof(Uint_04_Editor));

                Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
                SModifier = s.SerializeObject<Jade_Reference<SND_SModifier>>(SModifier, name: nameof(SModifier))?
                    .Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);

                if (!Loader.IsBinaryData)
                {
                    Uint_0C_Editor = s.Serialize<uint>(Uint_0C_Editor, name: nameof(Uint_0C_Editor));
                    Uint_10_Editor = s.Serialize<uint>(Uint_10_Editor, name: nameof(Uint_10_Editor));
                }

                Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
                Uint_10 = s.Serialize<uint>(Uint_10, name: nameof(Uint_10));
                Uint_14 = s.Serialize<uint>(Uint_14, name: nameof(Uint_14));
                Float_18 = s.Serialize<float>(Float_18, name: nameof(Float_18));
                Float_1C = s.Serialize<float>(Float_1C, name: nameof(Float_1C));
                Int_20 = s.Serialize<int>(Int_20, name: nameof(Int_20));

                if (!Loader.IsBinaryData) Bytes_24_Editor = s.SerializeArray(Bytes_24_Editor, 40, name: nameof(Bytes_24_Editor));
            }
        }
    }
}
