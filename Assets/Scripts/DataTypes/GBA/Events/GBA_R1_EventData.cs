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

            // TODO: Parse the ETA fully - sadly it's structured differently from PS1 even though it uses pointers the same way
            // Serialize the current state
            s.DoAt(ETAPointer, () =>
            {
                uint EtatCount = (uint)Etat + 1;
                s.DoAt(ETAPointer, () => {
                    uint CurEtatCount = 0;
                    Pointer off_prev = null;
                    while (true) {
                        Pointer off_next = s.SerializePointer(null, allowInvalid: true, name: "TestPointer");
                        if (CurEtatCount < EtatCount
                        || (off_next != null
                        && off_next != ETAPointer
                        && (off_prev == null
                        || (off_next.AbsoluteOffset - off_prev.AbsoluteOffset > 0)
                        && (off_next.AbsoluteOffset - off_prev.AbsoluteOffset < 0x10000)))) {
                            CurEtatCount++;
                            off_prev = off_next;
                        } else {
                            break;
                        }
                    }
                    EtatCount = CurEtatCount;
                });
                Pointer[] EtatPointers = null;
                s.DoAt(ETAPointer, () => {
                    EtatPointers = s.SerializePointerArray(EtatPointers, EtatCount, name: $"{nameof(EtatPointers)}");
                });
                // Serialize subetats
                ETA = new Common_EventState[EtatPointers.Length][];
                for (int j = 0; j < EtatPointers.Length; j++) {
                    Pointer nextPointer = j < EtatPointers.Length - 1 ? EtatPointers[j + 1] : ETAPointer;
                    uint count = (uint)((nextPointer - EtatPointers[j]) / 8);
                    s.DoAt(EtatPointers[j], () => {
                        ETA[j] = s.SerializeObjectArray<Common_EventState>(ETA[j], count, name: $"{nameof(ETA)}[{j}]");
                    });
                }


                if (ETA == null)
                    ETA = new Common_EventState[Etat + 1][];

                // TODO: Clean up
                /*for (int i = 0; i < ETA.Length; i++)
                {
                    var pointer = s.SerializePointer(null, name: $"EtatPointer {i}");

                    s.DoAt(pointer, () =>
                    {
                        ETA[i] = s.SerializeObjectArray<Common_EventState>(ETA[i], i == Etat ? SubEtat + 1 : 1, name: $"ETA_GBA[{i}]");
                    });
                }*/
            });

            if (CommandsPointer != null)
                s.DoAt(CommandsPointer, () => Commands = s.SerializeObject<Common_EventCommandCollection>(Commands, name: nameof(Commands)));
        }
    }
}