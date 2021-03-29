using System;

namespace R1Engine.Jade
{
    public class GAO_ModifierMPAG : MDF_Modifier
    {
        public int Int_00 { get; set; }
        public int Type { get; set; }

        // Generator struct
        public int Int_10 { get; set; }
        public byte Byte_14 { get; set; }
        public byte Byte_15 { get; set; }

        public float Float_28 { get; set; }
        public float Float_2C { get; set; }
        public float Float_30 { get; set; }
        
        public float Float_160 { get; set; }
        public float Float_164 { get; set; }
        public float Float_168 { get; set; }
        public float Float_16C { get; set; }
        public float Float_170 { get; set; }
        public float Float_174 { get; set; }
        public float Float_178 { get; set; }
        public float Float_17C { get; set; }
        public float Float_180 { get; set; }
        public float Float_184 { get; set; }
        public float Float_188 { get; set; }
        public float Float_18C { get; set; }
        public float Float_190 { get; set; }
        public float Float_194 { get; set; }
        public float Float_198 { get; set; }
        public float Float_19C { get; set; }
        public float Float_1A0 { get; set; }
        public float Float_1A4 { get; set; }
        public float Float_1A8 { get; set; }

        public Jade_Vector Vector_1AC { get; set; }

        public float Float_1B8 { get; set; }
        public float Float_1BC { get; set; }
        public float Float_1C0 { get; set; }
        public float Float_1C4 { get; set; }
        public float Float_1C8 { get; set; }
        public float Float_1CC { get; set; }
        public float Float_1D0 { get; set; }
        public float Float_1D4 { get; set; }
        public float Float_1D8 { get; set; }
        public float Float_1DC { get; set; }
        public float Float_1E0 { get; set; }
        public float Float_1E4 { get; set; }
        public uint Uint_1E8 { get; set; }

        public byte Byte_270 { get; set; }

