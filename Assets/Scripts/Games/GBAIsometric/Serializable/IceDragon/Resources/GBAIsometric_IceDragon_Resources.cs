using System;
using BinarySerializer;
using BinarySerializer.GBA;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_IceDragon_Resources : BinarySerializable
    {
        public GBAIsometric_IceDragon_ResourceEntry[] DataEntries { get; set; }

        public static void DoAtResource(SerializerObject s, GBAIsometric_IceDragon_CompressionType compressionType, Action<long> action, long size = -1)
        {
            switch (compressionType)
            {
                case GBAIsometric_IceDragon_CompressionType.None:
                    action(size);
                    break;

                case GBAIsometric_IceDragon_CompressionType.Huffman:
                    s.DoEncoded(new GBA_Huffman4Encoder(), () => action(s.CurrentLength)); break;

                case GBAIsometric_IceDragon_CompressionType.LZSS:
                    s.DoEncoded(new GBA_LZSSEncoder(), () => action(s.CurrentLength));
                    break;

                // RL encoding is never used by any of the games
                case GBAIsometric_IceDragon_CompressionType.RL:
                    throw new NotImplementedException("RL encoding is not implemented");

                default:
                    throw new ArgumentOutOfRangeException(nameof(compressionType), compressionType, null);
            }
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