using System;
using BinarySerializer;
using Ray1Map;

namespace Ray1Map.Gameloft
{
    public class Gameloft_ResourceFile : BinarySerializable
    {
        public uint ResourcesCount { get; set; }
        public ushort OffsetsCount { get; set; }
        public uint[] Offsets { get; set; }
        public Pointer StartPointer { get; set; }
        public uint TotalSize { get; set; }
        public byte Byte_RK { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion >= EngineVersion.Gameloft_RK) {
                ResourcesCount = s.Serialize<uint>(ResourcesCount, name: nameof(ResourcesCount));
                OffsetsCount = (ushort)ResourcesCount;
            } else {
                OffsetsCount = s.Serialize<ushort>(OffsetsCount, name: nameof(OffsetsCount));
                ResourcesCount = (uint)(OffsetsCount - 1);
            }
            if (ResourcesCount > 256) {
                throw new System.Exception($"File {Offset.File.FilePath} is not a valid resource file ({ResourcesCount} resources)!");
            }
            Offsets = s.SerializeArray<uint>(Offsets, OffsetsCount, name: nameof(Offsets));
            StartPointer = s.CurrentPointer;
            TotalSize = s.CurrentLength32;
            /*if (Offsets.Length > 0 && StartPointer.FileOffset + Offsets[Offsets.Length - 1] != s.CurrentLength) {
                throw new System.Exception($"File {Offset.file.filePath} is not a valid resource file!");
            }*/
        }
        public int GetSizeOfResource(int i) {
            if (i >= ResourcesCount) {
                return 0;
            }
            Pointer startOffset = StartPointer;
            int size = 0;
            if (Context.GetR1Settings().EngineVersion >= EngineVersion.Gameloft_RK) {
                if (i > 0) startOffset += Offsets[i - 1];
                size = (int)(Offsets[i] - (i > 0 ? Offsets[i - 1] : 0));
                startOffset += 1;
                if (size > 0) size -= 1;
            } else {
                startOffset += Offsets[i];
                size = (int)(Offsets[i + 1] - Offsets[i]);
            }
            if (startOffset.FileOffset + size > TotalSize) size = (int)(TotalSize - startOffset.FileOffset);
            return size;
        }

        public T SerializeResource<T>(SerializerObject s, T t, int i, Action<T> onPreSerialize = null, string name = null) where T : Gameloft_Resource, new() {
            if (i >= ResourcesCount) {
                throw new System.Exception($"{typeof(T)}: Resource number {i} too high for file {Offset.File.FilePath} (max {ResourcesCount})");
            }
            Pointer startOffset = StartPointer;
            int size = 0;
            if (s.GetR1Settings().EngineVersion >= EngineVersion.Gameloft_RK) {
                if (i > 0) startOffset += Offsets[i - 1];
                size = (int)(Offsets[i] - (i > 0 ? Offsets[i - 1] : 0));
                startOffset += 1;
                if(size > 0) size -= 1;
            } else {
                startOffset += Offsets[i];
                size = (int)(Offsets[i+1] - Offsets[i]);
            }
            if (startOffset.FileOffset + size > TotalSize) size = (int)(TotalSize - startOffset.FileOffset);
            if (size <= 0) return null;
            s.DoAt(startOffset, () => {
                t = s.SerializeObject<T>(t, onPreSerialize: obj => {
                    obj.ResourceSize = (uint)size;
                    onPreSerialize?.Invoke(obj);
                }, name: name);
                if (s.CurrentPointer != startOffset + size) {
                    UnityEngine.Debug.LogWarning($"{typeof(T)} @ {startOffset} - Resource {i}: Serialized size: {(s.CurrentPointer - startOffset)}/{(s.CurrentPointer - startOffset):X8} != ResourceSize: {size}");
                }
            });
            return t;
        }
    }
}