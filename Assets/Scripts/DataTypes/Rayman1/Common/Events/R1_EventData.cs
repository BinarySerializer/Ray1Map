using R1Engine.Serialize;
using System;
using System.Linq;
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
        public static R1_EventData GetRayman(Context context, R1_EventData rayPos) => new R1_EventData().InitRayman(context, rayPos);
        public static R1_EventData GetMapObj(Context context, short x, short y, int index) => new R1_EventData().InitMapObj(context, x, y, index);

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
        public uint PS1_Unk1 { get; set; }

        public CommandContext[] CMD_Contexts { get; set; }

        // How many of these uints are a part of the CMD context array?
        public uint Uint_1C { get; set; }
        public uint Uint_20 { get; set; }
        public uint IsActive { get; set; } // 0 if inactive, 1 if active - is this a bool or can it be other values too? Game checks if it's 0 to see if always object is inactive.

        public int XPosition { get; set; }
        public int YPosition { get; set; }

        public short PS1Demo_Unk3 { get; set; }

        public uint Uint_30 { get; set; }

        // This index is used by the game to handle the event links during runtime
        public short EventIndex { get; set; }

        public short ScreenXPosition { get; set; }
        public short ScreenYPosition { get; set; }
        public short Short_3A { get; set; }

        public short InitialXPosition { get; set; }
        public short InitialYPosition { get; set; }

        public bool PS1Demo_IsFlipped { get; set; }
        public byte PS1Demo_Padding { get; set; }
        public short SpeedX { get; set; }
        public short SpeedY { get; set; }

        public ushort ImageDescriptorCount { get; set; }

        public short CMD_CurrentOffset { get; set; }
        public short CMD_Arg0 { get; set; } // This along with CMD_Arg1 might be a more generic temp value, so might have other uses too
        public short Short_4A { get; set; } // For Rayman this holds the index of the object he's standing on. It most likely has different uses for other events based on type. In R2 this is in the type specific data.
        public short Short_4C { get; set; }
        public short Short_4E { get; set; }

        // This value is used for voice lines as a replacement of the normal HitPoints value in order to have a sample index higher than 255. When this is used HitPoints is always EDU_ExtHitPoints % 256.
        public uint EDU_ExtHitPoints { get; set; }
        
        public short CMD_Arg1 { get; set; }
        public short Short_52 { get; set; } // Linked event index?
        public short Short_54 { get; set; }
        public short Short_56 { get; set; }
        public short Short_58 { get; set; } // Prev collision type for moving platforms
        public short Short_5A { get; set; }

        public R1_ZDCEntry Runtime_TypeZDC { get; set; }
        public short Short_5E { get; set; }

        public R1_EventType Type { get; set; }

        public R1_TileCollisionType[] CollisionTypes { get; set; }

        public byte Byte_67 { get; set; }

        public byte OffsetBX { get; set; }
        public byte OffsetBY { get; set; }

        public byte RuntimeCurrentAnimIndex { get; set; }
        public byte RuntimeCurrentAnimFrame { get; set; }

        public byte SubEtat { get; set; }
        public byte Etat { get; set; }

        public byte InitialSubEtat { get; set; }
        public byte InitialEtat { get; set; }

        public uint CMD_CurrentCommand { get; set; }

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
        public byte InitialHitPoints { get; set; }

        /// <summary>
        /// The layer the event sprite gets drawn to, between 1 and 7
        /// </summary>
        public byte DisplayPrio { get; set; }

        public byte HitSprite { get; set; }

        public byte PS1_Unk5 { get; set; }

        public byte Byte_7A { get; set; }
        public byte Byte_7B { get; set; }
        public byte CMD_CurrentContext { get; set; }
        public byte Byte_7D { get; set; }
        public byte PS1Demo_Unk5 { get; set; }
        public byte PS1Demo_Unk6 { get; set; }
        public byte PS1Demo_Unk7 { get; set; }
        public byte PS1Demo_Unk8 { get; set; }

        public byte InitialDisplayPrio { get; set; }

        public byte Byte_7F { get; set; }

        public byte AnimDescriptorCount { get; set; }

        public PC_EventFlags PC_Flags { get; set; }

        public PS1_EventFlags PS1_RuntimeFlags { get; set; }
        public byte PS1_Flags { get; set; }
        public byte PS1_Unk7 { get; set; }

        public ushort Ushort_82 { get; set; }

        #endregion

        #region Helper Data

        public bool IsPCFormat(GameSettings settings) => settings.EngineVersion == EngineVersion.R1_PC || 
                                                         settings.EngineVersion == EngineVersion.R1_PC_Kit || 
                                                         settings.EngineVersion == EngineVersion.R1_PC_Edu || 
                                                         settings.EngineVersion == EngineVersion.R1_PS1_Edu || 
                                                         settings.EngineVersion == EngineVersion.R1_PocketPC || 
                                                         settings.EngineVersion == EngineVersion.R1_GBA || 
                                                         settings.EngineVersion == EngineVersion.R1_DSi;

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

                PS1_Flags = (byte)BitHelpers.SetBits(PS1_Flags, value ? 1 : 0, 1, offset);
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
            if (!IsPCFormat(s.GameSettings) || Offset?.file is ProcessMemoryStreamFile || s.GameSettings.EngineVersion == EngineVersion.R1_GBA || s.GameSettings.EngineVersion == EngineVersion.R1_DSi)
            {
                ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
                AnimDescriptorsPointer = s.SerializePointer(AnimDescriptorsPointer, name: nameof(AnimDescriptorsPointer));
                ImageBufferPointer = s.SerializePointer(ImageBufferPointer, allowInvalid: true, name: nameof(ImageBufferPointer));
                ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));

                CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));

                if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
                {
                    PS1Demo_Unk1 = s.SerializeArray<byte>(PS1Demo_Unk1, 40, name: nameof(PS1Demo_Unk1));

                    EventIndex = s.Serialize<short>(EventIndex, name: nameof(EventIndex));
                    ScreenXPosition = s.Serialize<short>(ScreenXPosition, name: nameof(ScreenXPosition));
                    ScreenYPosition = s.Serialize<short>(ScreenYPosition, name: nameof(ScreenYPosition));
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
                CMD_Contexts = s.SerializeObjectArray<CommandContext>(CMD_Contexts, 1, name: nameof(CMD_Contexts));
                Uint_1C = s.Serialize<uint>(Uint_1C, name: nameof(Uint_1C));
                Uint_20 = s.Serialize<uint>(Uint_20, name: nameof(Uint_20));
                IsActive = s.Serialize<uint>(IsActive, name: nameof(IsActive));
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
                PS1Demo_Unk3 = s.Serialize<short>(PS1Demo_Unk3, name: nameof(PS1Demo_Unk3));
            }
            else
            {
                if (IsPCFormat(s.GameSettings))
                    Uint_30 = s.Serialize<uint>(Uint_30, name: nameof(Uint_30));

                EventIndex = s.Serialize<short>(EventIndex, name: nameof(EventIndex));
                ScreenXPosition = s.Serialize<short>(ScreenXPosition, name: nameof(ScreenXPosition));
                ScreenYPosition = s.Serialize<short>(ScreenYPosition, name: nameof(ScreenYPosition));
                Short_3A = s.Serialize<short>(Short_3A, name: nameof(Short_3A));
            }

            InitialXPosition = s.Serialize<short>(InitialXPosition, name: nameof(InitialXPosition));
            InitialYPosition = s.Serialize<short>(InitialYPosition, name: nameof(InitialYPosition));

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3)
            {
                PS1Demo_IsFlipped = s.Serialize<bool>(PS1Demo_IsFlipped, name: nameof(PS1Demo_IsFlipped)); // This is stored as a short in the game, but used as a bool
                PS1Demo_Padding = s.Serialize<byte>(PS1Demo_Padding, name: nameof(PS1Demo_Padding));
            }

            SpeedX = s.Serialize<short>(SpeedX, name: nameof(SpeedX));
            SpeedY = s.Serialize<short>(SpeedY, name: nameof(SpeedY));

            ImageDescriptorCount = s.Serialize<ushort>(ImageDescriptorCount, name: nameof(ImageDescriptorCount));

            CMD_CurrentOffset = s.Serialize<short>(CMD_CurrentOffset, name: nameof(CMD_CurrentOffset));
            CMD_Arg0 = s.Serialize<short>(CMD_Arg0, name: nameof(CMD_Arg0));

            Short_4A = s.Serialize<short>(Short_4A, name: nameof(Short_4A));
            Short_4C = s.Serialize<short>(Short_4C, name: nameof(Short_4C));
            Short_4E = s.Serialize<short>(Short_4E, name: nameof(Short_4E));

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PC_Kit || 
                s.GameSettings.EngineVersion == EngineVersion.R1_PC_Edu || 
                s.GameSettings.EngineVersion == EngineVersion.R1_PS1_Edu)
                EDU_ExtHitPoints = s.Serialize<uint>(EDU_ExtHitPoints, name: nameof(EDU_ExtHitPoints));

            CMD_Arg1 = s.Serialize<short>(CMD_Arg1, name: nameof(CMD_Arg1));
            Short_52 = s.Serialize<short>(Short_52, name: nameof(Short_52));
            Short_54 = s.Serialize<short>(Short_54, name: nameof(Short_54));
            Short_56 = s.Serialize<short>(Short_56, name: nameof(Short_56));
            
            Short_58 = s.Serialize<short>(Short_58, name: nameof(Short_58));
            Short_5A = s.Serialize<short>(Short_5A, name: nameof(Short_5A));
            Runtime_TypeZDC = s.SerializeObject<R1_ZDCEntry>(Runtime_TypeZDC, name: nameof(Runtime_TypeZDC));
            Short_5E = s.Serialize<short>(Short_5E, name: nameof(Short_5E));

            if (IsPCFormat(s.GameSettings))
                Type = s.Serialize<R1_EventType>(Type, name: nameof(Type));

            CollisionTypes = s.SerializeArray<R1_TileCollisionType>(CollisionTypes, s.GameSettings.EngineVersion != EngineVersion.R1_PS1_JPDemoVol3 ? 5 : 1, name: nameof(CollisionTypes));
            Byte_67 = s.Serialize<byte>(Byte_67, name: nameof(Byte_67));

            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));

            RuntimeCurrentAnimIndex = s.Serialize<byte>(RuntimeCurrentAnimIndex, name: nameof(RuntimeCurrentAnimIndex));
            RuntimeCurrentAnimFrame = s.Serialize<byte>(RuntimeCurrentAnimFrame, name: nameof(RuntimeCurrentAnimFrame));

            if (IsPCFormat(s.GameSettings))
            {
                SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
                Etat = s.Serialize<byte>(Etat, name: nameof(Etat));

                InitialSubEtat = s.Serialize<byte>(InitialSubEtat, name: nameof(InitialSubEtat));
                InitialEtat = s.Serialize<byte>(InitialEtat, name: nameof(InitialEtat));
            }
            else
            {
                Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
                InitialEtat = s.Serialize<byte>(InitialEtat, name: nameof(InitialEtat));
                SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
                InitialSubEtat = s.Serialize<byte>(InitialSubEtat, name: nameof(InitialSubEtat));
            }

            CMD_CurrentCommand = s.Serialize<uint>(CMD_CurrentCommand, name: nameof(CMD_CurrentCommand));

            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
                PS1_Flags = s.Serialize<byte>(PS1_Flags, name: nameof(PS1_Flags));

            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));
            HitPoints = s.Serialize<byte>(HitPoints, name: nameof(HitPoints));
            InitialHitPoints = s.Serialize<byte>(InitialHitPoints, name: nameof(InitialHitPoints));
            DisplayPrio = s.Serialize<byte>(DisplayPrio, name: nameof(DisplayPrio));

            if (!IsPCFormat(s.GameSettings))
                Type = (R1_EventType)s.Serialize<byte>((byte)Type, name: nameof(Type));

            HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));

            if (!IsPCFormat(s.GameSettings))
                PS1_Unk5 = s.Serialize<byte>(PS1_Unk5, name: nameof(PS1_Unk5));

            Byte_7A = s.Serialize<byte>(Byte_7A, name: nameof(Byte_7A));
            Byte_7B = s.Serialize<byte>(Byte_7B, name: nameof(Byte_7B));
            CMD_CurrentContext = s.Serialize<byte>(CMD_CurrentContext, name: nameof(CMD_CurrentContext));
            Byte_7D = s.Serialize<byte>(Byte_7D, name: nameof(Byte_7D));

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

            InitialDisplayPrio = s.Serialize<byte>(InitialDisplayPrio, name: nameof(InitialDisplayPrio));
            Byte_7F = s.Serialize<byte>(Byte_7F, name: nameof(Byte_7F));

            AnimDescriptorCount = s.Serialize<byte>(AnimDescriptorCount, name: nameof(AnimDescriptorCount));

            if (IsPCFormat(s.GameSettings))
            {
                PC_Flags = s.Serialize<PC_EventFlags>(PC_Flags, name: nameof(PC_Flags));
                Ushort_82 = s.Serialize<ushort>(Ushort_82, name: nameof(Ushort_82));
            }
            else
            {
                if (s.GameSettings.EngineVersion != EngineVersion.R1_PS1_JPDemoVol3)
                {
                    if (s.GameSettings.EngineVersion != EngineVersion.R1_PS1_JPDemoVol6)
                    {
                        PS1_RuntimeFlags = s.Serialize<PS1_EventFlags>(PS1_RuntimeFlags, name: nameof(PS1_RuntimeFlags));
                        PS1_Flags = s.Serialize<byte>(PS1_Flags, name: nameof(PS1_Flags));
                    }

                    PS1_Unk7 = s.Serialize<byte>(PS1_Unk7, name: nameof(PS1_Unk7));
                }
            }

            // Parse data from pointers only on PS1 and if we're not reading from processed memory
            if (!IsPCFormat(s.GameSettings) && !(Offset?.file is ProcessMemoryStreamFile) && s.FullSerialize)
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
                                curLength += (uint)(img.Width / 2) * img.Height;
                            else if (img.ImageType == 3)
                                curLength += (uint)img.Width * img.Height;

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
                if (LabelOffsetsPointer != null && Commands != null && Commands.Commands.Length > 0)
                {
                    s.DoAt(LabelOffsetsPointer, () =>
                    {
                        if (LabelOffsets == null) {
                            int length = Commands.Commands.Max(c => c.UsesLabelOffsets ? (int)c.Arguments[0] : -1) + 1;

                            LabelOffsets = new ushort[length];
                        }
                        // Serialize the label offsets
                        LabelOffsets = s.SerializeArray(LabelOffsets, LabelOffsets.Length, name: nameof(LabelOffsets));
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

        public R1_EventData InitRayman(Context context, R1_EventData rayPos)
        {
            OffsetBX = 80;
            OffsetBY = (byte)(context.Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || context.Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6 ? 80 : 78);
            if (rayPos != null)
            {
                XPosition = rayPos.XPosition + rayPos.OffsetBX - OffsetBX;
                YPosition = rayPos.YPosition + rayPos.OffsetBY - OffsetBY;
            }
            else
            {
                XPosition = 100;
                YPosition = 10;
            }
            Type = R1_EventType.TYPE_RAYMAN;
            SubEtat = 2;
            Etat = 2;
            OffsetHY = 20;
            FollowSprite = 0;
            HitPoints = 0;
            InitialDisplayPrio = DisplayPrio = 7;
            HitSprite = 0;

            PC_ImageDescriptorsIndex = 1;
            PC_AnimationDescriptorsIndex = 1;
            PC_ImageBufferIndex = 1;
            PC_ETAIndex = 0;

            CMD_Contexts = new CommandContext[]
            {
                new CommandContext()
            };
            CollisionTypes = new R1_TileCollisionType[5];

            Commands = new R1_EventCommandCollection()
            {
                Commands = new R1_EventCommand[0]
            };
            LabelOffsets = new ushort[0];

            return this;
        }

        // Copied from INIT_CHEMIN
        public R1_EventData InitMapObj(Context context, short x, short y, int index)
        {
            Type = R1_EventType.TYPE_MEDAILLON;
            Etat = 5;

            // Set correct sub-etat and position
            if (context.Settings.EngineVersion == EngineVersion.R1_PC_Kit)
            {
                SubEtat = 69;

                // ?
                XPosition = x - 34;
                YPosition = y - 39;

                // ?
                //OffsetBX = 80;
                //OffsetBY = 64;
            }
            else if (context.Settings.EngineVersion == EngineVersion.R1_PC_Edu || context.Settings.EngineVersion == EngineVersion.R1_PS1_Edu)
            {
                if (index == 0) // Normal
                    SubEtat = 39;
                else if (index == 2) // End
                    SubEtat = 55;
                else if (index == 4) // Demo
                    SubEtat = 54;
                else if (index == 3) // Start point
                    SubEtat = 45;

                // ?
                XPosition = x - 34 - 36;
                YPosition = y - 39 - 25;

                // ?
                //OffsetBX = 80;
                //OffsetBY = 64;
            }
            else
            {
                XPosition = x - 70; // In the code it's 78 - why do we have to offset it differently here?
                YPosition = y - 64;

                OffsetBX = 80;
                OffsetBY = 64;

                // Mr Dark
                if (index == 17)
                    SubEtat = 59;
                else if (index > 17)
                    SubEtat = 58;
                else
                    SubEtat = 39;
            }

            PC_ImageDescriptorsIndex = 4;
            PC_AnimationDescriptorsIndex = 4;
            PC_ImageBufferIndex = 4;
            PC_ETAIndex = 2;

            CMD_Contexts = new CommandContext[]
            {
                new CommandContext()
            };
            CollisionTypes = new R1_TileCollisionType[5];

            Commands = new R1_EventCommandCollection()
            {
                Commands = new R1_EventCommand[0]
            };
            LabelOffsets = new ushort[0];

            return this;
        }

        /// <summary>
        /// Flags for an event on PC. All values are runtime only except for FollowEnabled.
        /// </summary>
        [Flags]
        public enum PC_EventFlags : byte
        {
            None = 0,

            UnkFlag_0 = 1 << 0,

            /// <summary>
            /// A flag used for commands
            /// </summary>
            Test = 1 << 1,

            /// <summary>
            /// Indicates if the event should be drawn on screen
            /// </summary>
            SwitchedOn = 1 << 2,

            /// <summary>
            /// Indicates if the event should be flipped
            /// </summary>
            IsFlipped = 1 << 3,

            UnkFlag_4 = 1 << 4,
            
            /// <summary>
            /// Indicates if the event has collision
            /// </summary>
            FollowEnabled = 1 << 5,

            UnkFlag_6 = 1 << 6,

            // Appears related to the displaying animation. Changes a lot when an animation is playing.
            UnkFlag_7 = 1 << 7,
        }

        [Flags]
        public enum PS1_EventFlags : byte
        {
            None = 0,

            UnkFlag_0 = 1 << 0,
            UnkFlag_1 = 1 << 1,
            UnkFlag_2 = 1 << 2,
            SwitchedOn = 1 << 3,
            UnkFlag_4 = 1 << 4,
            UnkFlag_5 = 1 << 5,
            IsFlipped = 1 << 6,
            UnkFlag_7 = 1 << 7,
        }

        public class CommandContext : R1Serializable
        {
            /// <summary>
            /// The offset where the context was stored, used to remember where to jump back to after execution of the sub-function has finished
            /// </summary>
            public ushort CmdOffset { get; set; }

            /// <summary>
            /// The amount of times the execution should repeat before continuing, used for loops
            /// </summary>
            public ushort Count { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                CmdOffset = s.Serialize<ushort>(CmdOffset, name: nameof(CmdOffset));
                Count = s.Serialize<ushort>(Count, name: nameof(Count));
            }
        }
    }
}