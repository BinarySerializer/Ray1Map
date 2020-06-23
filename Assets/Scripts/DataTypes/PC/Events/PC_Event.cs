using System;

namespace R1Engine
{
    /*
     
    PC 1.21 memory:

    unkData 1 and 2 seems to be split up link tables?
    0x16DDE8 - unkPointer1
    0x16DDEC - unkPointer1DataCount
    0x16DDF0 - pointer to events in memory
    0x16DDF4 - event count
    0x16DDF8 - unkPointer2
    0x16DDFC - unkPointer2DataCount

    0x16F650 - pointer to Rayman's event in memory (note: Rayman uses the "normal" x and y values rather than the runtime ones!)
     
     */

    // TODO: Merge event data with PS1? The majority is the same.
    /// <summary>
    /// Event data for PC
    /// </summary>
    public class PC_Event : R1Serializable
    {
        // These are indexes in the files and get replaced with pointers during runtime!
        public uint DES_ImageDescriptors { get; set; }
        public uint DES_AnimationDescriptors { get; set; }
        public uint DES_ImageBuffer { get; set; }
        public uint ETA { get; set; }

        public uint RuntimeCommandsPointer { get; set; }
        public uint RuntimeLabelOffsetsPointer { get; set; }

        public uint Unk_24 { get; set; }
        public uint Unk_28 { get; set; }
        public uint Unk_32 { get; set; }
        public uint Unk_36 { get; set; }

        public uint XPosition { get; set; }
        public uint YPosition { get; set; }

        public uint Unk_48 { get; set; }

        // This index is used by the game to handle the event links
        public ushort EventIndex { get; set; }

        public ushort Unk_54 { get; set; }
        public ushort Unk_56 { get; set; }
        public ushort Unk_58 { get; set; }

        public ushort RuntimeXPosition { get; set; }
        public ushort RuntimeYPosition { get; set; }

        public ushort Unk_64 { get; set; }
        public ushort Unk_66 { get; set; }

        public ushort ImageDescriptorCount { get; set; }

        public uint Unk_Kit { get; set; }

        public ushort Unk_70 { get; set; }
        public ushort Unk_72 { get; set; }
        public ushort Unk_74 { get; set; }
        public ushort Unk_76 { get; set; }
        public ushort Unk_78 { get; set; }
        public ushort Unk_80 { get; set; }
        public ushort Unk_82 { get; set; }
        public ushort Unk_84 { get; set; }
        public ushort Unk_86 { get; set; }
        public ushort Unk_88 { get; set; }
        public ushort Unk_90 { get; set; }
        public ushort Unk_92 { get; set; }
        public ushort Unk_94 { get; set; }

        public EventType Type { get; set; }

        public byte[] Unk_98 { get; set; }

        public byte Unk_103 { get; set; }

        public byte OffsetBX { get; set; }
        public byte OffsetBY { get; set; }

        public byte RuntimeCurrentAnimIndex { get; set; }
        public byte RuntimeCurrentAnimFrame { get; set; }

        public byte SubEtat { get; set; }
        public byte Etat { get; set; }

        public byte RuntimeSubEtat { get; set; }
        public byte RuntimeEtat { get; set; }

        public uint Unk_112 { get; set; }

        public byte OffsetHY { get; set; }

        /// <summary>
        /// The sprite index which uses the event collision
        /// </summary>
        public byte FollowSprite { get; set; }

        public byte HitPoints { get; set; }
        public byte RuntimeHitPoints { get; set; }

        /// <summary>
        /// The layer the event sprite gets drawn to, between 1 and 7
        /// </summary>
        public byte Layer { get; set; }

        public byte HitSprite { get; set; }

        public byte Unk_122 { get; set; }
        public byte Unk_123 { get; set; }
        public byte Unk_124 { get; set; }
        public byte Unk_125 { get; set; }

        /// <summary>
        /// The layer, as stored in the game. Always 0 when serialized
        /// </summary>
        public byte RuntimeLayer { get; set; }

