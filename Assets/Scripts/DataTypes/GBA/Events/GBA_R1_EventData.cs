namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_EventData : R1Serializable
    {
        public Pointer ETAPointer { get; set; }

        public Pointer CommandsPointer { get; set; }

        public ushort XPosition { get; set; }
        public ushort YPosition { get; set; }

        public ushort Layer { get; set; }

        //*(_WORD*) (2 * eventIndex + v8549200) = *(_WORD*) (eventStruct1CurrentPointer + 14); - linkindex???
        public ushort SomeIndex { get; set; }

        public byte Etat { get; set; }
        public byte SubEtat { get; set; }
        public byte OffsetBX { get; set; }
        public byte OffsetBY { get; set; }
        public byte OffsetHY { get; set; }

        // *(_BYTE*)(eventOffsetInMemory + 112) = *(_BYTE*)(eventOffsetInMemory + 112) & 0xFE | *(_BYTE*)(eventStruct1CurrentPointer + 21) & 1;
        public byte SomeFlag { get; set; }

        public byte FollowSprite { get; set; }
        public byte HitPoints { get; set; }

        public EventType Type { get; set; }

        public byte HitSprite { get; set; }

        public byte Unk { get; set; }


        public Common_EventState[][] ETA_GBA { get; set; }

        /// <summary>
        /// The event commands
        /// </summary>
        public Common_EventCommandCollection Commands_GBA { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));
            CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));
            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));
            Layer = s.Serialize<ushort>(Layer, name: nameof(Layer));
            SomeIndex = s.Serialize<ushort>(SomeIndex, name: nameof(SomeIndex));
            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));
            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));
            SomeFlag = s.Serialize<byte>(SomeFlag, name: nameof(SomeFlag));
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
                if (ETA_GBA == null)
                    ETA_GBA = new Common_EventState[Etat + 1][];

                // TODO: Clean up
                for (int i = 0; i < ETA_GBA.Length; i++)
                {
                    var pointer = s.SerializePointer(null, name: $"EtatPointer {i}");

                    s.DoAt(pointer, () =>
                    {
                        ETA_GBA[i] = s.SerializeObjectArray<Common_EventState>(ETA_GBA[i], i == Etat ? SubEtat + 1 : 1, name: $"ETA_GBA[{i}]");
                    });
                }
            });

            if (CommandsPointer != null)
                s.DoAt(CommandsPointer, () => Commands_GBA = s.SerializeObject<Common_EventCommandCollection>(Commands_GBA, name: nameof(Commands_GBA)));
        }
    }
}