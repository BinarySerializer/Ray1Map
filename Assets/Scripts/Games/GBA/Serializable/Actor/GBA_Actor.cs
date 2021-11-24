using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_Actor : BinarySerializable
    {
        #region Actor Data

        public short XPos { get; set; }
        public short YPos { get; set; }

        public byte Byte_04 { get; set; }
        
        public byte ActorID { get; set; }
        
        public ushort Index_ActorModel { get; set; }

        public byte ActionIndex { get; set; }

        public byte Link_0 { get; set; } = 0xFF;
        public byte Link_1 { get; set; } = 0xFF;
        public byte Link_2 { get; set; } = 0xFF;
        public byte Link_3 { get; set; } = 0xFF;

        // For specific actor types
        public ActorType Type { get; set; }
        // Captor
        public byte Index_CaptorData { get; set; }
        public byte[] UnkData1 { get; set; }
        public byte[] UnkData2 { get; set; }
        public short BoxMinY { get; set; }
        public short BoxMinX { get; set; }
        public short BoxMaxY { get; set; }
        public short BoxMaxX { get; set; }
        public CaptorType CaptorID { get; set; }
        public byte LinkedActorsCount { get; set; }

        // Unk2
        public byte Index { get; set; }
        public byte Unk_01 { get; set; }

        // Only in Prince of Persia
        public short Short_0C { get; set; }
        public short Short_0E { get; set; }
        public byte[] ExtraData { get; set; }

        // Star Wars Trilogy & above
        public ushort ActorSize { get; set; }

        // Milan
        public ushort Milan_Height { get; set; }
        public ushort Milan_XlateID { get; set; }
        public ushort Milan_LinksCount { get; set; }
        public Milan_ActorLink[] Milan_Links { get; set; }
        public ushort DialogCount { get; set; }
        public short[] DialogTable { get; set; } // Loc indices for the dialog boxes when interacting with the actor

        public ushort Milan_CaptorIndicesCount { get; set; }
        public ushort[] Milan_CaptorIndices { get; set; }

        public int? OverridePaletteIndex { get; set; } // Not a serialized property - use this to force the actor to use a specific animation index when loaded

        #endregion

        #region Parsed

        public GBA_ActorModel ActorModel { get; set; }

        public GBA_CaptorData CaptorData { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().GBA_IsMilan)
            {
                if (Type == ActorType.Actor)
                {
                    Index_ActorModel = s.Serialize<ushort>(Index_ActorModel, name: nameof(Index_ActorModel));
                    XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                    YPos = s.Serialize<short>(YPos, name: nameof(YPos));
                    Milan_Height = s.Serialize<ushort>(Milan_Height, name: nameof(Milan_Height));
                    Milan_XlateID = s.Serialize<ushort>(Milan_XlateID, name: nameof(Milan_XlateID));

                    Milan_LinksCount = s.Serialize<ushort>(Milan_LinksCount, name: nameof(Milan_LinksCount));
                    Milan_Links = s.SerializeObjectArray<Milan_ActorLink>(Milan_Links, Milan_LinksCount, name: nameof(Milan_Links));

                    UnkData1 = s.SerializeArray<byte>(UnkData1, s.GetR1Settings().EngineVersion == EngineVersion.GBA_TomClancysRainbowSixRogueSpear ? 8 : 12, name: nameof(UnkData1));

                    DialogCount = s.Serialize<ushort>(DialogCount, name: nameof(DialogCount));
                    DialogTable = s.SerializeArray<short>(DialogTable, DialogCount, name: nameof(DialogTable));
                }
                else
                {
                    XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                    YPos = s.Serialize<short>(YPos, name: nameof(YPos));
                    Milan_Height = s.Serialize<ushort>(Milan_Height, name: nameof(Milan_Height));
                    Milan_XlateID = s.Serialize<ushort>(Milan_XlateID, name: nameof(Milan_XlateID));

                    Milan_LinksCount = s.Serialize<ushort>(Milan_LinksCount, name: nameof(Milan_LinksCount));
                    Milan_Links = s.SerializeObjectArray<Milan_ActorLink>(Milan_Links, Milan_LinksCount, name: nameof(Milan_Links));

                    Milan_CaptorIndicesCount = s.Serialize<ushort>(Milan_CaptorIndicesCount, name: nameof(Milan_CaptorIndicesCount));
                    Milan_CaptorIndices = s.SerializeArray<ushort>(Milan_CaptorIndices, Milan_CaptorIndicesCount, name: nameof(Milan_CaptorIndices));
                }

                return;
            }

            if (Type == ActorType.Unk) {
                Index = s.Serialize<byte>(Index, name: nameof(Index));
                Unk_01 = s.Serialize<byte>(Unk_01, name: nameof(Unk_01));
                ActorSize = s.Serialize<ushort>(ActorSize, name: nameof(ActorSize));
                if (ActorSize >= 2) {
                    Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
                    Index_ActorModel = s.Serialize<byte>((byte)Index_ActorModel, name: nameof(Index_ActorModel));
                }
                ExtraData = s.SerializeArray<byte>(ExtraData, ActorSize - (s.CurrentPointer - Offset), name: nameof(ExtraData));
            } else {
                XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                YPos = s.Serialize<short>(YPos, name: nameof(YPos));

                if (Type != ActorType.Captor) {
                    Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
                    ActorID = s.Serialize<byte>(ActorID, name: nameof(ActorID));

                    if (s.GetR1Settings().EngineVersion < EngineVersion.GBA_SplinterCellPandoraTomorrow || Type == ActorType.Actor || Type == ActorType.AlwaysActor) {
                        Index_ActorModel = s.Serialize<byte>((byte)Index_ActorModel, name: nameof(Index_ActorModel));
                        ActionIndex = s.Serialize<byte>(ActionIndex, name: nameof(ActionIndex));
                    }

                    if (s.GetR1Settings().EngineVersion > EngineVersion.GBA_BatmanVengeance && s.GetR1Settings().EngineVersion < EngineVersion.GBA_SplinterCellPandoraTomorrow) {
                        Link_0 = s.Serialize<byte>(Link_0, name: nameof(Link_0));
                        Link_1 = s.Serialize<byte>(Link_1, name: nameof(Link_1));
                        Link_2 = s.Serialize<byte>(Link_2, name: nameof(Link_2));
                        Link_3 = s.Serialize<byte>(Link_3, name: nameof(Link_3));
                    }

                    if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_SplinterCell
                        && s.GetR1Settings().EngineVersion < EngineVersion.GBA_SplinterCellPandoraTomorrow) {
                        Short_0C = s.Serialize<short>(Short_0C, name: nameof(Short_0C));
                        Short_0E = s.Serialize<short>(Short_0E, name: nameof(Short_0E));
                        int len = Short_0E & 0xF;
                        ExtraData = s.SerializeArray<byte>(ExtraData, len, name: nameof(ExtraData));
                    } 
                    else if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow) 
                    {
                        if (Type == ActorType.Waypoint || Type == ActorType.Unk) {
                            ActorSize = s.Serialize<ushort>(ActorSize, name: nameof(ActorSize));
                            ExtraData = s.SerializeArray<byte>(ExtraData, ActorSize - 8, name: nameof(ExtraData));
                        } else {
                            Short_0C = s.Serialize<short>(Short_0C, name: nameof(Short_0C));
                            ActorSize = s.Serialize<ushort>(ActorSize, name: nameof(ActorSize));
                            ExtraData = s.SerializeArray<byte>(ExtraData, ActorSize - 12, name: nameof(ExtraData));
                        }
                    }
                } else {
                    Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));

                    if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_PrinceOfPersia) {
                        UnkData1 = s.SerializeArray<byte>(UnkData1, 1, name: nameof(UnkData1));
                        LinkedActorsCount = s.Serialize<byte>(LinkedActorsCount, name: nameof(LinkedActorsCount));
                        CaptorID = s.Serialize<CaptorType>(CaptorID, name: nameof(CaptorID));
                    } else {
                        s.DoBits<byte>(b =>
                        {
                            LinkedActorsCount = b.SerializeBits<byte>(LinkedActorsCount, 5, name: nameof(LinkedActorsCount));
                            CaptorID = b.SerializeBits<CaptorType>(CaptorID, 3, name: nameof(CaptorID));
                        });
                    }
                    Index_CaptorData = s.Serialize<byte>(Index_CaptorData, name: nameof(Index_CaptorData));

                    if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow) {
                        UnkData2 = s.SerializeArray<byte>(UnkData2, 1, name: nameof(UnkData2));
                        ActorSize = s.Serialize<ushort>(ActorSize, name: nameof(ActorSize));
                    } else if (s.GetR1Settings().EngineVersion < EngineVersion.GBA_PrinceOfPersia) {
                        UnkData2 = s.SerializeArray<byte>(UnkData2, 1, name: nameof(UnkData2));
                    } else {
                        UnkData2 = s.SerializeArray<byte>(UnkData2, 3, name: nameof(UnkData2));
                    }
                    BoxMinY = s.Serialize<short>(BoxMinY, name: nameof(BoxMinY));
                    BoxMinX = s.Serialize<short>(BoxMinX, name: nameof(BoxMinX));
                    BoxMaxY = s.Serialize<short>(BoxMaxY, name: nameof(BoxMaxY));
                    BoxMaxX = s.Serialize<short>(BoxMaxX, name: nameof(BoxMaxX));
                    if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow) {
                        ExtraData = s.SerializeArray<byte>(ExtraData, ActorSize - 20, name: nameof(ExtraData));
                    }
                }
            }
        }

        #endregion


        public enum ActorType {
            MainActor,
            AlwaysActor,
            Actor,
            Captor,
            Waypoint,
            Unk
        }

        public enum CaptorType : byte
        {
            // Triggers when the area is hit
            Hit = 0,

            // Triggers when the player is in the area
            Player = 1,

            Unk_2,
            Unk_3,
            Unk_4,
            Unk_5,
            Unk_6,
            Unk_7,
        }

        public class Milan_ActorLink : BinarySerializable
        {
            public ushort Ushort_00 { get; set; }
            public ushort LinkedActor { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
                LinkedActor = s.Serialize<ushort>(LinkedActor, name: nameof(LinkedActor));
            }
        }
    }
}