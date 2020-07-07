namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_EventData : R1Serializable
    {
        public Pointer ETAPointer { get; set; }

        public Pointer CommandsPointer { get; set; }

        public short XPosition { get; set; }
        public short YPosition { get; set; }

        public ushort Layer { get; set; }

        //*(_WORD*) (2 * eventIndex + v8549200) = *(_WORD*) (eventStruct1CurrentPointer + 14); - linkindex???
        public ushort LinkGroup { get; set; }

        public byte Etat { get; set; }
        public byte SubEtat { get; set; }
        public byte OffsetBX { get; set; }
        public byte OffsetBY { get; set; }
        public byte OffsetHY { get; set; }

        // *(_BYTE*)(eventOffsetInMemory + 112) = *(_BYTE*)(eventOffsetInMemory + 112) & 0xFE | *(_BYTE*)(eventStruct1CurrentPointer + 21) & 1;
        public bool FollowEnabled { get; set; }

        public byte FollowSprite { get; set; }
        public byte HitPoints { get; set; }

        public EventType Type { get; set; }

        public byte HitSprite { get; set; }

        public byte Unk { get; set; }


        public Common_EventState[][] ETA { get; set; }

        /// <summary>
        /// The event commands
        /// </summary>
        public Common_EventCommandCollection Commands { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));
            CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            Layer = s.Serialize<ushort>(Layer, name: nameof(Layer));
            LinkGroup = s.Serialize<ushort>(LinkGroup, name: nameof(LinkGroup));
            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));
            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));
            FollowEnabled = s.Serialize<bool>(FollowEnabled, name: nameof(FollowEnabled));
            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));
            HitPoints = s.Serialize<byte>(HitPoints, name: nameof(HitPoints));
            Type = s.Serialize<EventType>(Type, name: nameof(Type));
            HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));
            Unk = s.Serialize<byte>(Unk, name: nameof(Unk));

            // Serialize data from pointers

            // Serialize the current state
            if(ETAPointer != null) {
                uint etatCount = (uint)Etat + 1;
                if (s.GameSettings.EngineVersion == EngineVersion.RayDSi) {
                } else {
                    s.DoAt(ETAPointer, () => {
                        uint curEtatCount = 0;
                        Pointer off_prev = null;
                        while (true) {
                            Pointer off_next = s.SerializePointer(null, allowInvalid: true, name: "TestPointer");
                            if (curEtatCount < etatCount
                            || (off_next != null
                            && off_next != ETAPointer
                            && (off_prev == null
                            || (off_next.AbsoluteOffset - off_prev.AbsoluteOffset > 0)
                            && (off_next.AbsoluteOffset - off_prev.AbsoluteOffset < 0x10000)))) {
                                curEtatCount++;
                                off_prev = off_next;
                            } else {
                                break;
                            }
                        }
                        etatCount = curEtatCount;
                    });
                }
                Pointer[] EtatPointers = null;
                s.DoAt(ETAPointer, () => {
                    EtatPointers = s.SerializePointerArray(EtatPointers, etatCount, name: $"{nameof(EtatPointers)}");
                });

                // Serialize subetats
                ETA = new Common_EventState[EtatPointers.Length][];
                for (int j = 0; j < EtatPointers.Length; j++) {
                    uint count = 0;
                    if (s.GameSettings.EngineVersion == EngineVersion.RayDSi) {
                        count = j == Etat ? (uint)(SubEtat + 1) : 1;
                    } else {
                        Pointer nextPointer = j < EtatPointers.Length - 1 ? EtatPointers[j + 1] : ETAPointer;
                        count = (uint)((nextPointer - EtatPointers[j]) / 8);
                    }
                    s.DoAt(EtatPointers[j], () => {
                        ETA[j] = s.SerializeObjectArray<Common_EventState>(ETA[j], count, name: $"{nameof(ETA)}[{j}]");
                    });
                }
            }

            if (CommandsPointer != null)
                s.DoAt(CommandsPointer, () => Commands = s.SerializeObject<Common_EventCommandCollection>(Commands, name: nameof(Commands)));
        }
    }
}