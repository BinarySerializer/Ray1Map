using System.Collections.Generic;

namespace R1Engine
{
    public class GBACrash_Isometric_MapLayer : R1Serializable
    {
        public bool IsWorldMap { get; set; } // Set before serializing

        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public uint[] TileMapRowOffsets { get; set; }

        public GBACrash_Isometric_TileMapRow[] TileMapRows { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            TileMapRowOffsets = s.SerializeArray<uint>(TileMapRowOffsets, IsWorldMap ? Width : Height, name: nameof(TileMapRowOffsets));

            if (TileMapRows == null)
                TileMapRows = new GBACrash_Isometric_TileMapRow[TileMapRowOffsets.Length];

            for (int i = 0; i < TileMapRows.Length; i++)
                TileMapRows[i] = s.DoAt(s.CurrentPointer + TileMapRowOffsets[i] * 2, () => s.SerializeObject<GBACrash_Isometric_TileMapRow>(TileMapRows[i], x => x.Width = (IsWorldMap ? Height : Width), name: $"{nameof(TileMapRows)}[{i}]"));
        }

        public class GBACrash_Isometric_TileMapRow : R1Serializable
        {
            public ushort Width { get; set; } // Set before serializing

            public GBACrash_Isometric_TileCommand[] Commands { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                if (Commands == null)
                {
                    var cmds = new List<GBACrash_Isometric_TileCommand>();
                    var count = 0;
                    var index = 0;

                    while (count < Width)
                    {
                        var cmd = s.SerializeObject<GBACrash_Isometric_TileCommand>(default, name: $"{nameof(Commands)}[{index++}]");
                        
                        cmds.Add(cmd);

                        count += cmd.Type == 2 || cmd.Type == 3 ? cmd.Length : 1;
                    }

                    Commands = cmds.ToArray();
                }
                else
                {
                    s.SerializeObjectArray<GBACrash_Isometric_TileCommand>(Commands, Commands.Length, name: nameof(Commands));
                }
            }

            public class GBACrash_Isometric_TileCommand : R1Serializable
            {
                public ushort Length { get; set; }
                public byte Type { get; set; }

                public ushort Param { get; set; }
                public ushort[] Params { get; set; }

                public override void SerializeImpl(SerializerObject s)
                {
                    s.SerializeBitValues<ushort>(bitFunc =>
                    {
                        Length = (ushort)bitFunc(Length, 14, name: nameof(Length));
                        Type = (byte)bitFunc(Type, 2, name: nameof(Type));
                    });

                    if (Type == 3)
                        Params = s.SerializeArray<ushort>(Params, Length, name: nameof(Params));
                    else if (Type == 2)
                        Param = s.Serialize<ushort>(Param, name: nameof(Param));
                }
            }
        }
    }
}