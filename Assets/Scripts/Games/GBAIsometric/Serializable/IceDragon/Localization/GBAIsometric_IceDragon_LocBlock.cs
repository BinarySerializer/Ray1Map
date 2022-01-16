﻿using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_IceDragon_LocBlock : BinarySerializable
    {
        public int Pre_Length { get; set; }
        public GBAIsometric_IceDragon_LocDecompress[] Pre_DecompressHelpers { get; set; }

        public ushort[] StringOffsets { get; set; }
        public byte[][] StringTileIndices { get; set; }
        public string[] Strings { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if ((s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro1 ||
                 s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2) && StringOffsets == null)
            {
                var firstOffset = s.DoAt(s.CurrentPointer, () => s.Serialize<ushort>(default, name: $"{nameof(StringOffsets)}[0]"));
                Pre_Length = firstOffset / 2;
            }

            StringOffsets = s.SerializeArray<ushort>(StringOffsets, Pre_Length, name: nameof(StringOffsets));

            if (StringTileIndices == null) 
            {
                StringTileIndices = new byte[StringOffsets.Length][];
                Strings = new string[StringOffsets.Length];

                var encoding = new SpyroEncoding(s.GetR1Settings().GameModeSelection);

                for (int i = 0; i < StringOffsets.Length; i++) {
                    s.DoAt(Offset + StringOffsets[i], () => 
                    {
                        if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro1 ||
                            s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2)
                        {
                            var length = s.Serialize<byte>((byte)(StringTileIndices?.ElementAtOrDefault(i)?.Length ?? 0), name: $"StringTileIndicesLength[{i}]");
                            StringTileIndices[i] = s.SerializeArray<byte>(StringTileIndices[i], length, name: $"{nameof(StringTileIndices)}[{i}]");
                        }
                        else
                        {
                            s.DoEncoded(new Spyro_StringEncoder(Pre_DecompressHelpers), () => 
                                StringTileIndices[i] = s.SerializeArray<byte>(StringTileIndices[i], s.CurrentLength, name: $"{nameof(StringTileIndices)}[{i}]"));
                        }

                        var str = encoding.GetString(StringTileIndices[i]);
                        Strings[i] = str;
                        s.Log($"String: {str}");
                    });
                }
            }
        }
    }
}