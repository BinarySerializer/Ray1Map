using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_Event : R1Serializable
    {
        #region Event Properties

        /// <summary>
        /// The pointer to the image descriptors
        /// </summary>
        public Pointer ImageDescriptorsPointer { get; set; }

        /// <summary>
        /// The pointer to the animation descriptors
        /// </summary>
        public Pointer AnimDescriptorsPointer { get; set; }

        // Never valid, except for vol3 demo
        public Pointer ImageBufferPointer { get; set; }

        /// <summary>
        /// Pointer to ETA information
        /// </summary>
        public Pointer ETAPointer { get; set; }

        /// <summary>
        /// Pointer to the event commands
        /// </summary>
        public Pointer CommandsPointer { get; set; }

        /// <summary>
        /// The pointer to the command label offsets
        /// </summary>
        public Pointer LabelOffsetsPointer { get; set; }

        // Always 0
        public uint Unknown1 { get; set; }

        public byte[] UnkDemo1 { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public ushort XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public ushort YPosition { get; set; }

        public byte[] Unknown2 { get; set; }

        /// <summary>
        /// The amount of image descriptors
        /// </summary>
        public ushort ImageDescriptorCount { get; set; }

        public ushort Unknown4 { get; set; }

        // Always 254?
        public ushort Unknown5 { get; set; }

        public byte[] Unknown6 { get; set; }

        public byte OffsetBX { get; set; }

        public byte OffsetBY { get; set; }

        public byte RuntimeCurrentAnimIndex { get; set; }

        public byte RuntimeCurrentAnimFrame { get; set; }

        public byte Etat { get; set; }

        public byte Unknown8 { get; set; }
        
        public byte SubEtat { get; set; }

        public byte Unknown9 { get; set; }

        public ushort Unknown10 { get; set; }

        public ushort Unknown11 { get; set; }

        public byte OffsetHY { get; set; }

        public byte UnkDemo2 { get; set; }

        /// <summary>
        /// The sprite index which uses the offset collision
        /// </summary>
        public byte FollowSprite { get; set; }

        public byte Hitpoints { get; set; }

        public byte Unknown15 { get; set; }

        /// <summary>
        /// The layer the event sprite gets drawn to, between 1 and 7
        /// </summary>
        public byte Layer { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        public EventType Type { get; set; }

        public byte HitSprite { get; set; }

        public byte[] Unknown12 { get; set; }

        /// <summary>
        /// The amount of animation descriptors
        /// </summary>
        public byte AnimDescriptorCount { get; set; }
        
        public byte Unknown13 { get; set; }

        public bool GetFollowEnabled(GameSettings settings)
        {
            var offset = settings.EngineVersion == EngineVersion.RaySaturn ? 7 : 0;

            return BitHelpers.ExtractBits(Flags, 1, offset) == 1;
        }

        public void SetFollowEnabled(GameSettings settings, bool value)
        {
            var offset = settings.EngineVersion == EngineVersion.RaySaturn ? 7 : 0;

            BitHelpers.SetBits(Flags, value ? 1 : 0, 1, offset);
        }

        // TODO: Is this value not used for the vol3 & vol6 demos? How is follow enabled determined?
        public byte Flags { get; set; }

        public byte Unknown14 { get; set; }

        #endregion

        #region Parsed From Pointers

        /// <summary>
        /// The image descriptors
        /// </summary>
        public Common_ImageDescriptor[] ImageDescriptors { get; set; }

        /// <summary>
        /// The animation descriptors
        /// </summary>
        public PS1_R1_AnimationDescriptor[] AnimDescriptors { get; set; }

        /// <summary>
        /// The event commands
        /// </summary>
        public Common_EventCommandCollection Commands { get; set; }

        /// <summary>
        /// The command label offsets
        /// </summary>
        public ushort[] LabelOffsets { get; set; }

        /// <summary>
        /// The number of Etats
        /// </summary>
        public uint NumEtats { get; set; }

        /// <summary>
        /// The numbers of SubEtats
        /// </summary>
        public uint[] NumSubEtats { get; set; }

        /// <summary>
        /// Pointers to the ETA descriptors
        /// </summary>
        public Pointer[] EtatPointers { get; set; }

        /// <summary>
        /// Collection of states and substates for the event
        /// </summary>
        public Common_EventState[][] EventStates { get; set; }

        /// <summary>
        /// Image buffer
        /// </summary>
        public byte[] ImageBuffer { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize pointers
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            AnimDescriptorsPointer = s.SerializePointer(AnimDescriptorsPointer, name: nameof(AnimDescriptorsPointer));
            ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
            ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));
            CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6)
            {
                UnkDemo1 = s.SerializeArray<byte>(UnkDemo1, 46, name: nameof(UnkDemo1));
            }
            else
            {
                LabelOffsetsPointer = s.SerializePointer(LabelOffsetsPointer, name: nameof(LabelOffsetsPointer));
                Unknown1 = s.Serialize<uint>(Unknown1, name: nameof(Unknown1));
            }

            // Serialize position
            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));
            
            // Serialize unknown properties
            Unknown2 = s.SerializeArray<byte>(Unknown2, s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 ? 12 : s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6 ? 10 : 16, name: nameof(Unknown2));
            ImageDescriptorCount = s.Serialize<ushort>(ImageDescriptorCount, name: nameof(ImageDescriptorCount));

            if (s.GameSettings.EngineVersion != EngineVersion.RayPS1JPDemoVol3)
            {
                Unknown4 = s.Serialize<ushort>(Unknown4, name: nameof(Unknown4));
                Unknown5 = s.Serialize<ushort>(Unknown5, name: nameof(Unknown5));
            }

            Unknown6 = s.SerializeArray<byte>(Unknown6, 28, name: nameof(Unknown6));

            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));

            RuntimeCurrentAnimIndex = s.Serialize<byte>(RuntimeCurrentAnimIndex, name: nameof(RuntimeCurrentAnimIndex));
            RuntimeCurrentAnimFrame = s.Serialize<byte>(RuntimeCurrentAnimFrame, name: nameof(RuntimeCurrentAnimFrame));

            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
            Unknown8 = s.Serialize<byte>(Unknown8, name: nameof(Unknown8));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            Unknown9 = s.Serialize<byte>(Unknown9, name: nameof(Unknown9));

            Unknown10 = s.Serialize<ushort>(Unknown10, name: nameof(Unknown10));
            Unknown11 = s.Serialize<ushort>(Unknown11, name: nameof(Unknown11));

            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6)
                UnkDemo2 = s.Serialize<byte>(UnkDemo2, name: nameof(UnkDemo2));

            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));

            Hitpoints = s.Serialize<byte>(Hitpoints, name: nameof(Hitpoints));

            Unknown15 = s.Serialize<byte>(Unknown15, name: nameof(Unknown15));

            Layer = s.Serialize<byte>(Layer, name: nameof(Layer));

            Type = (EventType)s.Serialize<byte>((byte)Type, name: nameof(Type));

            HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));

            Unknown12 = s.SerializeArray<byte>(Unknown12, s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 ? 11 : s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6 ? 8 : 7, name: nameof(Unknown12));

            AnimDescriptorCount = s.Serialize<byte>(AnimDescriptorCount, name: nameof(AnimDescriptorCount));

            if (s.GameSettings.EngineVersion != EngineVersion.RayPS1JPDemoVol3)
            {
                Unknown13 = s.Serialize<byte>(Unknown13, name: nameof(Unknown13));

                if (s.GameSettings.EngineVersion != EngineVersion.RayPS1JPDemoVol6)
                {
                    Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
                    Unknown14 = s.Serialize<byte>(Unknown14, name: nameof(Unknown14));
                }
            }

            // Serialize the image descriptors
            s.DoAt(ImageDescriptorsPointer, () => {
                ImageDescriptors = s.SerializeObjectArray<Common_ImageDescriptor>(ImageDescriptors, ImageDescriptorCount, name: nameof(ImageDescriptors));
            });

            // Serialize the animation descriptors
            s.DoAt(AnimDescriptorsPointer, () => {
                AnimDescriptors = s.SerializeObjectArray<PS1_R1_AnimationDescriptor>(AnimDescriptors, AnimDescriptorCount, name: nameof(AnimDescriptors));
            });

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3) {
                if (ImageBuffer == null && ImageBufferPointer != null && ImageDescriptors != null) {
                    // Determine length of image buffer
                    uint length = 0;
                    foreach (Common_ImageDescriptor img in ImageDescriptors) {
                        if (img.ImageType != 2 && img.ImageType != 3) continue;
                        uint curLength = img.ImageBufferOffset;
                        if (img.ImageType == 2) {
                            curLength += (uint)(img.OuterWidth / 2) * img.OuterHeight;
                        } else if (img.ImageType == 3) {
                            curLength += (uint)img.OuterWidth * img.OuterHeight;
                        }
                        if (curLength > length) length = curLength;
                    }
                    ImageBuffer = new byte[length];
                }
                s.DoAt(ImageBufferPointer, () => {
                    ImageBuffer = s.SerializeArray<byte>(ImageBuffer, ImageBuffer.Length, name: nameof(ImageBuffer));
                });
            }

            // TODO: Serialize commands in the demos. They appear to be formatted differently as the invalid command is 0x42 rather then 0x21 - is everything doubled? Why are there no labels? Does it use local labels instead?
            if (s.GameSettings.EngineVersion != EngineVersion.RayPS1JPDemoVol3 && s.GameSettings.EngineVersion != EngineVersion.RayPS1JPDemoVol6)
            {
                // Serialize the commands
                if (CommandsPointer != null)
                {
                    s.DoAt(CommandsPointer, () => {
                        Commands = s.SerializeObject(Commands, name: nameof(Commands));
                    });
                }

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
            }

            // Serialize ETA

            // TODO: The ETA hack doesn't work on vol6 as it doesn't have them in the same structure as other versions (?). The length of a state is also 12 so the Common_EventState class needs to be updated!
            if (s.GameSettings.EngineVersion != EngineVersion.RayPS1JPDemoVol6)
            {
                // Get number of ETAs, hack
                if (s is BinaryDeserializer)
                {
                    s.DoAt(ETAPointer, () => {
                        Pointer p = s.SerializePointer(null, name: "FirstEtat");
                        if (p.file != ETAPointer.file
                        || p.AbsoluteOffset < ETAPointer.AbsoluteOffset + 4
                        || (p.AbsoluteOffset - ETAPointer.AbsoluteOffset) % 4 != 0
                        || (p.AbsoluteOffset - ETAPointer.AbsoluteOffset) / 4 <= Etat)
                        {
                            Debug.LogWarning("Number of ETAs wasn't correctly determined");
                        }
                        NumEtats = (p.AbsoluteOffset - ETAPointer.AbsoluteOffset) / 4;
                    });
                }
                s.DoAt(ETAPointer, () => {
                    EtatPointers = s.SerializePointerArray(EtatPointers, NumEtats, name: nameof(EtatPointers));
                    if (NumSubEtats == null)
                    {
                        // Get number of subetats, hack
                        NumSubEtats = new uint[NumEtats];
                        for (int i = 0; i < EtatPointers.Length - 1; i++)
                        {
                            if (EtatPointers[i] != null)
                            {
                                if (EtatPointers[i + 1] != null)
                                {
                                    NumSubEtats[i] = (EtatPointers[i + 1].AbsoluteOffset - EtatPointers[i].AbsoluteOffset) / (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 ? 14u : s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6 ? 12u : 8);
                                }
                                else
                                {
                                    Debug.LogWarning("An Etat Pointer was null - Number of SubEtats couldn't be determined");
                                }
                            }
                        }
                        if (EtatPointers[NumEtats - 1] != null)
                        {
                            // TODO: Parse this last array
                            NumSubEtats[NumEtats - 1] =
                                // Temp fix so we don't read past the end of the stream - this however causes certain events to get the wrong state!
                                s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.RaySaturn || s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6 ? 8u
                                    : 20u;

                            //if (Etat == NumEtats - 1) {
                            //    NumSubEtats[NumEtats - 1] = (uint)SubEtat + 1;
                            //}
                        }
                    }
                    if (EventStates == null)
                    {
                        EventStates = new Common_EventState[NumEtats][];
                    }
                    for (int i = 0; i < EtatPointers.Length; i++)
                    {
                        s.DoAt(EtatPointers[i], () => {
                            EventStates[i] = s.SerializeObjectArray<Common_EventState>(EventStates[i], NumSubEtats[i], name: nameof(EventStates) + "[" + i + "]");
                        });
                    }
                });

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

                if (EventStates?.ElementAtOrDefault(Etat)?.ElementAtOrDefault(SubEtat) == null)
                    Debug.LogWarning($"Matching event state not found for event {Type} at {XPosition}x{YPosition} with E{Etat},SE{SubEtat}");
            }
        }

        #endregion
    }
}