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
        public byte Type { get; set; }

        // NOTE: Maybe a byte?
        public ushort HitSprite { get; set; }

        public byte[] Unknown10 { get; set; }

        /// <summary>
        /// The amount of animation descriptors
        /// </summary>
        public ushort AnimDescriptorCount { get; set; }

        public ushort Unknown11 { get; set; }

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
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: "UnkPointer1");
            AnimDescriptorsPointer = s.SerializePointer(AnimDescriptorsPointer, name: "UnkPointer2");
            UnkPointer3 = s.SerializePointer(UnkPointer3, name: "UnkPointer3");
            ETAPointer = s.SerializePointer(ETAPointer, name: "UnkPointer4");
            CommandsPointer = s.SerializePointer(CommandsPointer, name: "CommandsPointer");
            LabelOffsetsPointer = s.SerializePointer(LabelOffsetsPointer, name: "UnkPointer5");
            Unknown1 = s.Serialize<uint>(Unknown1, name: "Unknown1");

            // Debug checks
            if (Unknown1 != 0)
                Debug.Log($"PS1 event unk1 is {Unknown1}");
            if (UnkPointer3 != null)
                Debug.Log($"PS1 event UnkPointer3 is {UnkPointer3}");

            // Serialize position
            XPosition = s.Serialize<ushort>(XPosition, name: "XPosition");
            YPosition = s.Serialize<ushort>(YPosition, name: "YPosition");

            // Serialize unknown properties
            Unknown2 = s.SerializeArray<byte>(Unknown2, 16, name: "Unknown2");
            ImageDescriptorCount = s.Serialize<ushort>(ImageDescriptorCount, name: "UnkPointer1Count");
            Unknown4 = s.Serialize<ushort>(Unknown4, name: "Unknown4");
            Unknown5 = s.Serialize<ushort>(Unknown5, name: "Unknown5");
            Unknown6 = s.SerializeArray<byte>(Unknown6, 28, name: "Unknown6");

            OffsetBX = s.Serialize<byte>(OffsetBX, name: "OffsetBX");
            OffsetBY = s.Serialize<byte>(OffsetBY, name: "OffsetBY");

            Unknown7 = s.Serialize<ushort>(Unknown7, name: "Unknown7");

            Etat = s.Serialize<ushort>(Etat, name: "Etat");
            SubEtat = s.Serialize<ushort>(SubEtat, name: "SubEtat");

            Unknown8 = s.Serialize<ushort>(Unknown8, name: "Unknown8");
            Unknown9 = s.Serialize<ushort>(Unknown9, name: "Unknown9");

            OffsetHY = s.Serialize<byte>(OffsetHY, name: "OffsetHY");
            FollowSprite = s.Serialize<byte>(FollowSprite, name: "FollowSprite");

            Hitpoints = s.Serialize<ushort>(Hitpoints, name: "Hitpoints");

            UnkGroup = s.Serialize<byte>(UnkGroup, name: "UnkGroup");

            Type = s.Serialize<byte>(Type, name: "Type");

            HitSprite = s.Serialize<ushort>(HitSprite, name: "HitSprite");

            Unknown10 = s.SerializeArray<byte>(Unknown10, 6, name: "Unknown10");

            AnimDescriptorCount = s.Serialize<ushort>(AnimDescriptorCount, name: "UnkPointer2Count");

            Unknown11 = s.Serialize<ushort>(Unknown11, name: "Unknown11");

            // Serialize the image descriptors
            s.DoAt(ImageDescriptorsPointer, () => {
                ImageDescriptors = s.SerializeObjectArray<PS1_R1_ImageDescriptor>(ImageDescriptors, ImageDescriptorCount, name: "ImageDescriptors");
            });

            // Serialize the animation descriptors
            s.DoAt(AnimDescriptorsPointer, () => {
                AnimDescriptors = s.SerializeObjectArray<PS1_R1_AnimationDescriptor>(AnimDescriptors, AnimDescriptorCount, name: "AnimDescriptors");
            });

            // Serialize the commands
            if (CommandsPointer != null)
            {
                s.DoAt(CommandsPointer, () => {
                    Commands = s.SerializeObject(Commands, name: "Commands");
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
                        s.SerializeArray(LabelOffsets, LabelOffsets.Length, name: "LabelOffsets");

                        // Null terminate it
                        s.Serialize((byte)0, name: "LabelOffsets NULL");
                    }
                });
            }

            // Serialize ETA
            s.DoAt(ETAPointer + (Etat * 4), () =>
            {
                // Get the state-array pointer
                ETASubEtatPointer = s.SerializePointer(ETASubEtatPointer, name: "ETAEtatPointer");

                // Serialize event state
                s.DoAt(ETASubEtatPointer + (SubEtat * 8), () =>
                {
                    EventState = s.SerializeObject(EventState, name: "EventState");
                });
            });
        }

        #endregion
    }
}