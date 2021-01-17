namespace R1Engine
{
    public class Gameloft_ResourceFile : R1Serializable
    {
        public uint ResourcesCount { get; set; }
        public uint[] Offsets { get; set; }
        public byte Byte_RK { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion >= EngineVersion.Gameloft_RK) {
                ResourcesCount = s.Serialize<uint>(ResourcesCount, name: nameof(ResourcesCount));
            } else {
                ResourcesCount = s.Serialize<ushort>((ushort)ResourcesCount, name: nameof(ResourcesCount));
            }
            Offsets = s.SerializeArray<uint>(Offsets, ResourcesCount, name: nameof(ResourcesCount));
            if (s.GameSettings.EngineVersion >= EngineVersion.Gameloft_RK) {
                Byte_RK = s.Serialize<byte>(Byte_RK, name: nameof(Byte_RK));
            }
        }

        public T SerializeResource<T>(SerializerObject s, T t, int i, string name) where T : Gameloft_Resource, new() {
            if (i >= ResourcesCount) {
                throw new System.Exception($"{typeof(T)}: Resource number {i} too high for file {Offset.file.filePath} (max {ResourcesCount})");
            }
            Pointer startOffset = Offset + Size;
            if(i > 0) startOffset += Offsets[i-1];
            uint size = Offsets[i] - (i > 0 ? Offsets[i-1] : 0);
            s.DoAt(startOffset, () => {
                t = s.SerializeObject<T>(t, onPreSerialize: obj => obj.ResourceSize = size, name: name);
                if (s.CurrentPointer != startOffset + size) {
                    UnityEngine.Debug.LogWarning($"{typeof(T)} @ {startOffset}: Serialized size: {(s.CurrentPointer - startOffset)} != ResourceSize: {size}");
                }
            });
            return t;
        }
    }
}