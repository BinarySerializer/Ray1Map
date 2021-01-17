namespace R1Engine
{
    public class GBACrash_Isometric_ObjectData : R1Serializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public Pointer Pointer_00 { get; set; } // 8 byte structs
        public Pointer Pointer_04 { get; set; }
        public Pointer Pointer_08 { get; set; }
        public Pointer ObjectsPointer { get; set; }
        public Pointer Pointer_10 { get; set; }

        // Serialized from pointers

        public GBACrash_Isometric_Object[] Objects { get; set; }

        // Most of these appear to be related to multiplayer data
        public Struct_04[] Structs_04 { get; set; }
        public Struct_08[] Structs_08 { get; set; }
        public Struct_10[] Structs_10 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pointer_00 = s.SerializePointer(Pointer_00, name: nameof(Pointer_00));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));
            Pointer_10 = s.SerializePointer(Pointer_10, name: nameof(Pointer_10));

            if (!SerializeData)
                return;

            Objects =  s.DoAt(ObjectsPointer, () => s.SerializeObjectArrayUntil<GBACrash_Isometric_Object>(Objects, x => x.ObjType == GBACrash_Isometric_Object.GBACrash_Isometric_ObjType.Invalid, name: nameof(Objects)));

            Structs_04 = s.DoAt(Pointer_04, () => s.SerializeObjectArrayUntil<Struct_04>(Structs_04, x => x.Int_00 == 0, name: nameof(Structs_04)));
            Structs_08 = s.DoAt(Pointer_08, () => s.SerializeObjectArrayUntil<Struct_08>(Structs_08, x => x.Int_00 == 0, name: nameof(Structs_08)));
            Structs_10 = s.DoAt(Pointer_10, () => s.SerializeObjectArrayUntil<Struct_10>(Structs_10, x => x.Int_00 == 0, name: nameof(Structs_10)));
        }

        public class Struct_04 : R1Serializable
        {
            public int Int_00 { get; set; }
            public int Int_04 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
                Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
            }
        }
        public class Struct_08 : R1Serializable
        {
            public int Int_00 { get; set; }
            public int Int_04 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
                Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
            }
        }
        public class Struct_10 : R1Serializable
        {
            public int Int_00 { get; set; }
            public int Int_04 { get; set; }
            public int Int_08 { get; set; }
            public int Int_0C { get; set; }
            public int Int_10 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
                Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
                Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
                Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
                Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            }
        }
    }
}