        public byte Unk_127 { get; set; }

        public byte AnimDescriptorCount { get; set; }

        /// /// <summary>
        /// The event flags
        /// </summary>
        public PC_EventFlags Flags { get; set; }

        /// <summary>
        /// Indicates if the event has collision
        /// </summary>
        public bool FollowEnabled
        {
            get => Flags.HasFlag(PC_EventFlags.FollowEnabled);
            set
            {
                if (value)
                    Flags |= PC_EventFlags.FollowEnabled;
                else
                    Flags &= ~PC_EventFlags.FollowEnabled;
            }
        }

        public ushort Unk_130 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s)
        {
            DES_ImageDescriptors = s.Serialize<uint>(DES_ImageDescriptors, name: nameof(DES_ImageDescriptors));
            DES_AnimationDescriptors = s.Serialize<uint>(DES_AnimationDescriptors, name: nameof(DES_AnimationDescriptors));
            DES_ImageBuffer = s.Serialize<uint>(DES_ImageBuffer, name: nameof(DES_ImageBuffer));
            ETA = s.Serialize<uint>(ETA, name: nameof(ETA));

            RuntimeCommandsPointer = s.Serialize<uint>(RuntimeCommandsPointer, name: nameof(RuntimeCommandsPointer));
            RuntimeLabelOffsetsPointer = s.Serialize<uint>(RuntimeLabelOffsetsPointer, name: nameof(RuntimeLabelOffsetsPointer));
            
            Unk_24 = s.Serialize<uint>(Unk_24, name: nameof(Unk_24));
            Unk_28 = s.Serialize<uint>(Unk_28, name: nameof(Unk_28));
            Unk_32 = s.Serialize<uint>(Unk_32, name: nameof(Unk_32));
            Unk_36 = s.Serialize<uint>(Unk_36, name: nameof(Unk_36));

            XPosition = s.Serialize<uint>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<uint>(YPosition, name: nameof(YPosition));

            Unk_48 = s.Serialize<uint>(Unk_48, name: nameof(Unk_48));

            EventIndex = s.Serialize<ushort>(EventIndex, name: nameof(EventIndex));
            Unk_54 = s.Serialize<ushort>(Unk_54, name: nameof(Unk_54));
            Unk_56 = s.Serialize<ushort>(Unk_56, name: nameof(Unk_56));
            Unk_58 = s.Serialize<ushort>(Unk_58, name: nameof(Unk_58));

            RuntimeXPosition = s.Serialize<ushort>(RuntimeXPosition, name: nameof(RuntimeXPosition));
            RuntimeYPosition = s.Serialize<ushort>(RuntimeYPosition, name: nameof(RuntimeYPosition));

            Unk_64 = s.Serialize<ushort>(Unk_64, name: nameof(Unk_64));
            Unk_66 = s.Serialize<ushort>(Unk_66, name: nameof(Unk_66));

            ImageDescriptorCount = s.Serialize<ushort>(ImageDescriptorCount, name: nameof(ImageDescriptorCount));

            // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong?
            if (s.GameSettings.EngineVersion == EngineVersion.RayKitPC || s.GameSettings.EngineVersion == EngineVersion.RayEduPC || s.GameSettings.EngineVersion == EngineVersion.RayEduPS1)
                Unk_Kit = s.Serialize<uint>(Unk_Kit, name: nameof(Unk_Kit));

            Unk_70 = s.Serialize<ushort>(Unk_70, name: nameof(Unk_70));
            Unk_72 = s.Serialize<ushort>(Unk_72, name: nameof(Unk_72));
            Unk_74 = s.Serialize<ushort>(Unk_74, name: nameof(Unk_74));
            Unk_76 = s.Serialize<ushort>(Unk_76, name: nameof(Unk_76));

            Unk_78 = s.Serialize<ushort>(Unk_78, name: nameof(Unk_78));
            Unk_80 = s.Serialize<ushort>(Unk_80, name: nameof(Unk_80));
            Unk_82 = s.Serialize<ushort>(Unk_82, name: nameof(Unk_82));
            Unk_84 = s.Serialize<ushort>(Unk_84, name: nameof(Unk_84));
            Unk_86 = s.Serialize<ushort>(Unk_86, name: nameof(Unk_86));
            
            Unk_88 = s.Serialize<ushort>(Unk_88, name: nameof(Unk_88));
            Unk_90 = s.Serialize<ushort>(Unk_90, name: nameof(Unk_90));
            Unk_92 = s.Serialize<ushort>(Unk_92, name: nameof(Unk_92));
            Unk_94 = s.Serialize<ushort>(Unk_94, name: nameof(Unk_94));

            Type = s.Serialize<EventType>(Type, name: nameof(Type));
            
            Unk_98 = s.SerializeArray<byte>(Unk_98, 5, name: nameof(Unk_98));
            Unk_103 = s.Serialize<byte>(Unk_103, name: nameof(Unk_103));

            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));

            RuntimeCurrentAnimIndex = s.Serialize<byte>(RuntimeCurrentAnimIndex, name: nameof(RuntimeCurrentAnimIndex));
            RuntimeCurrentAnimFrame = s.Serialize<byte>(RuntimeCurrentAnimFrame, name: nameof(RuntimeCurrentAnimFrame));

            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));

            RuntimeSubEtat = s.Serialize<byte>(RuntimeSubEtat, name: nameof(RuntimeSubEtat));
            RuntimeEtat = s.Serialize<byte>(RuntimeEtat, name: nameof(RuntimeEtat));
            Unk_112 = s.Serialize<uint>(Unk_112, name: nameof(Unk_112));

            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));
            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));
            HitPoints = s.Serialize<byte>(HitPoints, name: nameof(HitPoints));
            RuntimeHitPoints = s.Serialize<byte>(RuntimeHitPoints, name: nameof(RuntimeHitPoints));
            Layer = s.Serialize<byte>(Layer, name: nameof(Layer));
            HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));

            Unk_122 = s.Serialize<byte>(Unk_122, name: nameof(Unk_122));
            Unk_123 = s.Serialize<byte>(Unk_123, name: nameof(Unk_123));
            Unk_124 = s.Serialize<byte>(Unk_124, name: nameof(Unk_124));
            Unk_125 = s.Serialize<byte>(Unk_125, name: nameof(Unk_125));
            RuntimeLayer = s.Serialize<byte>(RuntimeLayer, name: nameof(RuntimeLayer));
            Unk_127 = s.Serialize<byte>(Unk_127, name: nameof(Unk_127));

            AnimDescriptorCount = s.Serialize<byte>(AnimDescriptorCount, name: nameof(AnimDescriptorCount));

            Flags = s.Serialize<PC_EventFlags>(Flags, name: nameof(Flags));

            Unk_130 = s.Serialize<ushort>(Unk_130, name: nameof(Unk_130));
        }

        /// <summary>
        /// Flags for <see cref="PC_Event"/>
        /// </summary>
        [Flags]
        public enum PC_EventFlags : byte
        {
            None = 0,

            UnkFlag_0 = 1 << 0,

            UnkFlag_1 = 1 << 1,

            /// <summary>
            /// Indicates if the event should be drawn on screen
            /// </summary>
            SwitchedOn = 1 << 2,

            /// <summary>
            /// Indicates if the event should be flipped
            /// </summary>
            DetectZone = 1 << 3,

            ExecuteCommands = 1 << 4,
            
            /// <summary>
            /// Indicates if the event has collision
            /// </summary>
            FollowEnabled = 1 << 5,

            UnkFlag_6 = 1 << 6,

            // Appears related to the displaying animation. Changes a lot when an animation is playing.
            UnkFlag_7 = 1 << 7,
        }
    }
}