namespace R1Engine
{
    public class GBACrash_Isometric_ObjectData : R1Serializable
    {
        public bool SerializeData { get; set; } // Set before serializing
        public bool IsMultiplayer { get; set; } // Set before serializing

        public Pointer StartPositionsPointer { get; set; }
        public Pointer MultiplayerFlagsPointer { get; set; }
        public Pointer MultiplayerCrownsPointer { get; set; }
        public Pointer ObjectsPointer { get; set; }
        public Pointer TargetObjectsPointer { get; set; }

        // Serialized from pointers

        public GBACrash_Isometric_Position[] StartPositions { get; set; }
        public GBACrash_Isometric_Position[] MultiplayerFlags { get; set; } // The game does * 0x3000 + 0x1800 on these positions
        public GBACrash_Isometric_Position[] MultiplayerCrowns { get; set; } // The game does << 8 on these positions
        public GBACrash_Isometric_Object[] Objects { get; set; }
        public GBACrash_Isometric_TargetObject[] TargetObjects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StartPositionsPointer = s.SerializePointer(StartPositionsPointer, name: nameof(StartPositionsPointer));
            MultiplayerFlagsPointer = s.SerializePointer(MultiplayerFlagsPointer, name: nameof(MultiplayerFlagsPointer));
            MultiplayerCrownsPointer = s.SerializePointer(MultiplayerCrownsPointer, name: nameof(MultiplayerCrownsPointer));
            ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));
            TargetObjectsPointer = s.SerializePointer(TargetObjectsPointer, name: nameof(TargetObjectsPointer));

            if (!SerializeData)
                return;

            StartPositions = s.DoAt(StartPositionsPointer, () => s.SerializeObjectArray<GBACrash_Isometric_Position>(StartPositions, IsMultiplayer ? 2 : 1, name: nameof(StartPositions)));
            MultiplayerFlags = s.DoAt(MultiplayerFlagsPointer, () => s.SerializeObjectArrayUntil<GBACrash_Isometric_Position>(MultiplayerFlags, x => x.XPos.Value == 0, name: nameof(MultiplayerFlags)));
            MultiplayerCrowns = s.DoAt(MultiplayerCrownsPointer, () => s.SerializeObjectArrayUntil<GBACrash_Isometric_Position>(MultiplayerCrowns, x => x.XPos.Value == 0, name: nameof(MultiplayerCrowns)));
            Objects =  s.DoAt(ObjectsPointer, () => s.SerializeObjectArrayUntil<GBACrash_Isometric_Object>(Objects, x => x.ObjType == GBACrash_Isometric_Object.GBACrash_Isometric_ObjType.Invalid, name: nameof(Objects)));
            TargetObjects = s.DoAt(TargetObjectsPointer, () => s.SerializeObjectArrayUntil<GBACrash_Isometric_TargetObject>(TargetObjects, x => x.ObjType == GBACrash_Isometric_TargetObject.GBACrash_Isometric_TargetObjType.Invalid, name: nameof(TargetObjects)));

        }
    }
}