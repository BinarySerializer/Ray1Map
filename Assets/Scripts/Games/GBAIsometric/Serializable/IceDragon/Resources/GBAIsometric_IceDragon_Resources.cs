using System;
using BinarySerializer;
using BinarySerializer.Nintendo;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_IceDragon_Resources : BinarySerializable
    {
        public GBAIsometric_IceDragon_ResourceEntry[] DataEntries { get; set; }

        public static void DoAtResource(SerializerObject s, GBAIsometric_IceDragon_CompressionType compressionType, Action<long> action, long size = -1)
        {
            IStreamEncoder encoder = compressionType switch
            {
                GBAIsometric_IceDragon_CompressionType.None => null,
                GBAIsometric_IceDragon_CompressionType.Huffman => new GBA_HuffmanEncoder(),
                GBAIsometric_IceDragon_CompressionType.LZSS => new GBA_LZSSEncoder(),
                GBAIsometric_IceDragon_CompressionType.RL => new GBA_RLEEncoder(),
                _ => throw new ArgumentOutOfRangeException(nameof(compressionType), compressionType, null)
            };

            s.DoEncodedIf(encoder, encoder != null, () => action(encoder != null ? s.CurrentLength : size));
        }

        public void DoAtResource(Context context, long index, Action<long> action)
        {
            SerializerObject s = context.Deserializer;
            GBAIsometric_IceDragon_ResourceEntry entry = DataEntries[index];
            s.DoAt(entry.DataPointer, () => DoAtResource(s, entry.Compression, action, entry.DataLength));
        }

        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetSettings<GBAIsometricSettings>();

            DataEntries = s.SerializeObjectArray<GBAIsometric_IceDragon_ResourceEntry>(DataEntries, settings.ResourcesCount, name: nameof(DataEntries));
        }
    }
}