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
        public ACT_ActionStruct2[] Structs2 { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            StructsCount = s.Serialize<byte>(StructsCount, name: nameof(StructsCount));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            if (!Loader.IsBinaryData) Ushort_02_Editor = s.Serialize<ushort>(Ushort_02_Editor, name: nameof(Ushort_02_Editor));

            Structs1 = s.SerializeObjectArray<ACT_ActionStruct1>(Structs1, StructsCount, name: nameof(Structs1));
            //Structs2 = s.SerializeObjectArray<ACT_ActionStruct2>(Structs2, StructsCount, name: nameof(Structs2));
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

            public override void SerializeImpl(SerializerObject s)
            {
			    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                ListTracks = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(ListTracks, name: nameof(ListTracks));
                Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
                Shape = s.SerializeObject<Jade_Reference<ANI_Shape>>(Shape, name: nameof(Shape));
                Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
                Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
                Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
                Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));
                Ushort_10 = s.Serialize<ushort>(Ushort_10, name: nameof(Ushort_10));
                Byte_12 = s.Serialize<byte>(Byte_12, name: nameof(Byte_12));
                if (!Loader.IsBinaryData) Byte_13_Editor = s.Serialize<byte>(Byte_13_Editor, name: nameof(Byte_13_Editor));
            }
        }
        public class ACT_ActionStruct2 : BinarySerializable
        {


            public override void SerializeImpl(SerializerObject s)
            {
			    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                throw new NotImplementedException($"TODO: Implement {GetType()}");
            }
        }
    }
}