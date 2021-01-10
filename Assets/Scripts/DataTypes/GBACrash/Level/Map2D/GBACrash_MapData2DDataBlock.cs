using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBACrash_MapData2DDataBlock : GBACrash_BaseBlock
    {
        public GBACrash_MapData2D MapData { get; set; } // Set before serializing

        public GBACrash_TileLayerData[] TileLayerDatas { get; set; }
        public GBACrash_TileLayerData CollisionLayerData { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            var basePointer = s.CurrentPointer;

            if (TileLayerDatas == null)
                TileLayerDatas = new GBACrash_TileLayerData[MapData.MapLayers.Length];

            for (int i = 0; i < TileLayerDatas.Length; i++)
            {
                if (MapData.MapLayers[i] != null)
                    TileLayerDatas[i] = s.DoAt(basePointer + MapData.MapLayers[i].DataBlockOffset, () => s.SerializeObject<GBACrash_TileLayerData>(TileLayerDatas[i], x => x.MapLayer = MapData.MapLayers[i], name: $"{nameof(TileLayerDatas)}[{i}]"));
            }

            if (MapData.CollisionDataPointer != null)
                CollisionLayerData = s.DoAt(basePointer + MapData.CollisionLayer.DataBlockOffset, () => s.SerializeObject<GBACrash_TileLayerData>(CollisionLayerData, x => x.MapLayer = MapData.CollisionLayer, name: nameof(CollisionLayerData)));

            s.Goto(basePointer + BlockLength);
        }

        public class GBACrash_TileLayerData : R1Serializable
        {
            public GBACrash_MapLayer MapLayer { get; set; } // Set before serializing

            public ushort[] Offsets { get; set; }

            public GBACrash_TileMapTileCommands[] TileMapTileCommands { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Offsets = s.SerializeArray<ushort>(Offsets, MapLayer.TileMap.Max() + 1, name: nameof(Offsets));

                if (TileMapTileCommands == null)
                    TileMapTileCommands = new GBACrash_TileMapTileCommands[Offsets.Length];

                for (int i = 0; i < TileMapTileCommands.Length; i++)
                    TileMapTileCommands[i] = s.DoAt(Offset + Offsets[i] * 4, () => s.SerializeObject<GBACrash_TileMapTileCommands>(TileMapTileCommands[i], name: $"{nameof(TileMapTileCommands)}[{i}]"));
            }

            public class GBACrash_TileMapTileCommands : R1Serializable
            {
                public GBACrash_TileCommand[] TileCommands { get; set; }

                public override void SerializeImpl(SerializerObject s)
                {
                    if (TileCommands == null)
                    {
                        var cmds = new List<GBACrash_TileCommand>();
                        var count = 0;
                        var index = 0;

                        while (count < 128)
                        {
                            var c = s.SerializeObject<GBACrash_TileCommand>(default, name: $"{nameof(TileCommands)}[{index++}]");
                            count += c.TilesCount;
                            cmds.Add(c);
                        }

                        TileCommands = cmds.ToArray();
                    }
                    else
                    {
                        s.SerializeObjectArray<GBACrash_TileCommand>(TileCommands, TileCommands.Length, name: nameof(TileCommands));
                    }
                }

                public class GBACrash_TileCommand : R1Serializable
                {
                    public byte TilesCount { get; set; }
                    public byte CommandType { get; set; }
                    public ushort[] TileIndices { get; set; }
                    public sbyte[] TileChanges { get; set; }

                    public override void SerializeImpl(SerializerObject s)
                    {
                        TilesCount = s.Serialize<byte>(TilesCount, name: nameof(TilesCount));
                        CommandType = s.Serialize<byte>(CommandType, name: nameof(CommandType));

                        if ((CommandType & 0x80) == 0 && (CommandType & 0x40) == 0)
                            TileIndices = s.SerializeArray<ushort>(TileIndices, TilesCount, name: nameof(TileIndices));
                        else
                            TileIndices = s.SerializeArray<ushort>(TileIndices, 1, name: nameof(TileIndices));

                        if ((CommandType & 0x40) != 0)
                        {
                            TileChanges = s.SerializeArray<sbyte>(TileChanges, TilesCount - 1, name: nameof(TileChanges));
                            s.Align(2);
                        }
                    }
                }
            }
        }
    }
}