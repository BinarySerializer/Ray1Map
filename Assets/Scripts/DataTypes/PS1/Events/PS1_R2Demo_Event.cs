namespace R1Engine
{
    // TODO: Clean up this class

    /// <summary>
    /// Event data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_Event : R1Serializable {
        public ushort UShort_00 { get; set; }
        public ushort UShort_02 { get; set; }
        public ushort UShort_04 { get; set; }
        public ushort UShort_06 { get; set; }
        public ushort UShort_08 { get; set; }
        public ushort UShort_0A { get; set; }
        
        // 12 (0xC)
        
        // Commands? This sets the object behavior.
        public Pointer BehaviorPointer { get; set; }

        // Leads to 16-byte long structures for collision data
        public Pointer CollisionDataPointer { get; set; }

        public Pointer AnimGroupPointer { get; set; }

        // Always 0 in file - gets set to a pointer to a function during runtime which gets called whenever the event is initialized
        public uint p_stHandlers { get; set; }
        
        // 28 (0x1C)

        /// <summary>
        /// The event x position on the map
        /// </summary>
        public ushort XPosition { get; set; }

        /// <summary>
        /// The event y position on the map
        /// </summary>
        public ushort YPosition { get; set; }

        // 32 (0x20)

        public byte Etat { get; set; }
        public byte SubEtat { get; set; }
        public byte UnkStateRelatedValue { get; set; }

        // 24 (0x22)

        // Between 40-44 is where x and y pos is stored during runtime
        public byte[] Unk1 { get; set; }

        // 56 (0x38)

        // Dev pointer in file - gets set to a pointer to the current event state during runtime
        public uint RuntimeCurrentStatePointer { get; set; }

        // Always 0 in file - gets set to a pointer during runtime
        public uint RuntimePointer3 { get; set; }

        // 64 (0x40)

        // Unknown value - probably runtime only?
        public ushort RuntimeUnk1 { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        public ushort EventType { get; set; }

        // 68 (0x44)

        // These values always match the position. They get copied to Position2 during runtime which is in Unk1. According to source code it's 0x28 and 0x2C (as ints), but that doesn't match files.
        public ushort XPosition3 { get; set; }
        public ushort YPosition3 { get; set; }

        // 72 (0x48)

        // Some runtime offset for the current sprite animation - setting to 0 places sprite in its origin point during runtime
        public ushort RuntimeOffset1 { get; set; }
        public ushort RuntimeOffset2 { get; set; }

        // 76 (0x4C)

        // 0x4C and 0x4E are ushorts

        // Second byte in here determines horizontal speed and fourth byte the vertical speed
        // Always 0 in files except last 2 bytes
        public byte[] RuntimeBytes1 { get; set; }

        // 84 (0x54)

        /// <summary>
        /// The current animation frame during runtime
        /// </summary>
        public byte RuntimeCurrentAnimFrame { get; set; }

        public byte RuntimeEtat { get; set; }
        public byte RuntimeSubEtat { get; set; }
        public byte RuntimeUnkStateRelatedValue { get; set; }
        public byte Unk_58 { get; set; }

        // The layer to appear on (0-7)
        public byte Layer { get; set; }

        // 90 (0x5A)

        public byte[] Unk3 { get; set; }

        /*
         
           *(undefined *)(eventOffset + 0x52) = *(undefined *)(eventOffset + 0x25);
           uVar3 = *(uint *)(eventOffset + 100);
           *(uint *)(eventOffset + 100) = uVar3 & 0x9fffffff;
           *(uint *)(eventOffset + 0x68) =
           *(uint *)(eventOffset + 0x68) & 0xffffff03 | *(uint *)(eventOffset + 0x24) >> 0xf & 0xfc;
           *(uint *)(eventOffset + 100) = uVar3 & 0x8fffffff | 0x4000000;
           *(undefined *)(eventOffset + 0x65) = 0;
           *(char *)(eventOffset + 100) = *(char *)(eventOffset + 0x24);
           if (1 < (byte)(*(char *)(eventOffset + 0x24) - 1U)) {
                *(undefined *)(eventOffset + 100) = 1;
           }
             
             */
        // Indicates if it's on the background map during runtime - always 0 in files
        public bool RuntimeIsOnBackgroundLayer { get; set; }

        public byte[] Unk4 { get; set; }

        /// <summary>
        /// Indicates if the event sprite should be flipped horizontally
        /// </summary>
        public bool IsFlippedHorizontally { get; set; }

        // Runtime only? What does it really do? - always 0 in files
        public bool IsFaded { get; set; }

        public byte[] Unk5 { get; set; }


        // Values parsed from pointers

        public byte[] CollisionDataValues { get; set; }

        public PS1_R2Demo_EventAnimGroup AnimGroup { get; set; }
        // TODO: Parse all available states
        public PS1_R2Demo_EventState CurrentState { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
            UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));
            UShort_04 = s.Serialize<ushort>(UShort_04, name: nameof(UShort_04));
            UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
            UShort_08 = s.Serialize<ushort>(UShort_08, name: nameof(UShort_08));
            UShort_0A = s.Serialize<ushort>(UShort_0A, name: nameof(UShort_0A));

            // Serialize pointers
            BehaviorPointer = s.SerializePointer(BehaviorPointer, name: nameof(BehaviorPointer));
            CollisionDataPointer = s.SerializePointer(CollisionDataPointer, name: nameof(CollisionDataPointer));
            AnimGroupPointer = s.SerializePointer(AnimGroupPointer, name: nameof(AnimGroupPointer));

            p_stHandlers = s.Serialize<uint>(p_stHandlers, name: nameof(p_stHandlers));

            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));

            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            UnkStateRelatedValue = s.Serialize<byte>(UnkStateRelatedValue, name: nameof(UnkStateRelatedValue));

            Unk1 = s.SerializeArray(Unk1, 21, name: nameof(Unk1));

            RuntimeCurrentStatePointer = s.Serialize<uint>(RuntimeCurrentStatePointer, name: nameof(RuntimeCurrentStatePointer));
            RuntimePointer3 = s.Serialize<uint>(RuntimePointer3, name: nameof(RuntimePointer3));

            RuntimeUnk1 = s.Serialize<ushort>(RuntimeUnk1, name: nameof(RuntimeUnk1));
            EventType = s.Serialize<ushort>(EventType, name: nameof(EventType));

            XPosition3 = s.Serialize<ushort>(XPosition3, name: nameof(XPosition3));
            YPosition3 = s.Serialize<ushort>(YPosition3, name: nameof(YPosition3));

            RuntimeOffset1 = s.Serialize<ushort>(RuntimeOffset1, name: nameof(RuntimeOffset1));
            RuntimeOffset2 = s.Serialize<ushort>(RuntimeOffset2, name: nameof(RuntimeOffset2));

            RuntimeBytes1 = s.SerializeArray(RuntimeBytes1, 8, name: nameof(RuntimeBytes1));
            RuntimeCurrentAnimFrame = s.Serialize<byte>(RuntimeCurrentAnimFrame, name: nameof(RuntimeCurrentAnimFrame));

            RuntimeEtat = s.Serialize<byte>(RuntimeEtat, name: nameof(RuntimeEtat));
            RuntimeSubEtat = s.Serialize<byte>(RuntimeSubEtat, name: nameof(RuntimeSubEtat));
            RuntimeUnkStateRelatedValue = s.Serialize<byte>(RuntimeUnkStateRelatedValue, name: nameof(RuntimeUnkStateRelatedValue));
            Unk_58 = s.Serialize<byte>(Unk_58, name: nameof(Unk_58));

            Layer = s.Serialize<byte>(Layer, name: nameof(Layer));

            Unk3 = s.SerializeArray(Unk3, 10, name: nameof(Unk3));

            RuntimeIsOnBackgroundLayer = s.Serialize<bool>(RuntimeIsOnBackgroundLayer, name: nameof(RuntimeIsOnBackgroundLayer));

            Unk4 = s.SerializeArray(Unk4, 3, name: nameof(Unk4));

            IsFlippedHorizontally = s.Serialize<bool>(IsFlippedHorizontally, name: nameof(IsFlippedHorizontally));
            IsFaded = s.Serialize<bool>(IsFaded, name: nameof(IsFaded));

            Unk5 = s.SerializeArray(Unk5, 2, name: nameof(Unk5));

            /*s.DoAt(UnkPointer3, () => {
                Pointer ptr = s.SerializePointer(null, name: "test");
                s.DoAt(ptr, () => {
                    s.SerializePointer(null, name: "test2"); // pointer to 16 byte long structs
                });
            });*/

            if (CollisionDataPointer != null)
            {
                s.DoAt(CollisionDataPointer, () =>
                {
                    CollisionDataValues = s.SerializeArray<byte>(CollisionDataValues, 16, name: nameof(CollisionDataValues));
                });
            }

            if (AnimGroupPointer != null)
            {
                s.DoAt(AnimGroupPointer, () =>
                {
                    AnimGroup = s.SerializeObject<PS1_R2Demo_EventAnimGroup>(AnimGroup, name: nameof(AnimGroup));

                    // TODO: Serialize all states
                    if (AnimGroup?.EtaPointer != null)
                    {
                        s.DoAt(AnimGroup.EtaPointer + (Etat * 4), () =>
                        {
                            var currentEtatPointer = s.SerializePointer(null, name: "currentEtatPointer");

                            s.DoAt(currentEtatPointer + (16 * SubEtat), () =>
                            {
                                CurrentState = s.SerializeObject<PS1_R2Demo_EventState>(CurrentState, name: nameof(CurrentState));
                            });
                        });
                    }
                });
            }
        }
    }

    // TODO: Move to separate file
    public class PS1_R2Demo_EventAnimGroup : R1Serializable
    {
        // Leads to the ETA pointer array which leads to each event state (16 bytes each)
        public Pointer EtaPointer { get; set; }

        public Pointer AnimationDescriptorsPointer { get; set; }

        public ushort AnimationDescriptorCount { get; set; }

        // Usually 0
        public ushort Unknown { get; set; }


        public PS1_R2Demo_AnimationDecriptor[] AnimationDecriptors { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            EtaPointer = s.SerializePointer(EtaPointer, name: nameof(EtaPointer));
            AnimationDescriptorsPointer = s.SerializePointer(AnimationDescriptorsPointer, name: nameof(AnimationDescriptorsPointer));

            AnimationDescriptorCount = s.Serialize<ushort>(AnimationDescriptorCount, name: nameof(AnimationDescriptorCount));
            Unknown = s.Serialize<ushort>(Unknown, name: nameof(Unknown));

            if (AnimationDescriptorsPointer != null)
            {
                s.DoAt(AnimationDescriptorsPointer, () =>
                {
                    AnimationDecriptors = s.SerializeObjectArray<PS1_R2Demo_AnimationDecriptor>(AnimationDecriptors, AnimationDescriptorCount, name: nameof(AnimationDecriptors));
                });
            }
        }
    }

    // TODO: Move to separate file and merge with normal anim desc class
    public class PS1_R2Demo_AnimationDecriptor : R1Serializable
    {
        // Leads to an array of pointers (size might match Unk1) which all lead to 4-byte struct arrays (of different sizes) which seem to be anim frames
        public Pointer UnkPointer1 { get; set; }

        public Pointer UnkPointer2 { get; set; }

        // Usually null
        public Pointer UnkPointer3 { get; set; }

        // These two are some counts for the anim data (layers and frames)
        public ushort Unk1 { get; set; }

        // Frame count
        public ushort Unk2 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
            UnkPointer2 = s.SerializePointer(UnkPointer2, name: nameof(UnkPointer2));
            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3));

            Unk1 = s.Serialize<ushort>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<ushort>(Unk2, name: nameof(Unk2));
        }
    }

    // TODO: Move to separate file and merge with normal event state class
    public class PS1_R2Demo_EventState : R1Serializable
    {
        // Right speed and left speed
        public byte[] Unk0 { get; set; }

        public byte AnimationIndex { get; set; }

        public byte[] Unk1 { get; set; }

        public byte LinkedEtat { get; set; }
        public byte LinkedSubEtat { get; set; }

        public byte Unk2 { get; set; }

        public byte AnimationSpeed { get; set; }

        public byte[] Unk3 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk0 = s.SerializeArray<byte>(Unk0, 4, name: nameof(Unk0));
            AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
            Unk1 = s.SerializeArray<byte>(Unk1, 5, name: nameof(Unk1));
            LinkedEtat = s.Serialize<byte>(LinkedEtat, name: nameof(LinkedEtat));
            LinkedSubEtat = s.Serialize<byte>(LinkedSubEtat, name: nameof(LinkedSubEtat));
            Unk2 = s.Serialize<byte>(Unk2, name: nameof(Unk2));
            AnimationSpeed = s.Serialize<byte>(AnimationSpeed, name: nameof(AnimationSpeed));
            Unk3 = s.SerializeArray<byte>(Unk3, 2, name: nameof(Unk3));
        }
    }

    // TODO: Create enum
    /*
     
    Event types for non-always events

    0
    2
    3 - 1up
    4 - Big power
    6
    9 - Floating mine
    11 - Fist reflector
    18 - Water lily
    19 - Flying ring
    24 - Teleport
    27
    34 - Scared platform
    36 - Trap cube
    38 - Trampoline
    48 - 
    49 - Rayman position
    52 - Hp potion?
    56
    81 - Cannon
    91
    95 - Destructable ground
    96
    97 - Ting
    98 - Dino
    102
    104

    Event types for always events

    4
    5
    8
    10
    13
    51
    76
    81
    82
    83
    92
    99
    100
    103

     */
}