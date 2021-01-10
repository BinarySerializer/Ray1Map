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
        
        public GBACrash_ObjGroups[] ObjGroups { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
            ObjGroupsCount = s.Serialize<ushort>(ObjGroupsCount, name: nameof(ObjGroupsCount));
            ObjGroupsPointer = s.SerializePointer(ObjGroupsPointer, name: nameof(ObjGroupsPointer));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
            Pointer_10 = s.SerializePointer(Pointer_10, name: nameof(Pointer_10));

            ObjGroups = s.DoAt(ObjGroupsPointer, () => s.SerializeObjectArray<GBACrash_ObjGroups>(ObjGroups, ObjGroupsCount, name: nameof(ObjGroups)));
        }
    }
}