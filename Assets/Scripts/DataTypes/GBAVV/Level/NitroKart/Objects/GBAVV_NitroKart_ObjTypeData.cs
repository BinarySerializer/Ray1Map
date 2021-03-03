namespace R1Engine
{
    public class GBAVV_NitroKart_ObjTypeData : R1Serializable
    {
        public int ObjType { get; set; }
        public int Int_04 { get; set; }
        public int Int_08 { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }
        public Pointer GraphicsDataPointer { get; set; }
        public int AnimSetIndex { get; set; }
        public int Int_1C { get; set; }
        public Pointer AnimationIndicesPointer { get; set; }
        public int AnimationIndicesCount { get; set; }
        public int Int_28 { get; set; }
        public int Int_2C { get; set; }
        public int Int_30 { get; set; }

        // Serialized from pointers
        public GBAVV_Map2D_Graphics GraphicsData { get; set; }
        public int[] AnimationIndices { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType = s.Serialize<int>(ObjType, name: nameof(ObjType));
            Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            GraphicsDataPointer = s.SerializePointer(GraphicsDataPointer, name: nameof(GraphicsDataPointer));
            AnimSetIndex = s.Serialize<int>(AnimSetIndex, name: nameof(AnimSetIndex));
            Int_1C = s.Serialize<int>(Int_1C, name: nameof(Int_1C));
            AnimationIndicesPointer = s.SerializePointer(AnimationIndicesPointer, name: nameof(AnimationIndicesPointer));
            AnimationIndicesCount = s.Serialize<int>(AnimationIndicesCount, name: nameof(AnimationIndicesCount));
            Int_28 = s.Serialize<int>(Int_28, name: nameof(Int_28));
            Int_2C = s.Serialize<int>(Int_2C, name: nameof(Int_2C));
            Int_30 = s.Serialize<int>(Int_30, name: nameof(Int_30));

            // TODO: Some objects have additional data

            GraphicsData = s.DoAt(GraphicsDataPointer, () => s.SerializeObject<GBAVV_Map2D_Graphics>(GraphicsData, name: nameof(GraphicsData)));
            AnimationIndices = s.DoAt(AnimationIndicesPointer, () => s.SerializeArray<int>(AnimationIndices, AnimationIndicesCount, name: nameof(AnimationIndices)));
        }
    }
}