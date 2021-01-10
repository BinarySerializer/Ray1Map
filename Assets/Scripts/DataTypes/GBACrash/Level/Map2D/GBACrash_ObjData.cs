namespace R1Engine
{
    public class GBACrash_ObjData : R1Serializable
    {
        public ushort Ushort_00 { get; set; }
        public ushort ObjGroupsCount { get; set; }
        public Pointer ObjGroupsPointer { get; set; }
        public Pointer Pointer_08 { get; set; }
        public Pointer Pointer_0C { get; set; }
        public Pointer Pointer_10 { get; set; }

        // Serialized from pointers
        
        public ObjGroup[] ObjGroups { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
            ObjGroupsCount = s.Serialize<ushort>(ObjGroupsCount, name: nameof(ObjGroupsCount));
            ObjGroupsPointer = s.SerializePointer(ObjGroupsPointer, name: nameof(ObjGroupsPointer));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
            Pointer_10 = s.SerializePointer(Pointer_10, name: nameof(Pointer_10));

            ObjGroups = s.DoAt(ObjGroupsPointer, () => s.SerializeObjectArray<ObjGroup>(ObjGroups, ObjGroupsCount, name: nameof(ObjGroups)));
        }

        public class ObjGroup : R1Serializable
        {
            public ushort Ushort_00 { get; set; }
            public ushort ObjectsCount { get; set; }
            public Pointer ObjectsPointer { get; set; }

            // Serialized from pointers
            public Object[] Objects { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
                ObjectsCount = s.Serialize<ushort>(ObjectsCount, name: nameof(ObjectsCount));
                ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));

                Objects = s.DoAt(ObjectsPointer, () => s.SerializeObjectArray<Object>(Objects, ObjectsCount, name: nameof(Objects)));
            }

            public class Object : R1Serializable
            {
                public ObjectType ObjType { get; set; }
                public short XPos { get; set; }
                public short YPos { get; set; }
                public ushort Ushort_06 { get; set; }

                public override void SerializeImpl(SerializerObject s)
                {
                    ObjType = s.Serialize<ObjectType>(ObjType, name: nameof(ObjType));
                    XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                    YPos = s.Serialize<short>(YPos, name: nameof(YPos));
                    Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
                }

                public enum ObjectType : short
                {
                    Crash = 0,

                    Type_1 = 1,
                    
                    Wumpa = 2,

                    Type_3 = 3,
                    Type_4 = 4,
                    Type_5 = 5,
                    Type_6 = 6,
                    Type_7 = 7,
                    Type_8 = 8,
                    
                    TimeTrialClock = 9,

                    Type_10 = 10,
                    Type_11 = 11,
                    Type_12 = 12,
                    
                    Crate_Normal = 13,
                    Crate_Checkpoint = 14,
                    Crate_AkuAku = 15,

                    Type_16 = 16,
                    Type_17 = 17,
                    Type_18 = 18,

                    Crate_NitroSwitch = 19,
                    Crate_Iron = 20,
                    Crate_IronUp = 21,
                    Crate_Life = 22,
                    Crate_Nitro = 23,
                    Crate_QuestionMark = 24,
                    Crate_Bounce = 25,
                    Crate_Locked = 26,
                    Crate_TNT = 27,
                    Crate_Slot = 28,
                    
                    Type_29 = 29,
                    Type_30 = 30,
                    Type_31 = 31,
                    Type_32 = 32,
                    Type_33 = 33,
                    Type_34 = 34,
                    Type_35 = 35,
                    Type_36 = 36,
                    Type_37 = 37,
                    LevelExit = 38,
                    Type_39 = 39,
                    Type_40 = 40,
                    Type_41 = 41,
                    Type_42 = 42,
                    Type_43 = 43,
                    Type_44 = 44,
                    Type_45 = 45,
                    Type_46 = 46,
                    Type_47 = 47,
                    Type_48 = 48,
                    Type_49 = 49,
                    Type_50 = 50,
                    Type_51 = 51,
                    Type_52 = 52,
                    Type_53 = 53,
                    Type_54 = 54,
                    Type_55 = 55,
                    Type_56 = 56,
                    Type_57 = 57,
                    Type_58 = 58,
                    Type_59 = 59,
                    Type_60 = 60,
                    Type_61 = 61,
                    Type_62 = 62,
                    Type_63 = 63,
                    Type_64 = 64,
                    Type_65 = 65,
                    Type_66 = 66,
                    Type_67 = 67,
                    Type_68 = 68,
                    Type_69 = 69,
                    Type_70 = 70,
                    Type_71 = 71,
                    Type_72 = 72,
                    Type_73 = 73,
                    Type_74 = 74,
                    Type_75 = 75,
                    Type_76 = 76,
                    Type_77 = 77,
                    Type_78 = 78,
                    Type_79 = 79,
                    Type_80 = 80,
                    Type_81 = 81,
                    Type_82 = 82,
                    Type_83 = 83,
                    Type_84 = 84,
                    Type_85 = 85,
                    Type_86 = 86,
                    Type_87 = 87,
                    Type_88 = 88,
                    Type_89 = 89,
                    Type_90 = 90,
                    Type_91 = 91,
                    Type_92 = 92,
                    Type_93 = 93,
                    Type_94 = 94,
                    Type_95 = 95,
                    Type_96 = 96,
                    Type_97 = 97,
                    Type_98 = 98,
                    Type_99 = 99,
                    Type_100 = 100,
                    Type_101 = 101,
                    Type_102 = 102,
                    Type_103 = 103,
                    Type_104 = 104,

                    TutorialMessage = 105,
                    
                    Type_106 = 106,
                    Type_107 = 107,
                    Type_108 = 108,
                    Type_109 = 109,
                }
            }
        }
    }
}