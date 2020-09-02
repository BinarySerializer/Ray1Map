using System;
using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

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

    // All offsets in the names are from the PC version

    /// <summary>
    /// Common event data
    /// </summary>
    public class R1_EventData : R1Serializable
    {
        #region Static Methods

        /// <summary>
        /// Gets a new event instance for Rayman
        /// </summary>
        public static R1_EventData GetRayman(R1_EventData rayPos) => new R1_EventData()
        {
            XPosition = rayPos?.XPosition ?? 100,
            YPosition = rayPos?.YPosition ?? 0,
            Type = R1_EventType.TYPE_RAYMAN,
            OffsetBX = 80,
            OffsetBY = 78,
            SubEtat = 19,
            Etat = 0,
            OffsetHY = 20,
            FollowSprite = 0,
            HitPoints = 0,
            Layer = 7,
            HitSprite = 0,

            PC_ImageDescriptorsIndex = 1,
            PC_AnimationDescriptorsIndex = 1,
            PC_ImageBufferIndex = 1,
            PC_ETAIndex = 0,

            Commands = new R1_EventCommandCollection()
            {
                Commands = new R1_EventCommand[0]
            },
            LabelOffsets = new ushort[0]
        };

        #endregion

        #region Header

        // These are indexes in the files and get replaced with pointers during runtime
        public uint PC_ImageDescriptorsIndex { get; set; }
        public uint PC_AnimationDescriptorsIndex { get; set; }
        public uint PC_ImageBufferIndex { get; set; }
        public uint PC_ETAIndex { get; set; }

        // Keep separate values for these to avoid invalid pointers when reading from the files
        public uint PC_RuntimeCommandsPointer { get; set; }
        public uint PC_RuntimeLabelOffsetsPointer { get; set; }

        public Pointer ImageDescriptorsPointer { get; set; }
        public Pointer AnimDescriptorsPointer { get; set; }

        // Only valid for vol3 PS1 demo and PC
        public Pointer ImageBufferPointer { get; set; }

        public Pointer ETAPointer { get; set; }

        public Pointer CommandsPointer { get; set; }
        public Pointer LabelOffsetsPointer { get; set; }

        #endregion

        #region Event Data

        public byte[] PS1Demo_Unk1 { get; set; }
        public uint PS1Demo_Unk2 { get; set; }
        public uint PS1_Unk1 { get; set; }

        public uint Unk_24 { get; set; }
        public uint Unk_28 { get; set; }
        public uint Unk_32 { get; set; }
        public uint Unk_36 { get; set; }

        public int XPosition { get; set; }
        public int YPosition { get; set; }

        public ushort PS1Demo_Unk3 { get; set; }

        public uint Unk_48 { get; set; }

        // This index is used by the game to handle the event links during runtime
        public ushort EventIndex { get; set; }

        public ushort Unk_54 { get; set; }
        public ushort Unk_56 { get; set; }
        public ushort Unk_58 { get; set; }

        public ushort RuntimeXPosition { get; set; }
        public ushort RuntimeYPosition { get; set; }

        public ushort PS1Demo_Unk4 { get; set; }
        public ushort Unk_64 { get; set; }
        public ushort Unk_66 { get; set; }

        public ushort ImageDescriptorCount { get; set; }

        public ushort RuntimeCurrentCommandOffset { get; set; }
        public ushort RuntimeCurrentCommandArgument { get; set; }
        public ushort Unk_74 { get; set; }
        public ushort Unk_76 { get; set; }
        public ushort Unk_78 { get; set; }

        // This value is used for voice lines as a replacement of the normal HitPoints value in order to have a sample index higher than 255. When this is used HitPoints is always EDU_ExtHitPoints % 256.
        public uint EDU_ExtHitPoints { get; set; }
        
        public ushort Unk_80 { get; set; }
        public ushort Unk_82 { get; set; }
        public ushort Unk_84 { get; set; }
        public ushort Unk_86 { get; set; }
        public ushort Unk_88 { get; set; }
        public ushort Unk_90 { get; set; }

        // TODO: How does this value get set? Appears to be from the type_zdc table, but doesn't always match
        // This, ignoring the last bit, is the index for the collision to use in the hard-coded zdc_tab table (with the structure of x, y, width, height, unk1, unk2)
        public ushort Runtime_ZdcIndex { get; set; }
        public ushort Unk_94 { get; set; }

        public ushort PS1_Unk2 { get; set; }
        public ushort PS1_Unk3 { get; set; }
        public ushort PS1_Unk4 { get; set; }

        public R1_EventType Type { get; set; }

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

        public uint RuntimeCurrentCommand { get; set; }

        public byte OffsetHY { get; set; }

        /// <summary>
        /// The sprite index which uses the event collision
        /// </summary>
        public byte FollowSprite { get; set; }

        public uint ActualHitPoints
        {
            get => Type == R1_EventType.EDU_VoiceLine ? EDU_ExtHitPoints : HitPoints;
            set
            {
                if (Type == R1_EventType.EDU_VoiceLine)
                    EDU_ExtHitPoints = value;

                HitPoints = (byte)(value % 256);
            }
        }

        public byte HitPoints { get; set; }
        public byte RuntimeHitPoints { get; set; }

        /// <summary>
        /// The layer the event sprite gets drawn to, between 1 and 7
        /// </summary>
        public byte Layer { get; set; }

        public byte HitSprite { get; set; }

        public byte PS1_Unk5 { get; set; }

        public byte Unk_122 { get; set; }
        public byte Unk_123 { get; set; }
        public byte Unk_124 { get; set; }
        public byte Unk_125 { get; set; }
        public byte PS1Demo_Unk5 { get; set; }
        public byte PS1Demo_Unk6 { get; set; }
        public byte PS1Demo_Unk7 { get; set; }
        public byte PS1Demo_Unk8 { get; set; }

        public byte RuntimeLayer { get; set; }

        public byte Unk_127 { get; set; }

        public byte AnimDescriptorCount { get; set; }

        public PC_EventFlags PC_Flags { get; set; }

        public byte PS1_Unk6 { get; set; }
        public byte PS1_Flags { get; set; }
        public byte PS1_Unk7 { get; set; }

        public ushort Unk_130 { get; set; }

        #endregion

        #region Helper Data

        public bool IsPCFormat(GameSettings settings) => settings.EngineVersion == EngineVersion.R1_PC || settings.EngineVersion == EngineVersion.R1_PC_Kit || settings.EngineVersion == EngineVersion.R1_PC_Edu || settings.EngineVersion == EngineVersion.R1_PS1_Edu || settings.EngineVersion == EngineVersion.R1_PocketPC || settings.EngineVersion == EngineVersion.R1_GBA || settings.EngineVersion == EngineVersion.R1_DSi;

        public bool GetFollowEnabled(GameSettings settings)
        {
            if (IsPCFormat(settings))
            {
                return PC_Flags.HasFlag(PC_EventFlags.FollowEnabled);
            }
            else
            {
                var offset = settings.EngineVersion == EngineVersion.R1_Saturn ? 7 : 0;

                return BitHelpers.ExtractBits(PS1_Flags, 1, offset) == 1;
            }
        }

        public void SetFollowEnabled(GameSettings settings, bool value)
        {
            if (IsPCFormat(settings))
            {
                if (value)
                    PC_Flags |= PC_EventFlags.FollowEnabled;
                else
                    PC_Flags &= ~PC_EventFlags.FollowEnabled;
            }
            else
            {
                var offset = settings.EngineVersion == EngineVersion.R1_Saturn ? 7 : 0;

                BitHelpers.SetBits(PS1_Flags, value ? 1 : 0, 1, offset);
            }
        }

        #endregion

        #region Parsed From Pointers

        /// <summary>
        /// The image descriptors
        /// </summary>
        public R1_ImageDescriptor[] ImageDescriptors { get; set; }

        /// <summary>
        /// The animation descriptors
        /// </summary>
        public R1_PS1_AnimationDescriptor[] AnimDescriptors { get; set; }

        /// <summary>
        /// Image buffer
        /// </summary>
        public byte[] ImageBuffer { get; set; }

        /// <summary>
        /// The event commands
        /// </summary>
        public R1_EventCommandCollection Commands { get; set; }

        /// <summary>
        /// The command label offsets
        /// </summary>
        public ushort[] LabelOffsets { get; set; }

        /// <summary>
        /// The event ETA
        /// </summary>
        public R1_PS1_ETA ETA { get; set; }

        #endregion

        public override void SerializeImpl(SerializerObject s)
        {
            if (!IsPCFormat(s.GameSettings) || Offset.file is ProcessMemoryStreamFile)
            {
                ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
                AnimDescriptorsPointer = s.SerializePointer(AnimDescriptorsPointer, name: nameof(AnimDescriptorsPointer));
                ImageBufferPointer = s.SerializePointer(ImageBufferPointer, allowInvalid: true, name: nameof(ImageBufferPointer));
                ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));

                CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));

                if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
                {
                    PS1Demo_Unk1 = s.SerializeArray<byte>(PS1Demo_Unk1, 40, name: nameof(PS1Demo_Unk1));

                    EventIndex = s.Serialize<ushort>(EventIndex, name: nameof(EventIndex));

                    PS1Demo_Unk2 = s.Serialize<uint>(PS1Demo_Unk2, name: nameof(PS1Demo_Unk2));
                }
                else
                {
                    LabelOffsetsPointer = s.SerializePointer(LabelOffsetsPointer, name: nameof(LabelOffsetsPointer));

                    if (!IsPCFormat(s.GameSettings))
                        PS1_Unk1 = s.Serialize<uint>(PS1_Unk1, name: nameof(PS1_Unk1));
                }
            }
            else
            {
                PC_ImageDescriptorsIndex = s.Serialize<uint>(PC_ImageDescriptorsIndex, name: nameof(PC_ImageDescriptorsIndex));
                PC_AnimationDescriptorsIndex = s.Serialize<uint>(PC_AnimationDescriptorsIndex, name: nameof(PC_AnimationDescriptorsIndex));
                PC_ImageBufferIndex = s.Serialize<uint>(PC_ImageBufferIndex, name: nameof(PC_ImageBufferIndex));
                PC_ETAIndex = s.Serialize<uint>(PC_ETAIndex, name: nameof(PC_ETAIndex));

                PC_RuntimeCommandsPointer = s.Serialize<uint>(PC_RuntimeCommandsPointer, name: nameof(PC_RuntimeCommandsPointer));
                PC_RuntimeLabelOffsetsPointer = s.Serialize<uint>(PC_RuntimeLabelOffsetsPointer, name: nameof(PC_RuntimeLabelOffsetsPointer));
            }

            if (IsPCFormat(s.GameSettings))
            {
                Unk_24 = s.Serialize<uint>(Unk_24, name: nameof(Unk_24));
                Unk_28 = s.Serialize<uint>(Unk_28, name: nameof(Unk_28));
                Unk_32 = s.Serialize<uint>(Unk_32, name: nameof(Unk_32));
                Unk_36 = s.Serialize<uint>(Unk_36, name: nameof(Unk_36));
            }

            if (IsPCFormat(s.GameSettings))
            {
                XPosition = s.Serialize<int>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<int>(YPosition, name: nameof(YPosition));
            }
            else
            {
                XPosition = s.Serialize<short>((short)XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<short>((short)YPosition, name: nameof(YPosition));
            }

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
            {
                PS1Demo_Unk3 = s.Serialize<ushort>(PS1Demo_Unk3, name: nameof(PS1Demo_Unk3));
            }
            else
            {
                if (IsPCFormat(s.GameSettings))
                    Unk_48 = s.Serialize<uint>(Unk_48, name: nameof(Unk_48));

                EventIndex = s.Serialize<ushort>(EventIndex, name: nameof(EventIndex));
                Unk_54 = s.Serialize<ushort>(Unk_54, name: nameof(Unk_54));
                Unk_56 = s.Serialize<ushort>(Unk_56, name: nameof(Unk_56));
                Unk_58 = s.Serialize<ushort>(Unk_58, name: nameof(Unk_58));
            }

            RuntimeXPosition = s.Serialize<ushort>(RuntimeXPosition, name: nameof(RuntimeXPosition));
            RuntimeYPosition = s.Serialize<ushort>(RuntimeYPosition, name: nameof(RuntimeYPosition));

            // NOTE: This appears between here and ImageDescriptorCount - where does it belong?
            if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3)
                PS1Demo_Unk4 = s.Serialize<ushort>(PS1Demo_Unk4, name: nameof(PS1Demo_Unk4));

            Unk_64 = s.Serialize<ushort>(Unk_64, name: nameof(Unk_64));
            Unk_66 = s.Serialize<ushort>(Unk_66, name: nameof(Unk_66));

            ImageDescriptorCount = s.Serialize<ushort>(ImageDescriptorCount, name: nameof(ImageDescriptorCount));

            RuntimeCurrentCommandOffset = s.Serialize<ushort>(RuntimeCurrentCommandOffset, name: nameof(RuntimeCurrentCommandOffset));
            
            RuntimeCurrentCommandArgument = s.Serialize<ushort>(RuntimeCurrentCommandArgument, name: nameof(RuntimeCurrentCommandArgument));
            Unk_74 = s.Serialize<ushort>(Unk_74, name: nameof(Unk_74));
            Unk_76 = s.Serialize<ushort>(Unk_76, name: nameof(Unk_76));
            Unk_78 = s.Serialize<ushort>(Unk_78, name: nameof(Unk_78));

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit || 
                s.GameSettings.EngineVersion == EngineVersion.R1_PC_Edu || 
                s.GameSettings.EngineVersion == EngineVersion.R1_PS1_Edu)
                EDU_ExtHitPoints = s.Serialize<uint>(EDU_ExtHitPoints, name: nameof(EDU_ExtHitPoints));

            Unk_80 = s.Serialize<ushort>(Unk_80, name: nameof(Unk_80));
            Unk_82 = s.Serialize<ushort>(Unk_82, name: nameof(Unk_82));
            Unk_84 = s.Serialize<ushort>(Unk_84, name: nameof(Unk_84));
            Unk_86 = s.Serialize<ushort>(Unk_86, name: nameof(Unk_86));
            
            Unk_88 = s.Serialize<ushort>(Unk_88, name: nameof(Unk_88));
            Unk_90 = s.Serialize<ushort>(Unk_90, name: nameof(Unk_90));
            Runtime_ZdcIndex = s.Serialize<ushort>(Runtime_ZdcIndex, name: nameof(Runtime_ZdcIndex));
            Unk_94 = s.Serialize<ushort>(Unk_94, name: nameof(Unk_94));

            if (IsPCFormat(s.GameSettings))
            {
                Type = s.Serialize<R1_EventType>(Type, name: nameof(Type));
                Unk_98 = s.SerializeArray<byte>(Unk_98, 5, name: nameof(Unk_98));
                Unk_103 = s.Serialize<byte>(Unk_103, name: nameof(Unk_103));
            }
            else
            {
                PS1_Unk2 = s.Serialize<ushort>(PS1_Unk2, name: nameof(PS1_Unk2));

                if (s.GameSettings.EngineVersion != EngineVersion.R1_PS1_JPDemoVol3)
                {
                    PS1_Unk3 = s.Serialize<ushort>(PS1_Unk3, name: nameof(PS1_Unk3));
                    PS1_Unk4 = s.Serialize<ushort>(PS1_Unk4, name: nameof(PS1_Unk4));
                }
            }

            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));

            RuntimeCurrentAnimIndex = s.Serialize<byte>(RuntimeCurrentAnimIndex, name: nameof(RuntimeCurrentAnimIndex));
            RuntimeCurrentAnimFrame = s.Serialize<byte>(RuntimeCurrentAnimFrame, name: nameof(RuntimeCurrentAnimFrame));

            if (IsPCFormat(s.GameSettings))
            {
                SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
                Etat = s.Serialize<byte>(Etat, name: nameof(Etat));

                RuntimeSubEtat = s.Serialize<byte>(RuntimeSubEtat, name: nameof(RuntimeSubEtat));
                RuntimeEtat = s.Serialize<byte>(RuntimeEtat, name: nameof(RuntimeEtat));
            }
            else
            {
                Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
                RuntimeEtat = s.Serialize<byte>(RuntimeEtat, name: nameof(RuntimeEtat));
                SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
                RuntimeSubEtat = s.Serialize<byte>(RuntimeSubEtat, name: nameof(RuntimeSubEtat));
            }

            RuntimeCurrentCommand = s.Serialize<uint>(RuntimeCurrentCommand, name: nameof(RuntimeCurrentCommand));

            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
                PS1_Flags = s.Serialize<byte>(PS1_Flags, name: nameof(PS1_Flags));

            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));
            HitPoints = s.Serialize<byte>(HitPoints, name: nameof(HitPoints));
            RuntimeHitPoints = s.Serialize<byte>(RuntimeHitPoints, name: nameof(RuntimeHitPoints));
            Layer = s.Serialize<byte>(Layer, name: nameof(Layer));

            if (!IsPCFormat(s.GameSettings))
                Type = (R1_EventType)s.Serialize<byte>((byte)Type, name: nameof(Type));

            HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));

            if (!IsPCFormat(s.GameSettings))
                PS1_Unk5 = s.Serialize<byte>(PS1_Unk5, name: nameof(PS1_Unk5));

            Unk_122 = s.Serialize<byte>(Unk_122, name: nameof(Unk_122));
            Unk_123 = s.Serialize<byte>(Unk_123, name: nameof(Unk_123));
            Unk_124 = s.Serialize<byte>(Unk_124, name: nameof(Unk_124));
            Unk_125 = s.Serialize<byte>(Unk_125, name: nameof(Unk_125));

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
            {
                PS1Demo_Unk5 = s.Serialize<byte>(PS1Demo_Unk5, name: nameof(PS1Demo_Unk5));

                if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3)
                {
                    PS1Demo_Unk6 = s.Serialize<byte>(PS1Demo_Unk6, name: nameof(PS1Demo_Unk6));
                    PS1Demo_Unk7 = s.Serialize<byte>(PS1Demo_Unk7, name: nameof(PS1Demo_Unk7));
                    PS1Demo_Unk8 = s.Serialize<byte>(PS1Demo_Unk8, name: nameof(PS1Demo_Unk8));
                }
            }

            RuntimeLayer = s.Serialize<byte>(RuntimeLayer, name: nameof(RuntimeLayer));
            Unk_127 = s.Serialize<byte>(Unk_127, name: nameof(Unk_127));

            AnimDescriptorCount = s.Serialize<byte>(AnimDescriptorCount, name: nameof(AnimDescriptorCount));

            if (IsPCFormat(s.GameSettings))
            {
                PC_Flags = s.Serialize<PC_EventFlags>(PC_Flags, name: nameof(PC_Flags));
                Unk_130 = s.Serialize<ushort>(Unk_130, name: nameof(Unk_130));
            }
            else
            {
                if (s.GameSettings.EngineVersion != EngineVersion.R1_PS1_JPDemoVol3)
                {
                    // Appears to be some form of runtime flags for if the event is in view, should be drawn etc. (not in demos though)
                    PS1_Unk6 = s.Serialize<byte>(PS1_Unk6, name: nameof(PS1_Unk6));

                    if (s.GameSettings.EngineVersion != EngineVersion.R1_PS1_JPDemoVol6)
                    {
                        PS1_Flags = s.Serialize<byte>(PS1_Flags, name: nameof(PS1_Flags));

                        // Always 0, even in memory
                        PS1_Unk7 = s.Serialize<byte>(PS1_Unk7, name: nameof(PS1_Unk7));
                    }
                }
            }

            // Parse data from pointers only on PS1 and if we're not reading from processed memory
            if (!IsPCFormat(s.GameSettings) && !(Offset.file is ProcessMemoryStreamFile))
            {
                // Serialize the image descriptors
                s.DoAt(ImageDescriptorsPointer, () => ImageDescriptors = s.SerializeObjectArray<R1_ImageDescriptor>(ImageDescriptors, ImageDescriptorCount, name: nameof(ImageDescriptors)));

                // Serialize the animation descriptors
                s.DoAt(AnimDescriptorsPointer, () => AnimDescriptors = s.SerializeObjectArray<R1_PS1_AnimationDescriptor>(AnimDescriptors, AnimDescriptorCount, name: nameof(AnimDescriptors)));

                if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3)
                {
                    if (ImageBuffer == null && ImageBufferPointer != null && ImageDescriptors != null)
                    {
                        // Determine length of image buffer
                        uint length = 0;
                        foreach (R1_ImageDescriptor img in ImageDescriptors)
                        {
                            if (img.ImageType != 2 && img.ImageType != 3)
                                continue;

                            uint curLength = img.ImageBufferOffset;

                            if (img.ImageType == 2)
                                curLength += (uint)(img.OuterWidth / 2) * img.OuterHeight;
                            else if (img.ImageType == 3)
                                curLength += (uint)img.OuterWidth * img.OuterHeight;

                            if (curLength > length)
                                length = curLength;
                        }
                        ImageBuffer = new byte[length];
                    }
                    s.DoAt(ImageBufferPointer, () => ImageBuffer = s.SerializeArray<byte>(ImageBuffer, ImageBuffer.Length, name: nameof(ImageBuffer)));
                }

                // Serialize the commands
                if (CommandsPointer != null)
                    s.DoAt(CommandsPointer, () => Commands = s.SerializeObject<R1_EventCommandCollection>(Commands, name: nameof(Commands)));

                // Serialize the label offsets
                if (LabelOffsetsPointer != null)
                {
                    s.DoAt(LabelOffsetsPointer, () =>
                    {
                        if (LabelOffsets == null)
                        {
                            // Create a temporary list
                            var l = new List<ushort>();

                            int index = 0;

                            // Loop until we reach null
                            while (l.LastOrDefault() != 0)
                            {
                                l.Add(s.Serialize((ushort)0, name: $"LabelOffsets [{index}]"));
                                index++;
                            }

                            // Set the label offsets
                            LabelOffsets = l.ToArray();
                        }
                        else
                        {
                            // Serialize the label offsets
                            s.SerializeArray(LabelOffsets, LabelOffsets.Length, name: nameof(LabelOffsets));

                            // Null terminate it
                            s.Serialize((byte)0, name: nameof(LabelOffsets) + " NULL");
                        }
                    });
                }

                // Serialize ETA
                if (ETAPointer != null)
                    s.DoAt(ETAPointer, () => ETA = s.SerializeObject<R1_PS1_ETA>(ETA, name: nameof(ETA)));

                /*s.DoAt(ETAPointer + (Etat * 4), () =>
                {
                    // Get the state-array pointer
                    ETASubEtatPointer = s.SerializePointer(ETASubEtatPointer, name: nameof(ETASubEtatPointer));

                    // Serialize event state
                    s.DoAt(ETASubEtatPointer + (SubEtat * 8), () =>
                    {
                        EventState = s.SerializeObject(EventState, name: nameof(EventState));
                    });
                });*/

                if (ETA?.EventStates?.ElementAtOrDefault(Etat)?.ElementAtOrDefault(SubEtat) == null)
                    Debug.LogWarning($"Matching event state not found for event {Type} at {XPosition}x{YPosition} with E{Etat},SE{SubEtat} for {s.GameSettings.GameModeSelection} in {s.GameSettings.World}{s.GameSettings.Level}");
            }
        }

        /// <summary>
        /// Flags for an event on PC. All values are runtime only except for FollowEnabled.
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