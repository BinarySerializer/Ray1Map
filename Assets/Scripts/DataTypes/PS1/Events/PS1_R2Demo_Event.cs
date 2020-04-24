namespace R1Engine
{
    // Value on decimal 100 is a uint flag?

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
        
        public Pointer UnkPointer1 { get; set; }

        // Leads to 16-byte long structures
        public Pointer UnkPointer2 { get; set; }

        // Leads to 12-byte long structures. Usually it's {Pointer1, Pointer2, ushort1, ushort2}
        // Pointer1 leads to more pointers
        public Pointer UnkPointer3 { get; set; }

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

        // First two bytes are single byte values related to UnkPointer3 - they get copied to 0x55 and 0x56
        // Between 40-44 is where x and y pos is stored during runtime
        // 56-60 is for the current state or animation
        public byte[] Unk1 { get; set; }

        // 56 (0x38)

        // Dev pointer in file - gets set to a pointer during runtime (*(int *)((uint)Unk1[0] * 4 + **(int **)(UnkPointer3)) + (uint)Unk1[1] * 0x10)
        public uint RuntimePointer2 { get; set; }

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

        // Always 0 in file
        // Second byte in here determines horizontal speed and fourth byte the vertical speed
        // Last 2 bytes have values in files
        public byte[] RuntimeBytes1 { get; set; }

        // 84 (0x54)

        /// <summary>
        /// The current animation frame during runtime
        /// </summary>
        public byte RuntimeCurrentAnimFrame { get; set; }

        // First 3 bytes always match first 3 bytes of Unk1!
        // Probably not runtime related - might have to do with animations/state?
        public byte[] RuntimeBytes2 { get; set; }

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


        public byte[] UnkPointer2Values { get; set; }
        public byte[] UnkPointer3Values { get; set; }


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
            UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
            UnkPointer2 = s.SerializePointer(UnkPointer2, name: nameof(UnkPointer2));
            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3));

            p_stHandlers = s.Serialize<uint>(p_stHandlers, name: nameof(p_stHandlers));

            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));

            Unk1 = s.SerializeArray(Unk1, 24, name: nameof(Unk1));

            RuntimePointer2 = s.Serialize<uint>(RuntimePointer2, name: nameof(RuntimePointer2));
            RuntimePointer3 = s.Serialize<uint>(RuntimePointer3, name: nameof(RuntimePointer3));

            RuntimeUnk1 = s.Serialize<ushort>(RuntimeUnk1, name: nameof(RuntimeUnk1));
            EventType = s.Serialize<ushort>(EventType, name: nameof(EventType));

            XPosition3 = s.Serialize<ushort>(XPosition3, name: nameof(XPosition3));
            YPosition3 = s.Serialize<ushort>(YPosition3, name: nameof(YPosition3));

            RuntimeOffset1 = s.Serialize<ushort>(RuntimeOffset1, name: nameof(RuntimeOffset1));
            RuntimeOffset2 = s.Serialize<ushort>(RuntimeOffset2, name: nameof(RuntimeOffset2));

            RuntimeBytes1 = s.SerializeArray(RuntimeBytes1, 8, name: nameof(RuntimeBytes1));
            RuntimeCurrentAnimFrame = s.Serialize<byte>(RuntimeCurrentAnimFrame, name: nameof(RuntimeCurrentAnimFrame));
            RuntimeBytes2 = s.SerializeArray(RuntimeBytes2, 4, name: nameof(RuntimeBytes2));

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

            if (UnkPointer2 != null)
            {
                s.DoAt(UnkPointer2, () =>
                {
                    UnkPointer2Values = s.SerializeArray<byte>(UnkPointer2Values, 16, name: nameof(UnkPointer2Values));
                });
            }

            if (UnkPointer3 != null)
            {
                s.DoAt(UnkPointer3, () =>
                {
                    UnkPointer3Values = s.SerializeArray<byte>(UnkPointer3Values, 12, name: nameof(UnkPointer3Values));
                });
            }
        }
    }

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
    34 - Enemy
    36 - Trap cube
    38 - Trampoline
    48 - Scared platform
    49 - Rayman position
    52 - Hp potion
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