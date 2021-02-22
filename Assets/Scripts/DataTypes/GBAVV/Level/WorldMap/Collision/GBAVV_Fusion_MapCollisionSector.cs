namespace R1Engine
{
    public class GBAVV_Fusion_MapCollisionSector : R1Serializable
    {
        public int Int_00 { get; set; } // X?
        public int Int_04 { get; set; } // Y?
        public int Int_08 { get; set; } // Width?
        public int Int_0C { get; set; } // Height?
        public int ItemsCount { get; set; }
        public Pointer CollisionLinesPointer { get; set; }
        public Pointer[] MapCollisionPointers { get; set; }

        // Serialized from pointers
        public GBAVV_Fusion_MapCollisionLine[] CollisionLines { get; set; }
        public GBAVV_Fusion_MapCollisionSector[] SubSectors { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
            ItemsCount = s.Serialize<int>(ItemsCount, name: nameof(ItemsCount));
            CollisionLinesPointer = s.SerializePointer(CollisionLinesPointer, name: nameof(CollisionLinesPointer));
            MapCollisionPointers = s.SerializePointerArray(MapCollisionPointers, 4, name: nameof(MapCollisionPointers));

            CollisionLines = s.DoAt(CollisionLinesPointer, () => s.SerializeObjectArray<GBAVV_Fusion_MapCollisionLine>(CollisionLines, ItemsCount, name: nameof(CollisionLines)));

            if (SubSectors == null)
                SubSectors = new GBAVV_Fusion_MapCollisionSector[MapCollisionPointers.Length];

            for (int i = 0; i < SubSectors.Length; i++)
                SubSectors[i] = s.DoAt(MapCollisionPointers[i], () => s.SerializeObject<GBAVV_Fusion_MapCollisionSector>(SubSectors[i], name: $"{nameof(SubSectors)}[{i}]"));
        }
    }
}