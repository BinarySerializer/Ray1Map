namespace R1Engine
{
    public class GBA_ActorModel : GBA_BaseBlock
    {
        public byte[] UnkData { get; set; }

        public byte SpriteGroupOffsetIndex { get; set; }
        public byte Byte_09 { get; set; }
        public byte Byte_0A { get; set; }
        public byte Byte_0B { get; set; }

        public GBA_Action[] Actions { get; set; }

        public GBA_Puppet Puppet { get; set; }
        public GBA_BatmanVengeance_Puppet Puppet_BatmanVengeance { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion > EngineVersion.GBA_BatmanVengeance) {
                UnkData = s.SerializeArray<byte>(UnkData, 8, name: nameof(UnkData));
            }

            SpriteGroupOffsetIndex = s.Serialize<byte>(SpriteGroupOffsetIndex, name: nameof(SpriteGroupOffsetIndex));
            Byte_09 = s.Serialize<byte>(Byte_09, name: nameof(Byte_09));
            Byte_0A = s.Serialize<byte>(Byte_0A, name: nameof(Byte_0A));
            Byte_0B = s.Serialize<byte>(Byte_0B, name: nameof(Byte_0B));

            // TODO: Get number of entries
            if (s.GameSettings.EngineVersion <= EngineVersion.GBA_BatmanVengeance) {
                Actions = s.SerializeObjectArray<GBA_Action>(Actions, (BlockSize - 4) / 12, name: nameof(Actions));
            } else {
                Actions = s.SerializeObjectArray<GBA_Action>(Actions, (BlockSize - 12) / 8, name: nameof(Actions));
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                Puppet_BatmanVengeance = s.DoAt(OffsetTable.GetPointer(SpriteGroupOffsetIndex), () => s.SerializeObject<GBA_BatmanVengeance_Puppet>(Puppet_BatmanVengeance, name: nameof(Puppet_BatmanVengeance)));
            } else {
                Puppet = s.DoAt(OffsetTable.GetPointer(SpriteGroupOffsetIndex), () => s.SerializeObject<GBA_Puppet>(Puppet, name: nameof(Puppet)));
            }

            // Parse state data
            for (var i = 0; i < Actions.Length; i++)
            {
                if (Actions[i].StateDataType != -1)
                    Actions[i].StateData = s.DoAt(OffsetTable.GetPointer(Actions[i].StateDataOffsetIndex), () => s.SerializeObject<GBA_ActorStateData>(Actions[i].StateData, name: $"{nameof(GBA_Action.StateData)}[{i}]"));
            }
        }
    }
}