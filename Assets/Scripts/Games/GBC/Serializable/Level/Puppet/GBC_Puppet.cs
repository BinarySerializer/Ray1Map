﻿using BinarySerializer;

namespace Ray1Map.GBC
{
    public class GBC_Puppet : GBC_BaseBlock
    {
        public byte[] PuppetData { get; set; }

        // Parsed
        public GBC_TileKit TileKit { get; set; }
        public GBC_RomChannel[] Animations { get; set; }
        public GBC_Puppet BasePuppet { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            PuppetData = s.SerializeArray<byte>(PuppetData, BlockSize, name: nameof(PuppetData));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBC_R1 || PuppetData.Length != 0) {
                TileKit = s.DoAt(DependencyTable.GetPointer(0), () => s.SerializeObject<GBC_TileKit>(TileKit, name: nameof(TileKit)));
            } else {
                BasePuppet = s.DoAt(DependencyTable.GetPointer(0), () => s.SerializeObject<GBC_Puppet>(BasePuppet, name: nameof(BasePuppet)));
            }
            if(Animations == null) Animations = new GBC_RomChannel[DependencyTable.DependenciesCount - 1];
            for (int i = 0; i < Animations.Length; i++) {
                Animations[i] = s.DoAt(DependencyTable.GetPointer(i+1), () => s.SerializeObject<GBC_RomChannel>(Animations[i], name: $"{nameof(Animations)}[{i}]"));
            }
        }
    }
}