using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_Events : R1Serializable
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
        public uint PS1_Unk1 { get; set; }

        public byte[] PS1Demo_Unk1 { get; set; }
        public uint PS1Demo_Unk2 { get; set; }
        public ushort PS1Demo_Unk3 { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public ushort XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public ushort YPosition { get; set; }

        // This index is used by the game to handle the event links
        public ushort EventIndex { get; set; }

        public byte[] PS1_Unk2 { get; set; }

        public ushort RuntimeXPosition { get; set; }
        public ushort RuntimeYPosition { get; set; }

        // 3 ushorts; Unk_64, Unk_66
        public byte[] PS1_Unk3 { get; set; }

        /// <summary>
        /// The amount of image descriptors
        /// </summary>
        public ushort ImageDescriptorCount { get; set; }

        public ushort PS1_Unk4 { get; set; }

        // Always 254?
        public ushort PS1_Unk5 { get; set; }

        // All ushorts from PC Unk_70-Unk_94 except last one which is only on PS1
        public byte[] PS1_Unk6 { get; set; }

        public byte OffsetBX { get; set; }
        public byte OffsetBY { get; set; }

        public byte RuntimeCurrentAnimIndex { get; set; }
        public byte RuntimeCurrentAnimFrame { get; set; }

        public byte Etat { get; set; }
        public byte RuntimeEtat { get; set; }
        
        public byte SubEtat { get; set; }
        public byte RuntimeSubEtat { get; set; }

        // These are uint Unk_112
        public ushort Unknown10 { get; set; }
        public ushort Unknown11 { get; set; }

        public byte OffsetHY { get; set; }

        /// <summary>
        /// The sprite index which uses the offset collision
        /// </summary>
        public byte FollowSprite { get; set; }

        public byte Hitpoints { get; set; }

        public byte RuntimeHitpoints { get; set; }

        /// <summary>
        /// The layer the event sprite gets drawn to, between 1 and 7
        /// </summary>
        public byte Layer { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        public EventType Type { get; set; }

        public byte HitSprite { get; set; }

        // First byte is PS1 only, rest are Unk_122-Unk_125
        public byte[] Unknown12 { get; set; }

        // Unk_127
        public byte RuntimeLayer { get; set; }
        public byte Unknown15 { get; set; }

        /// <summary>
        /// The amount of animation descriptors
        /// </summary>
        public byte AnimDescriptorCount { get; set; }
        
        // PS1 only - runtime flags?
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
        /// The event ETA
        /// </summary>
        public PS1_ETA ETA { get; set; }

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
                PS1Demo_Unk1 = s.SerializeArray<byte>(PS1Demo_Unk1, 40, name: nameof(PS1Demo_Unk1));

                EventIndex = s.Serialize<ushort>(EventIndex, name: nameof(EventIndex));

                PS1Demo_Unk2 = s.Serialize<uint>(PS1Demo_Unk2, name: nameof(PS1Demo_Unk2));
            }
            else
            {
                LabelOffsetsPointer = s.SerializePointer(LabelOffsetsPointer, name: nameof(LabelOffsetsPointer));
                PS1_Unk1 = s.Serialize<uint>(PS1_Unk1, name: nameof(PS1_Unk1));
            }

            // Serialize position
            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));
            
            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1 || s.GameSettings.EngineVersion == EngineVersion.RayPS1JP)
            {
                EventIndex = s.Serialize<ushort>(EventIndex, name: nameof(EventIndex));

                PS1_Unk2 = s.SerializeArray<byte>(PS1_Unk2, 6, name: nameof(PS1_Unk2));
            }
            else
            {
                PS1Demo_Unk3 = s.Serialize<ushort>(PS1Demo_Unk3, name: nameof(PS1Demo_Unk3));
            }

            RuntimeXPosition = s.Serialize<ushort>(RuntimeXPosition, name: nameof(RuntimeXPosition));
            RuntimeYPosition = s.Serialize<ushort>(RuntimeYPosition, name: nameof(RuntimeYPosition));

            PS1_Unk3 = s.SerializeArray<byte>(PS1_Unk3, s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 ? 6 : 4, name: nameof(PS1_Unk3));

            ImageDescriptorCount = s.Serialize<ushort>(ImageDescriptorCount, name: nameof(ImageDescriptorCount));

            PS1_Unk6 = s.SerializeArray<byte>(PS1_Unk6, 28, name: nameof(PS1_Unk6));

            if (s.GameSettings.EngineVersion != EngineVersion.RayPS1JPDemoVol3)
            {
                PS1_Unk4 = s.Serialize<ushort>(PS1_Unk4, name: nameof(PS1_Unk4));
                PS1_Unk5 = s.Serialize<ushort>(PS1_Unk5, name: nameof(PS1_Unk5));
            }

            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));

            RuntimeCurrentAnimIndex = s.Serialize<byte>(RuntimeCurrentAnimIndex, name: nameof(RuntimeCurrentAnimIndex));
            RuntimeCurrentAnimFrame = s.Serialize<byte>(RuntimeCurrentAnimFrame, name: nameof(RuntimeCurrentAnimFrame));

            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
            RuntimeEtat = s.Serialize<byte>(RuntimeEtat, name: nameof(RuntimeEtat));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            RuntimeSubEtat = s.Serialize<byte>(RuntimeSubEtat, name: nameof(RuntimeSubEtat));

            Unknown10 = s.Serialize<ushort>(Unknown10, name: nameof(Unknown10));
            Unknown11 = s.Serialize<ushort>(Unknown11, name: nameof(Unknown11));

            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6)
                Flags = s.Serialize<byte>(Flags, name: nameof(Flags));

            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));

            Hitpoints = s.Serialize<byte>(Hitpoints, name: nameof(Hitpoints));

            RuntimeHitpoints = s.Serialize<byte>(RuntimeHitpoints, name: nameof(RuntimeHitpoints));

            Layer = s.Serialize<byte>(Layer, name: nameof(Layer));

            Type = (EventType)s.Serialize<byte>((byte)Type, name: nameof(Type));

            HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));

            Unknown12 = s.SerializeArray<byte>(Unknown12, s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3 ? 9 : s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6 ? 6 : 5, name: nameof(Unknown12));

            RuntimeLayer = s.Serialize<byte>(RuntimeLayer, name: nameof(RuntimeLayer));

            // Always 0, even in memory
            Unknown15 = s.Serialize<byte>(Unknown15, name: nameof(Unknown15));

            AnimDescriptorCount = s.Serialize<byte>(AnimDescriptorCount, name: nameof(AnimDescriptorCount));

            if (s.GameSettings.EngineVersion != EngineVersion.RayPS1JPDemoVol3)
            {
                // Appears to be some form of runtime flags for if the event is in view, should be drawn etc. (not in demos though)
                Unknown13 = s.Serialize<byte>(Unknown13, name: nameof(Unknown13));

                if (s.GameSettings.EngineVersion != EngineVersion.RayPS1JPDemoVol6)
                {
                    Flags = s.Serialize<byte>(Flags, name: nameof(Flags));

                    // Always 0, even in memory
                    Unknown14 = s.Serialize<byte>(Unknown14, name: nameof(Unknown14));
                }
            }

            // Serialize the image descriptors
            s.DoAt(ImageDescriptorsPointer, () => ImageDescriptors = s.SerializeObjectArray<Common_ImageDescriptor>(ImageDescriptors, ImageDescriptorCount, name: nameof(ImageDescriptors)));

            // Serialize the animation descriptors
            s.DoAt(AnimDescriptorsPointer, () => AnimDescriptors = s.SerializeObjectArray<PS1_R1_AnimationDescriptor>(AnimDescriptors, AnimDescriptorCount, name: nameof(AnimDescriptors)));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3) {
                if (ImageBuffer == null && ImageBufferPointer != null && ImageDescriptors != null) {
                    // Determine length of image buffer
                    uint length = 0;
                    foreach (Common_ImageDescriptor img in ImageDescriptors) 
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
                s.DoAt(CommandsPointer, () => Commands = s.SerializeObject<Common_EventCommandCollection>(Commands, name: nameof(Commands)));

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
                s.DoAt(ETAPointer, () => ETA = s.SerializeObject<PS1_ETA>(ETA, name: nameof(ETA)));

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
                Debug.LogWarning($"Matching event state not found for event {Type} at {XPosition}x{YPosition} with E{Etat},SE{SubEtat}");
        }

        #endregion
    }
}