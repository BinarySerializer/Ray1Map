using System;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_TileMapData_TileData8 : R1Serializable
    {
        public ushort MapTileValue { get; set; }
        public ushort UShort_02 { get; set; }
        public ushort[] Decompressed { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapTileValue = s.Serialize<ushort>(MapTileValue, name: nameof(MapTileValue));
            UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));
            s.DoEncoded(new RHREncoder(RHREncoder.EncoderMode.TileData), () => {
                Decompressed = s.SerializeArray<ushort>(Decompressed, s.CurrentLength / 2, name: nameof(Decompressed));
            });
        }
    }
}