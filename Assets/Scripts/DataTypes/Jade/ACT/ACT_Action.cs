using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class ACT_Action : Jade_File 
    {
        public byte StructsCount { get; set; }
        public byte Byte_01 { get; set; }
        public ushort Ushort_02_Editor { get; set; }
        public ACT_ActionStruct1[] Structs1 { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            StructsCount = s.Serialize<byte>(StructsCount, name: nameof(StructsCount));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            if (!Loader.IsBinaryData) Ushort_02_Editor = s.Serialize<ushort>(Ushort_02_Editor, name: nameof(Ushort_02_Editor));

            Structs1 = s.SerializeObjectArray<ACT_ActionStruct1>(Structs1, StructsCount, name: nameof(Structs1));
            foreach (var struct1 in Structs1) {
                struct1.SerializeStruct2(s);
            }
        }

        public class ACT_ActionStruct1 : BinarySerializable
        {
            public Jade_Reference<EVE_ListTracks> ListTracks { get; set; }
            public uint Uint_04 { get; set; }
            public Jade_Reference<ANI_Shape> Shape { get; set; }
            public byte Byte_0C { get; set; }
            public byte Byte_0D { get; set; }
            public byte Byte_0E { get; set; }
            public byte Byte_0F { get; set; }
            public ushort Ushort_10 { get; set; }
            public byte Byte_12 { get; set; }
            public byte Byte_13_Editor { get; set; }

            public ACT_ActionStruct2 Struct2 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
			    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                ListTracks = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(ListTracks, name: nameof(ListTracks))?.Resolve();
                Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
                Shape = s.SerializeObject<Jade_Reference<ANI_Shape>>(Shape, name: nameof(Shape))?.Resolve();
                Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
                Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
                Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
                Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));
                Ushort_10 = s.Serialize<ushort>(Ushort_10, name: nameof(Ushort_10));
                Byte_12 = s.Serialize<byte>(Byte_12, name: nameof(Byte_12));
                if (!Loader.IsBinaryData) Byte_13_Editor = s.Serialize<byte>(Byte_13_Editor, name: nameof(Byte_13_Editor));
            }

            public void SerializeStruct2(SerializerObject s) {
                if (Uint_04 != 0) {
                    Struct2 = s.SerializeObject<ACT_ActionStruct2>(Struct2, name: nameof(Struct2));
                }
            }
        }
        public class ACT_ActionStruct2 : BinarySerializable {
            public uint UInt_00_Editor { get; set; }
            public uint Count { get; set; }
            public uint UInt_04_Editor { get; set; }
            public uint UInt_04 { get; set; }

            public Unk0[] Unk0s { get; set; }
            public Unk1[] Unk1s { get; set; }

            public override void SerializeImpl(SerializerObject s) {
			    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                if(!Loader.IsBinaryData) UInt_00_Editor = s.Serialize<uint>(UInt_00_Editor, name: nameof(UInt_00_Editor));
                Count = s.Serialize<uint>(Count, name: nameof(Count));
                if (!Loader.IsBinaryData) UInt_04_Editor = s.Serialize<uint>(UInt_04_Editor, name: nameof(UInt_04_Editor));
                UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));

                Unk0s = s.SerializeObjectArray<Unk0>(Unk0s, Count, name: nameof(Unk0s));
                Unk1s = s.SerializeObjectArray<Unk1>(Unk1s, Count, name: nameof(Unk1s));
            }

			public class Unk0 : BinarySerializable {
                public uint UInt_00 { get; set; }
                public uint UInt_04_Editor { get; set; }
				public override void SerializeImpl(SerializerObject s) {
                    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                    UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
                    if (!Loader.IsBinaryData) UInt_04_Editor = s.Serialize<uint>(UInt_04_Editor, name: nameof(UInt_04_Editor));
                }
            }

            public class Unk1 : BinarySerializable {
                public ushort UShort_00 { get; set; }
                public byte Byte_02 { get; set; }
                public byte Byte_03 { get; set; }
                public override void SerializeImpl(SerializerObject s) {
                    UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
                    Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
                    Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
                }
            }
        }
    }
}