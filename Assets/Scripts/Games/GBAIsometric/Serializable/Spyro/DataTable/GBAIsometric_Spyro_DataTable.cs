using System;
using BinarySerializer;
using BinarySerializer.GBA;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_DataTable : BinarySerializable
    {
        public GBAIsometric_Spyro_DataTableEntry[] DataEntries { get; set; }

        public static void DoAtData(SerializerObject s, GBAIsometric_Spyro_CompressionType compressionType, Action<long> action, long size = -1)
        {
            switch (compressionType)
            {
                case GBAIsometric_Spyro_CompressionType.None:
                    action(size);
                    break;

                case GBAIsometric_Spyro_CompressionType.Huffman:
                    s.DoEncoded(new GBA_Huffman4Encoder(), () => action(s.CurrentLength)); break;

                case GBAIsometric_Spyro_CompressionType.LZSS:
                    s.DoEncoded(new GBA_LZSSEncoder(), () => action(s.CurrentLength));
                    break;

                // RL encoding is never used by any of the games
                case GBAIsometric_Spyro_CompressionType.RL:
                    throw new NotImplementedException("RL encoding is not implemented");

                default:
                    throw new ArgumentOutOfRangeException(nameof(compressionType), compressionType, null);
            }
        }

        public void DoAtBlock(Context context, long index, Action<long> action)
        {
            SerializerObject s = context.Deserializer;
            GBAIsometric_Spyro_DataTableEntry entry = DataEntries[index];
            s.DoAt(entry.DataPointer, () => DoAtData(s, entry.Compression, action, entry.DataLength));
        }

        public override void SerializeImpl(SerializerObject s)
        {
            var manager = (GBAIsometric_Spyro_Manager)s.GetR1Settings().GetGameManager;

            DataEntries = s.SerializeObjectArray<GBAIsometric_Spyro_DataTableEntry>(DataEntries, manager.DataTableCount, name: nameof(DataEntries));
        }
    }
}