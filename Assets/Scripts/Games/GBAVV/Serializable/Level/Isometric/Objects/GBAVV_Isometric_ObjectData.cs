using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Isometric_ObjectData : BinarySerializable
    {
        public bool SerializeData { get; set; } // Set before serializing
        public bool IsMultiplayer { get; set; } // Set before serializing

        public Pointer StartPositionsPointer { get; set; }
        public Pointer MultiplayerFlagsPointer { get; set; }
        public Pointer MultiplayerCrownsPointer { get; set; }
        public Pointer ObjectsPointer { get; set; }
        public Pointer TargetObjectsPointer { get; set; }

        // Serialized from pointers

        public GBAVV_Isometric_Position[] StartPositions { get; set; }
        public GBAVV_Isometric_MultiplayerFlag[] MultiplayerFlags { get; set; } // The game does * 0x3000 + 0x1800 on these positions
        public GBAVV_Isometric_MultiplayerCrown[] MultiplayerCrowns { get; set; } // The game does << 8 on these positions
        public GBAVV_Isometric_Object[] Objects { get; set; }
        public GBAVV_Isometric_TargetObject[] TargetObjects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StartPositionsPointer = s.SerializePointer(StartPositionsPointer, name: nameof(StartPositionsPointer));
            MultiplayerFlagsPointer = s.SerializePointer(MultiplayerFlagsPointer, name: nameof(MultiplayerFlagsPointer));
            MultiplayerCrownsPointer = s.SerializePointer(MultiplayerCrownsPointer, name: nameof(MultiplayerCrownsPointer));
            ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));
            TargetObjectsPointer = s.SerializePointer(TargetObjectsPointer, name: nameof(TargetObjectsPointer));

            if (!SerializeData)
                return;

            StartPositions = s.DoAt(StartPositionsPointer, () => s.SerializeObjectArray<GBAVV_Isometric_Position>(StartPositions, IsMultiplayer ? 2 : 1, name: nameof(StartPositions)));
            MultiplayerFlags = s.DoAt(MultiplayerFlagsPointer, 
                () => s.SerializeObjectArrayUntil<GBAVV_Isometric_MultiplayerFlag>(MultiplayerFlags, x => x.XPos == 0, getLastObjFunc: () => new GBAVV_Isometric_MultiplayerFlag(), name: nameof(MultiplayerFlags)));
            MultiplayerCrowns = s.DoAt(MultiplayerCrownsPointer, 
                () => s.SerializeObjectArrayUntil<GBAVV_Isometric_MultiplayerCrown>(MultiplayerCrowns, x => x.XPos == 0, getLastObjFunc: () => new GBAVV_Isometric_MultiplayerCrown(), name: nameof(MultiplayerCrowns)));
            Objects =  s.DoAt(ObjectsPointer, 
                () => s.SerializeObjectArrayUntil<GBAVV_Isometric_Object>(Objects, x => x.ObjType == GBAVV_Isometric_Object.GBAVV_Isometric_ObjType.Invalid, getLastObjFunc: () => new GBAVV_Isometric_Object(), name: nameof(Objects)));
            TargetObjects = s.DoAt(TargetObjectsPointer, 
                () => s.SerializeObjectArrayUntil<GBAVV_Isometric_TargetObject>(TargetObjects, x => x.ObjType == GBAVV_Isometric_TargetObject.GBAVV_Isometric_TargetObjType.Invalid, getLastObjFunc: () => new GBAVV_Isometric_TargetObject(), name: nameof(TargetObjects)));

        }
    }
}