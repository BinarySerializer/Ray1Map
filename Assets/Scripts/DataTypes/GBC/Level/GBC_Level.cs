namespace R1Engine
{
    public class GBC_Level : GBC_BaseBlock
    {
        public byte LevelIndex { get; set; }
        public sbyte InGameLevelIndex { get; set; } // The order of the levels in-game (-1 for optional levels)
        public byte Byte_02 { get; set; }
        public MapType Type { get; set; }
        public byte LinkedLevelsCount { get; set; }
        public bool IsGift { get; set; }
        public sbyte LinkedLevelsStartIndex { get; set; } // Actually start index + 1 (-1 if there are no linked levels)
        public sbyte TimeOut { get; set; }
        public sbyte NbTings { get; set; }
        public byte[] UnkData0 { get; set; }
        public CageUID[] Cages { get; set; }
        public VignetteReference VignetteIntro { get; set; }
        public VignetteReference VignetteOutro { get; set; }
        public VignetteReference VignetteLevelName { get; set; }
        public byte[] UnkData1 { get; set; }
        public byte[] LinkedLevels { get; set; } // level block index = LinkedLevelsStartIndex - 1 + LinkedLevels[i]
        
        // Parsed
        public GBC_Scene Scene { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            LevelIndex = s.Serialize<byte>(LevelIndex, name: nameof(LevelIndex));
            InGameLevelIndex = s.Serialize<sbyte>(InGameLevelIndex, name: nameof(InGameLevelIndex));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Type = s.Serialize<MapType>(Type, name: nameof(Type));
            LinkedLevelsCount = s.Serialize<byte>(LinkedLevelsCount, name: nameof(LinkedLevelsCount));
            IsGift = s.Serialize<bool>(IsGift, name: nameof(IsGift));
            LinkedLevelsStartIndex = s.Serialize<sbyte>(LinkedLevelsStartIndex, name: nameof(LinkedLevelsStartIndex));
            TimeOut = s.Serialize<sbyte>(TimeOut, name: nameof(TimeOut));
            NbTings = s.Serialize<sbyte>(NbTings, name: nameof(NbTings));
            UnkData0 = s.SerializeArray<byte>(UnkData0, 21, name: nameof(UnkData0));
            Cages = s.SerializeObjectArray<CageUID>(Cages, 10, name: nameof(Cages));
            VignetteIntro = s.SerializeObject<VignetteReference>(VignetteIntro, name: nameof(VignetteIntro));
            VignetteOutro = s.SerializeObject<VignetteReference>(VignetteOutro, name: nameof(VignetteOutro));
            VignetteLevelName = s.SerializeObject<VignetteReference>(VignetteLevelName, name: nameof(VignetteLevelName));
            UnkData1 = s.SerializeArray<byte>(UnkData1, 1, name: nameof(UnkData1));
            LinkedLevels = s.SerializeArray<byte>(LinkedLevels, LinkedLevelsCount, name: nameof(LinkedLevels));

            // TODO: Parse remaining data

            // Parse data from pointers
            Scene = s.DoAt(DependencyTable.GetPointer(0), () => s.SerializeObject<GBC_Scene>(Scene, name: nameof(Scene)));
        }

        public class CageUID : R1Serializable {
            public byte GameObjectXlateID { get; set; } // Local to this level
            public byte GlobalCageID { get; set; } // Global

            public override void SerializeImpl(SerializerObject s) {
				GameObjectXlateID = s.Serialize<byte>(GameObjectXlateID, name: nameof(GameObjectXlateID));
                GlobalCageID = s.Serialize<byte>(GlobalCageID, name: nameof(GlobalCageID));
			}
        }
        public class VignetteReference : R1Serializable {
            public bool HasVignette { get; set; }
            public byte? Index { get; set; }
            public byte Byte_02 { get; set; }
            public byte Byte_03 { get; set; }
            public byte? Byte_04 { get; set; }

            // For parsing
            public GBC_Vignette Vignette { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				HasVignette = s.Serialize<bool>(HasVignette, name: nameof(HasVignette));
                Index = s.Serialize<byte?>(Index, name: nameof(Index));
                Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
                Byte_04 = s.Serialize<byte?>(Byte_04, name: nameof(Byte_04));
            }

            public void SerializeVignettes(SerializerObject s, GBC_BaseBlock parentBlock) {
                if(!HasVignette) return;
                s.DoAt(parentBlock.DependencyTable.GetPointer((byte)(Index-1)), () => {
                    Vignette = s.SerializeObject<GBC_Vignette>(Vignette, name: nameof(Vignette));
                });
            }
		}

        public enum MapType : byte {
            Adventure = 0,
            Bonus = 1,
            WorldMap = 2
        }
    }
}