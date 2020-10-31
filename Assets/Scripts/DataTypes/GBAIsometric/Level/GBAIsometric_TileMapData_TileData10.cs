using System;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_TileMapData_TileData10 : R1Serializable
    {
        public ushort[] Decompressed { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoEncoded(new RHREncoder(RHREncoder.EncoderMode.TileData), () => {
                Decompressed = s.SerializeArray<ushort>(Decompressed, s.CurrentLength / 2, name: nameof(Decompressed));
            });
        }
    }
}