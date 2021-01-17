namespace R1Engine
{
    public class Gameloft_ResourceFile : R1Serializable
    {
        public uint ResourcesCount { get; set; }
        public ushort OffsetsCount { get; set; }
        public uint[] Offsets { get; set; }
        public Pointer StartPointer { get; set; }
        public byte Byte_RK { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion >= EngineVersion.Gameloft_RK) {
                ResourcesCount = s.Serialize<uint>(ResourcesCount, name: nameof(ResourcesCount));
                OffsetsCount = (ushort)ResourcesCount;
            } else {
                OffsetsCount = s.Serialize<ushort>(OffsetsCount, name: nameof(OffsetsCount));
                ResourcesCount = (uint)(OffsetsCount - 1);
            }
            if (ResourcesCount > 256) {
                throw new System.Exception($"File {Offset.file.filePath} is not a valid resource file ({ResourcesCount} resources)!");
            }
            Offsets = s.SerializeArray<uint>(Offsets, OffsetsCount, name: nameof(Offsets));
            StartPointer = s.CurrentPointer;
            /*if (Offsets.Length > 0 && StartPointer.FileOffset + Offsets[Offsets.Length - 1] != s.CurrentLength) {
                throw new System.Exception($"File {Offset.file.filePath} is not a valid resource file!");
            }*/
        }

        public T SerializeResource<T>(SerializerObject s, T t, int i, string name) where T : Gameloft_Resource, new() {
            if (i >= ResourcesCount) {
                throw new System.Exception($"{typeof(T)}: Resource number {i} too high for file {Offset.file.filePath} (max {ResourcesCount})");
            }
            Pointer startOffset = StartPointer;
            uint size = 0;
            if (s.GameSettings.EngineVersion >= EngineVersion.Gameloft_RK) {
                if (i > 0) startOffset += Offsets[i - 1];
                size = Offsets[i] - (i > 0 ? Offsets[i - 1] : 0);
                startOffset += 1;
                size -= 1;
            } else {
                startOffset += Offsets[i];
                size = Offsets[i+1] - Offsets[i];
            }
            s.DoAt(startOffset, () => {
                if (startOffset.FileOffset + size > s.CurrentLength) size = s.CurrentLength - startOffset.FileOffset;
                t = s.SerializeObject<T>(t, onPreSerialize: obj => obj.ResourceSize = size, name: name);
                if (s.CurrentPointer != startOffset + size) {
                    UnityEngine.Debug.LogWarning($"{typeof(T)} @ {startOffset} - Resource {i}: Serialized size: {(s.CurrentPointer - startOffset)} != ResourceSize: {size}");
                }
            });
            return t;
        }
    }
}