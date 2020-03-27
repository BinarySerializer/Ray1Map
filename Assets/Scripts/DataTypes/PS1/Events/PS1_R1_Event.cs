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

        // Never valid
        public Pointer UnkPointer3 { get; set; }

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

        public ushort Unknown7 { get; set; }

        public ushort Etat { get; set; }

        public ushort SubEtat { get; set; }

        public ushort Unknown8 { get; set; }

        public ushort Unknown9 { get; set; }

        public byte OffsetHY { get; set; }

        public byte FollowSprite { get; set; }

        public ushort Hitpoints { get; set; }

        public byte UnkGroup { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        public EventType Type { get; set; }

        // NOTE: Maybe a byte?
        public ushort HitSprite { get; set; }

        public byte[] Unknown10 { get; set; }

        /// <summary>
        /// The amount of animation descriptors
        /// </summary>
        public ushort AnimDescriptorCount { get; set; }

        public bool FollowEnabled { get; set; }

        public byte Unknown11 { get; set; }

        #endregion

        #region Parsed From Pointers

        /// <summary>
        /// The image descriptors
        /// </summary>
        public PS1_R1_ImageDescriptor[] ImageDescriptors { get; set; }

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
        /// The SubEtat collection pointer for the ETA
        /// </summary>
        public Pointer ETASubEtatPointer { get; set; }

        /// <summary>
        /// The event ETA state
        /// </summary>
        public Common_EventState EventState { get; set; }

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
            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3));
            ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));
            CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));
            LabelOffsetsPointer = s.SerializePointer(LabelOffsetsPointer, name: nameof(LabelOffsetsPointer));
            Unknown1 = s.Serialize<uint>(Unknown1, name: nameof(Unknown1));

            // Debug checks
            if (Unknown1 != 0)
                Debug.Log($"PS1 event unk1 is {Unknown1}");
            if (UnkPointer3 != null)
                Debug.Log($"PS1 event UnkPointer3 is {UnkPointer3}");

            // Serialize position
            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));

            // Serialize unknown properties
            Unknown2 = s.SerializeArray<byte>(Unknown2, 16, name: nameof(Unknown2));
            ImageDescriptorCount = s.Serialize<ushort>(ImageDescriptorCount, name: nameof(ImageDescriptorCount));
            Unknown4 = s.Serialize<ushort>(Unknown4, name: nameof(Unknown4));
            Unknown5 = s.Serialize<ushort>(Unknown5, name: nameof(Unknown5));
            Unknown6 = s.SerializeArray<byte>(Unknown6, 28, name: nameof(Unknown6));

            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));

            Unknown7 = s.Serialize<ushort>(Unknown7, name: nameof(Unknown7));

            Etat = s.Serialize<ushort>(Etat, name: nameof(Etat));
            SubEtat = s.Serialize<ushort>(SubEtat, name: nameof(SubEtat));

            Unknown8 = s.Serialize<ushort>(Unknown8, name: nameof(Unknown8));
            Unknown9 = s.Serialize<ushort>(Unknown9, name: nameof(Unknown9));

            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));
            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));

            Hitpoints = s.Serialize<ushort>(Hitpoints, name: nameof(Hitpoints));

            UnkGroup = s.Serialize<byte>(UnkGroup, name: nameof(UnkGroup));

            Type = (EventType)s.Serialize<byte>((byte)Type, name: nameof(Type));

            HitSprite = s.Serialize<ushort>(HitSprite, name: nameof(HitSprite));

            Unknown10 = s.SerializeArray<byte>(Unknown10, 6, name: nameof(Unknown10));

            AnimDescriptorCount = s.Serialize<ushort>(AnimDescriptorCount, name: nameof(AnimDescriptorCount));

            FollowEnabled = s.Serialize<bool>(FollowEnabled, name: nameof(FollowEnabled));
            Unknown11 = s.Serialize<byte>(Unknown11, name: nameof(Unknown11));

            // Serialize the image descriptors
            s.DoAt(ImageDescriptorsPointer, () => {
                ImageDescriptors = s.SerializeObjectArray<PS1_R1_ImageDescriptor>(ImageDescriptors, ImageDescriptorCount, name: nameof(ImageDescriptors));
            });

            // Serialize the animation descriptors
            s.DoAt(AnimDescriptorsPointer, () => {
                AnimDescriptors = s.SerializeObjectArray<PS1_R1_AnimationDescriptor>(AnimDescriptors, AnimDescriptorCount, name: nameof(AnimDescriptors));
            });

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

            // Serialize ETA
            s.DoAt(ETAPointer + (Etat * 4), () =>
            {
                // Get the state-array pointer
                ETASubEtatPointer = s.SerializePointer(ETASubEtatPointer, name: nameof(ETASubEtatPointer));

                // Serialize event state
                s.DoAt(ETASubEtatPointer + (SubEtat * 8), () =>
                {
                    EventState = s.SerializeObject(EventState, name: nameof(EventState));
                });
            });
        }

        #endregion
    }
}