        // Editor
        public byte Type0_Byte_03_Editor { get; set; }
        public uint Type0_Uint_18_Editor { get; set; }
        public uint Type0_Uint_1C_Editor { get; set; }
        public uint Type0_Uint_20_Editor { get; set; }
        public uint Type0_Uint_24_Editor { get; set; }
        public uint Type0_Uint_28_Editor { get; set; }
        public uint Type0_Uint_2C_Editor { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            Type = s.Serialize<int>(Type, name: nameof(Type));

            if (Type == 0)
            {
                Int_10 = s.Serialize<byte>((byte)Int_10, name: nameof(Int_10));
                Byte_14 = s.Serialize<byte>(Byte_14, name: nameof(Byte_14));
                Byte_15 = s.Serialize<byte>(Byte_15, name: nameof(Byte_15));

                if (!Loader.IsBinaryData) Type0_Byte_03_Editor = s.Serialize<byte>(Type0_Byte_03_Editor, name: nameof(Type0_Byte_03_Editor));

                Float_164 = s.Serialize<float>(Float_164, name: nameof(Float_164));
                Float_168 = s.Serialize<float>(Float_168, name: nameof(Float_168));
                Float_16C = s.Serialize<float>(Float_16C, name: nameof(Float_16C));

                Float_2C = s.Serialize<float>(Float_2C, name: nameof(Float_2C));
                Float_30 = s.Serialize<float>(Float_30, name: nameof(Float_30));

                if (!Loader.IsBinaryData)
                {
                    Type0_Uint_18_Editor = s.Serialize<uint>(Type0_Uint_18_Editor, name: nameof(Type0_Uint_18_Editor));
                    Type0_Uint_1C_Editor = s.Serialize<uint>(Type0_Uint_1C_Editor, name: nameof(Type0_Uint_1C_Editor));
                    Type0_Uint_20_Editor = s.Serialize<uint>(Type0_Uint_20_Editor, name: nameof(Type0_Uint_20_Editor));
                    Type0_Uint_24_Editor = s.Serialize<uint>(Type0_Uint_24_Editor, name: nameof(Type0_Uint_24_Editor));
                    Type0_Uint_28_Editor = s.Serialize<uint>(Type0_Uint_28_Editor, name: nameof(Type0_Uint_28_Editor));
                    Type0_Uint_2C_Editor = s.Serialize<uint>(Type0_Uint_2C_Editor, name: nameof(Type0_Uint_2C_Editor));
                }

                Float_170 = s.Serialize<float>(Float_170, name: nameof(Float_170));
                Float_174 = s.Serialize<float>(Float_174, name: nameof(Float_174));
                Float_178 = s.Serialize<float>(Float_178, name: nameof(Float_178));
                Float_17C = s.Serialize<float>(Float_17C, name: nameof(Float_17C));

                Float_180 = s.Serialize<float>(Float_180, name: nameof(Float_180));
                Float_184 = Float_180;
                Float_188 = s.Serialize<float>(Float_188, name: nameof(Float_188));
                Float_18C = Float_188;

                Float_190 = s.Serialize<float>(Float_190, name: nameof(Float_190));
                Float_194 = s.Serialize<float>(Float_194, name: nameof(Float_194));

                Vector_1AC = s.SerializeObject(Vector_1AC, name: nameof(Vector_1AC));
            }
            else if (Type == 1)
            {
                Int_10 = s.Serialize<byte>((byte)Int_10, name: nameof(Int_10));
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 2)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 3)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 4)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 5)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 6)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 7)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 8)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 9)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 10)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 11)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 12)
            {
                Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
                Byte_14 = s.Serialize<byte>(Byte_14, name: nameof(Byte_14));
                Byte_15 = s.Serialize<byte>(Byte_15, name: nameof(Byte_15));

                Float_160 = s.Serialize<float>(Float_160, name: nameof(Float_160));
                Float_164 = s.Serialize<float>(Float_164, name: nameof(Float_164));
                Float_168 = s.Serialize<float>(Float_168, name: nameof(Float_168));
                Float_16C = s.Serialize<float>(Float_16C, name: nameof(Float_16C));

                Float_2C = s.Serialize<float>(Float_2C, name: nameof(Float_2C));
                Float_28 = Float_2C;

                Float_170 = s.Serialize<float>(Float_170, name: nameof(Float_170));
                Float_174 = s.Serialize<float>(Float_174, name: nameof(Float_174));
                Float_178 = s.Serialize<float>(Float_178, name: nameof(Float_178));
                Float_17C = s.Serialize<float>(Float_17C, name: nameof(Float_17C));
                Float_180 = s.Serialize<float>(Float_180, name: nameof(Float_180));
                Float_184 = s.Serialize<float>(Float_184, name: nameof(Float_184));
                Float_188 = s.Serialize<float>(Float_188, name: nameof(Float_188));
                Float_18C = s.Serialize<float>(Float_18C, name: nameof(Float_18C));
                Float_190 = s.Serialize<float>(Float_190, name: nameof(Float_190));
                Float_194 = s.Serialize<float>(Float_194, name: nameof(Float_194));
                Float_198 = s.Serialize<float>(Float_198, name: nameof(Float_198));
                Float_19C = s.Serialize<float>(Float_19C, name: nameof(Float_19C));
                Float_1A0 = s.Serialize<float>(Float_1A0, name: nameof(Float_1A0));
                Float_1A4 = s.Serialize<float>(Float_1A4, name: nameof(Float_1A4));

                Vector_1AC = s.SerializeObject(Vector_1AC, name: nameof(Vector_1AC));

                Float_1B8 = s.Serialize<float>(Float_1B8, name: nameof(Float_1B8));
                Float_1A8 = s.Serialize<float>(Float_1A8, name: nameof(Float_1A8));

                Uint_1E8 = s.Serialize<uint>(Uint_1E8, name: nameof(Uint_1E8));

                Float_1BC = s.Serialize<float>(Float_1BC, name: nameof(Float_1BC));
                Float_1C0 = s.Serialize<float>(Float_1C0, name: nameof(Float_1C0));
                Float_1C4 = s.Serialize<float>(Float_1C4, name: nameof(Float_1C4));
                Float_1C8 = s.Serialize<float>(Float_1C8, name: nameof(Float_1C8));
                Float_1CC = s.Serialize<float>(Float_1CC, name: nameof(Float_1CC));
                Float_1D0 = s.Serialize<float>(Float_1D0, name: nameof(Float_1D0));
                Float_1D4 = s.Serialize<float>(Float_1D4, name: nameof(Float_1D4));
                Float_1D8 = s.Serialize<float>(Float_1D8, name: nameof(Float_1D8));
                Float_1DC = s.Serialize<float>(Float_1DC, name: nameof(Float_1DC));
                Float_1E0 = s.Serialize<float>(Float_1E0, name: nameof(Float_1E0));
                Float_1E4 = s.Serialize<float>(Float_1E4, name: nameof(Float_1E4));

                Byte_270 = s.Serialize<byte>(Byte_270, name: nameof(Byte_270));
            }
        }
    }
}