namespace R1Engine
{
    public class GBA_ActorModel : GBA_BaseBlock
    {
        public byte[] UnkData { get; set; }

        public byte Index_Puppet { get; set; }
        public byte Byte_09 { get; set; }
        public byte Byte_0A { get; set; }
        public byte Byte_0B { get; set; }

        public GBA_Action[] Actions { get; set; }

        public GBA_Puppet Puppet { get; set; }
        public GBA_BatmanVengeance_Puppet Puppet_BatmanVengeance { get; set; }

        // Milan
        public string Milan_ActorID { get; set; }
        public GBA_BlockArray<GBA_Puppet> Milan_Puppets { get; set; }
        public GBA_BlockArray<GBA_BatmanVengeance_Puppet> TomClancy_Puppets { get; set; }
        public GBA_BlockArray<GBA_Milan_ActionBlock> Milan_Actions { get; set; }

        public GBA_Puppet[] GetPuppets => Context.Settings.GBA_IsMilan ? Milan_Puppets.Blocks : new GBA_Puppet[]
        {
            Puppet
        };

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GameSettings.GBA_IsMilan)
            {
                Milan_ActorID = s.SerializeString(Milan_ActorID, length: 4, name: nameof(Milan_ActorID));
                s.SerializeArray<byte>(new byte[8], 8, name: "Padding");
            }
            else
            {
                if (s.GameSettings.EngineVersion > EngineVersion.GBA_BatmanVengeance)
                {
                    UnkData = s.SerializeArray<byte>(UnkData, 8, name: nameof(UnkData));
                }

                Index_Puppet = s.Serialize<byte>(Index_Puppet, name: nameof(Index_Puppet));
                Byte_09 = s.Serialize<byte>(Byte_09, name: nameof(Byte_09));
                Byte_0A = s.Serialize<byte>(Byte_0A, name: nameof(Byte_0A));
                Byte_0B = s.Serialize<byte>(Byte_0B, name: nameof(Byte_0B));

                if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance)
                {
                    Actions = s.SerializeObjectArray<GBA_Action>(Actions, (BlockSize - 4) / 12, name: nameof(Actions));
                }
                else
                {
                    Actions = s.SerializeObjectArray<GBA_Action>(Actions, (BlockSize - 12) / 8, name: nameof(Actions));
                }
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) 
            {
                Puppet_BatmanVengeance = s.DoAt(OffsetTable.GetPointer(Index_Puppet), () => s.SerializeObject<GBA_BatmanVengeance_Puppet>(Puppet_BatmanVengeance, name: nameof(Puppet_BatmanVengeance)));
            }
            else if (s.GameSettings.GBA_IsMilan)
            {
                if (s.GameSettings.EngineVersion == EngineVersion.GBA_TomClancysRainbowSixRogueSpear)
                {
                    TomClancy_Puppets = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_BlockArray<GBA_BatmanVengeance_Puppet>>(TomClancy_Puppets, name: nameof(TomClancy_Puppets)));
                }
                else
                {
                    Milan_Puppets = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_BlockArray<GBA_Puppet>>(Milan_Puppets, name: nameof(Milan_Puppets)));
                }

                Milan_Actions = s.DoAt(OffsetTable.GetPointer(1), () => s.SerializeObject<GBA_BlockArray<GBA_Milan_ActionBlock>>(Milan_Actions, name: nameof(Milan_Actions)));
            }
            else 
            {
                Puppet = s.DoAt(OffsetTable.GetPointer(Index_Puppet), () => s.SerializeObject<GBA_Puppet>(Puppet, name: nameof(Puppet)));
            }

            if (!s.GameSettings.GBA_IsMilan)
            {
                // Parse state data
                for (var i = 0; i < Actions.Length; i++)
                {
                    if (Actions[i].StateDataType != -1)
                        Actions[i].StateData = s.DoAt(OffsetTable.GetPointer(Actions[i].Index_StateData), () => s.SerializeObject<GBA_ActorStateData>(Actions[i].StateData, name: $"{nameof(GBA_Action.StateData)}[{i}]"));
                }
            }
        }
    }
}