namespace R1Engine
{
    // TODO: Merge with PS1_R1_Event once all values have been parsed
    /// <summary>
    /// Event data for Rayman 1 (PS1 - Japan Demo Vol3)
    /// </summary>
    public class PS1_R1JPDemoVol3_Event : R1Serializable
    {
        /// <summary>
        /// The pointer to the image descriptors
        /// </summary>
        public Pointer ImageDescriptorsPointer { get; set; }

        /// <summary>
        /// The pointer to the animation descriptors
        /// </summary>
        public Pointer AnimDescriptorsPointer { get; set; }

        public Pointer UnknownPointer1 { get; set; }

        public Pointer ETAPointer { get; set; }

        /// <summary>
        /// Pointer to the event commands
        /// </summary>
        public Pointer CommandsPointer { get; set; }

        public byte[] Unk1 { get; set; }

        public ushort XPosition { get; set; }

        public ushort YPosition { get; set; }

        public byte[] Unk2 { get; set; }

        public ushort ImageDescriptorCount { get; set; }

        public byte[] Unk3 { get; set; }

        public byte OffsetBX { get; set; }

        public byte OffsetBY { get; set; }

        public ushort Unknown7 { get; set; }

        public byte Etat { get; set; }

        public byte Unk6 { get; set; }

        public byte SubEtat { get; set; }
        
        public byte Unk7 { get; set; }

        public ushort Unknown8 { get; set; }

        public ushort Unknown9 { get; set; }

        public byte OffsetHY { get; set; }

        public byte Unk4 { get; set; }

        /// <summary>
        /// The sprite index which uses the offset collision
        /// </summary>
        public byte FollowSprite { get; set; }

        public ushort Hitpoints { get; set; }

        /// <summary>
        /// The layer the event sprite gets drawn to, between 1 and 7
        /// </summary>
        public byte Layer { get; set; }

        public EventType EventType { get; set; }

        public ushort HitSprite { get; set; }

        public byte[] Unk5 { get; set; }

        // ushort?
        public byte AnimDescriptorCount { get; set; }

        /// <summary>
        /// The image descriptors
        /// </summary>
        public PS1_R1_ImageDescriptor[] ImageDescriptors { get; set; }

        /// <summary>
        /// The animation descriptors
        /// </summary>
        public PS1_R1_AnimationDescriptor[] AnimDescriptors { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize pointers
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            AnimDescriptorsPointer = s.SerializePointer(AnimDescriptorsPointer, name: nameof(AnimDescriptorsPointer));
            UnknownPointer1 = s.SerializePointer(UnknownPointer1, name: nameof(UnknownPointer1));
            ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));
            CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));

            // Serialize values
            Unk1 = s.SerializeArray<byte>(Unk1, 46, name: nameof(Unk1));

            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));
            
            Unk2 = s.SerializeArray<byte>(Unk2, 12, name: nameof(Unk2));

            ImageDescriptorCount = s.Serialize<ushort>(ImageDescriptorCount, name: nameof(ImageDescriptorCount));

            Unk3 = s.SerializeArray<byte>(Unk3, 28, name: nameof(Unk3));
            
            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));
            Unknown7 = s.Serialize<ushort>(Unknown7, name: nameof(Unknown7));
            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
            Unk6 = s.Serialize<byte>(Unk6, name: nameof(Unk6));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            Unk7 = s.Serialize<byte>(Unk7, name: nameof(Unk7));

            Unknown8 = s.Serialize<ushort>(Unknown8, name: nameof(Unknown8));
            Unknown9 = s.Serialize<ushort>(Unknown9, name: nameof(Unknown9));

            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));

            Unk4 = s.Serialize<byte>(Unk4, name: nameof(Unk4));
            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));

            Hitpoints = s.Serialize<ushort>(Hitpoints, name: nameof(Hitpoints));
            Layer = s.Serialize<byte>(Layer, name: nameof(Layer));

            EventType = (EventType)s.Serialize<byte>((byte)EventType, name: nameof(EventType));
            HitSprite = s.Serialize<ushort>(HitSprite, name: nameof(HitSprite));

            Unk5 = s.SerializeArray<byte>(Unk5, 10, name: nameof(Unk5));
            AnimDescriptorCount = s.Serialize<byte>(AnimDescriptorCount, name: nameof(AnimDescriptorCount));

            s.DoAt(ImageDescriptorsPointer, () => {
                ImageDescriptors = s.SerializeObjectArray<PS1_R1_ImageDescriptor>(ImageDescriptors, ImageDescriptorCount, name: nameof(ImageDescriptors));
            });

            s.DoAt(AnimDescriptorsPointer, () => {
                AnimDescriptors = s.SerializeObjectArray<PS1_R1_AnimationDescriptor>(AnimDescriptors, AnimDescriptorCount, name: nameof(AnimDescriptors));
            });
        }
    }
}