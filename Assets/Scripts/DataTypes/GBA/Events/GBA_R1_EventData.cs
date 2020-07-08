using System;
using System.Linq;

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
            if(ETAPointer != null) 
            {
                do
                {
                    var hasSerialized = ETA != null && s is BinaryDeserializer;

                    int etatCount;

                    if (!hasSerialized)
                    {
                        etatCount = Etat + 1;

                        // Get correct etat count on GBA
                        if (s.GameSettings.EngineVersion == EngineVersion.RayGBA)
                        {
                            s.DoAt(ETAPointer, () => {
                                int curEtatCount = 0;
                                Pointer off_prev = null;
                                while (true)
                                {
                                    Pointer off_next = s.SerializePointer(null, allowInvalid: true, name: "TestPointer");
                                    if (curEtatCount < etatCount 
                                        || (off_next != null 
                                        && off_next != ETAPointer
                                        && (off_prev == null 
                                        || (off_next.AbsoluteOffset - off_prev.AbsoluteOffset > 0) 
                                        && (off_next.AbsoluteOffset - off_prev.AbsoluteOffset < 0x10000))))
                                    {
                                        curEtatCount++;
                                        off_prev = off_next;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                etatCount = curEtatCount;
                            });
                        }
                    }
                    else
                    {
                        etatCount = ETA.Length;
                    }

                    // Get max linked etat if we've already serialized ETA
                    if (hasSerialized)
                    {
                        var maxLinked = ETA.SelectMany(x => x).Where(x => x != null).Max(x => x.LinkedEtat) + 1;

                        if (etatCount < maxLinked)
                            etatCount = maxLinked;
                    }

                    // Serialize etat pointers
                    Pointer[] EtatPointers = s.DoAt(ETAPointer, () => s.SerializePointerArray(default, etatCount, name: $"{nameof(EtatPointers)}"));

                    // Serialize subetats
                    var prevETA = ETA;
                    ETA = new Common_EventState[EtatPointers.Length][];
                    for (int j = 0; j < EtatPointers.Length; j++)
                    {
                        int count;

                        if (!hasSerialized || prevETA.Length <= j)
                        {
                            if (s.GameSettings.EngineVersion == EngineVersion.RayDSi)
                            {
                                count = j == Etat ? (SubEtat + 1) : 1;
                            }
                            else
                            {
                                Pointer nextPointer = j < EtatPointers.Length - 1 ? EtatPointers[j + 1] : ETAPointer;
                                count = (int)((nextPointer - EtatPointers[j]) / 8);
                            }
                        }
                        else
                        {
                            count = prevETA[j].Length;

                            // Get max linked subetat
                            var validLinks = prevETA.SelectMany(x => x).Where(x => x?.LinkedEtat == j).ToArray();
                            var maxLinked = validLinks.Any() ? validLinks.Max(x => x.LinkedSubEtat) + 1 : -1;

                            if (count < maxLinked)
                                count = maxLinked;
                        }

                        // Serialize all states
                        s.DoAt(EtatPointers[j], () => ETA[j] = s.SerializeObjectArray<Common_EventState>(ETA[j], count, name: $"{nameof(ETA)}[{j}]"));

                        if (ETA[j] == null)
                            ETA[j] = new Common_EventState[count];
                    }
                } while (!ETA.SelectMany(x => x).Where(x => x != null).All(eta => ETA.Length > eta.LinkedEtat && ETA[eta.LinkedEtat].Length > eta.LinkedSubEtat));
            }

            if (CommandsPointer != null)
                s.DoAt(CommandsPointer, () => Commands = s.SerializeObject<Common_EventCommandCollection>(Commands, name: nameof(Commands)));
        }
    }
}