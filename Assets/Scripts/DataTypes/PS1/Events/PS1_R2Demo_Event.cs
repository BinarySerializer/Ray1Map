namespace R1Engine
{
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

        // Always 0 in file - gets set to a pointer during runtime
        public uint RuntimePointer1 { get; set; }
        
        // 28 (0x1C)

        public ushort XPosition { get; set; }
        
        public ushort YPosition { get; set; }

        // 32 (0x20)

        public byte[] Unk1 { get; set; }

        // 56 (0x38)

        // Dev pointer in file - gets set to a pointer during runtime
        public uint RuntimePointer2 { get; set; }

        // Always 0 in file - gets set to a pointer during runtime
        public uint RuntimePointer3 { get; set; }

        // 64 (0x40)

        // Unknown value - probably runtime only?
        public ushort RuntimeUnk1 { get; set; }

        // Runtime only? Is 0 when object is no longer active.
        public ushort RuntimeUnk2 { get; set; }

        // 68 (0x44)

        public byte[] Unk2 { get; set; }

        // 72 (0x48)

        // Some runtime offset for the current sprite animation - setting to 0 places sprite in its origin point during runtime
        public ushort RuntimeOffset1 { get; set; }
        public ushort RuntimeOffset2 { get; set; }

        // 76 (0x4C)

        // Always 0 in file
        public byte[] RuntimeBytes1 { get; set; }

        // 84 (0x54)

        /// <summary>
        /// The current animation frame during runtime
        /// </summary>
        public byte RuntimeCurrentAnimFrame { get; set; }

        // Always 0 in file - probably animation related
        public byte[] RuntimeBytes2 { get; set; }

        // The layer to appear on - only effects visual so not the map's z-index?
        public byte Layer { get; set; }

        // 90 (0x5A)

        public byte[] Unk3 { get; set; }

        // Flags?
        public uint Unk4 { get; set; }

        public byte[] Unk5 { get; set; }

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

            RuntimePointer1 = s.Serialize<uint>(RuntimePointer1, name: nameof(RuntimePointer1));

            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));

            Unk1 = s.SerializeArray(Unk1, 24, name: nameof(Unk1));

            RuntimePointer2 = s.Serialize<uint>(RuntimePointer2, name: nameof(RuntimePointer2));
            RuntimePointer3 = s.Serialize<uint>(RuntimePointer3, name: nameof(RuntimePointer3));

            RuntimeUnk1 = s.Serialize<ushort>(RuntimeUnk1, name: nameof(RuntimeUnk1));
            RuntimeUnk2 = s.Serialize<ushort>(RuntimeUnk2, name: nameof(RuntimeUnk2));

            Unk2 = s.SerializeArray(Unk2, 4, name: nameof(Unk2));

            RuntimeOffset1 = s.Serialize<ushort>(RuntimeOffset1, name: nameof(RuntimeOffset1));
            RuntimeOffset2 = s.Serialize<ushort>(RuntimeOffset2, name: nameof(RuntimeOffset2));

            RuntimeBytes1 = s.SerializeArray(RuntimeBytes1, 8, name: nameof(RuntimeBytes1));
            RuntimeCurrentAnimFrame = s.Serialize<byte>(RuntimeCurrentAnimFrame, name: nameof(RuntimeCurrentAnimFrame));
            RuntimeBytes2 = s.SerializeArray(RuntimeBytes2, 4, name: nameof(RuntimeBytes2));

            Layer = s.Serialize<byte>(Layer, name: nameof(Layer));

            Unk3 = s.SerializeArray(Unk3, 10, name: nameof(Unk3));

            Unk4 = s.Serialize<uint>(Unk4, name: nameof(Unk4));

            Unk5 = s.SerializeArray(Unk5, 4, name: nameof(Unk5));

            /*s.DoAt(UnkPointer3, () => {
                Pointer ptr = s.SerializePointer(null, name: "test");
                s.DoAt(ptr, () => {
                    s.SerializePointer(null, name: "test2"); // pointer to 16 byte long structs
                });
            });*/
        }
    }
}