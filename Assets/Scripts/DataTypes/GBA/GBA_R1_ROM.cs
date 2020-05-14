namespace R1Engine
{
    /// <summary>
    /// ROM data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_ROM : GBA_ROM
    {
        /// <summary>
        /// The data for the levels
        /// </summary>
        public GBA_R1_Level[] Levels { get; set; }


        /// <summary>
        /// The background vignette data
        /// </summary>
        public GBA_R1_BackgroundVignette[] BackgroundVignettes { get; set; }

        public GBA_R1_IntroVignette[] IntroVignettes { get; set; }

        public GBA_R1_WorldMapVignette WorldMapVignette { get; set; }


        /// <summary>
        /// The sprite palettes. The game uses the same 16 palettes (with 16 colors) for every sprite in the game. During runtime this gets copied to 0x05000200.
        /// </summary>
        public ARGB1555Color[] SpritePalettes { get; set; }

        public Pointer[] UnkLevelPointerArray1 { get; set; }
        public Pointer[] UnkLevelPointerArray2 { get; set; }
        public Pointer[] UnkLevelPointerArray3 { get; set; }
        public uint[] UnkLevelDwordArray { get; set; }

      
        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = GBA_R1_PointerTable.GetPointerTable(s.GameSettings.GameModeSelection);

            const int levelCount = 22 + 18 + 13 + 13 + 12 + 4 + 6;

            // Serialize data from the ROM
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.Levels], this.Offset.file), 
                () => Levels = s.SerializeObjectArray<GBA_R1_Level>(Levels, levelCount, name: nameof(Levels)));

            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.BackgroundVignette], this.Offset.file), 
                () => BackgroundVignettes = s.SerializeObjectArray<GBA_R1_BackgroundVignette>(BackgroundVignettes, 48, name: nameof(BackgroundVignettes)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.IntroVignette], this.Offset.file), 
                () => IntroVignettes = s.SerializeObjectArray<GBA_R1_IntroVignette>(IntroVignettes, 14, name: nameof(IntroVignettes)));
            WorldMapVignette = s.SerializeObject<GBA_R1_WorldMapVignette>(WorldMapVignette, name: nameof(WorldMapVignette));

            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.SpritePalettes], this.Offset.file), 
                () => SpritePalettes = s.SerializeObjectArray<ARGB1555Color>(SpritePalettes, 16 * 16 * 2, name: nameof(SpritePalettes)));

            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelPointerArray1], this.Offset.file), () => UnkLevelPointerArray1 = s.SerializePointerArray(UnkLevelPointerArray1, levelCount, name: nameof(UnkLevelPointerArray1)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelPointerArray2], this.Offset.file), () => UnkLevelPointerArray2 = s.SerializePointerArray(UnkLevelPointerArray2, levelCount, name: nameof(UnkLevelPointerArray2)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelPointerArray3], this.Offset.file), () => UnkLevelPointerArray3 = s.SerializePointerArray(UnkLevelPointerArray3, levelCount, name: nameof(UnkLevelPointerArray3)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelDwordArray], this.Offset.file), () => UnkLevelDwordArray = s.SerializeArray<uint>(UnkLevelDwordArray, levelCount, name: nameof(UnkLevelDwordArray)));

            Test(s);
        }
        
        public void Test(SerializerObject s)
        {
            // For every level...
            for (int i = 0; i < 1; i++)
            {
                // Get values
                var p1 = UnkLevelPointerArray1[i];
                var p2 = UnkLevelPointerArray2[i];
                var p3 = UnkLevelPointerArray3[i];
                var v = UnkLevelDwordArray[i];

                byte[] v7values = null;
                s.DoAt(p3, () => v7values = s.SerializeArray<byte>(v7values, v, name: nameof(v7values)));

                for (int j = 0; j < v; j++)
                {
                    var v7 = v7values[j];

                    var eventStruct1CurrentPointer = p2 + (4 * j);
                    var eventStruct2CurrentPointer = p1 + (4 * j);

                    Pointer pr2 = null;
                    s.DoAt(eventStruct1CurrentPointer, () => pr2 = s.SerializePointer(pr2, name: nameof(pr2)));

                    Pointer pr1 = null;

                    s.DoAt(eventStruct2CurrentPointer, () => pr1 = s.SerializePointer(pr1, name: nameof(pr1)));

                    for (int v5 = 0; v5 < v7; v5++)
                    {
                        for (int eventIndex = 0; eventIndex < 255; eventIndex++)
                        {
                            GBA_R1_EventGraphicsData eventGraphicsData = null;
                            GBA_R1_EventData eventData = null;

                            s.DoAt(pr1, () => eventGraphicsData = s.SerializeObject<GBA_R1_EventGraphicsData>(eventGraphicsData, name: nameof(eventGraphicsData)));

                            s.DoAt(pr2, () => eventData = s.SerializeObject<GBA_R1_EventData>(eventData, name: nameof(eventData)));

                            pr2 += 28;
                        }
                    }
                }
            }
        }
    }

    // From sub_6438
    public class GBA_R1_EventData : R1Serializable
    {
        public Pointer ETAPointer { get; set; }

        public Pointer CommandsPointer { get; set; }

        public ushort XPosition { get; set; }
        public ushort YPosition { get; set; }

        public ushort Layer { get; set; }

        //*(_WORD*) (2 * eventIndex + v8549200) = *(_WORD*) (eventStruct1CurrentPointer + 14); - linkindex???
        public ushort SomeIndex { get; set; }

        public byte Etat { get; set; }
        public byte SubEtat { get; set; }
        public byte OffsetBX { get; set; }
        public byte OffsetBY { get; set; }
        public byte OffsetHY { get; set; }

        // *(_BYTE*)(eventOffsetInMemory + 112) = *(_BYTE*)(eventOffsetInMemory + 112) & 0xFE | *(_BYTE*)(eventStruct1CurrentPointer + 21) & 1;
        public byte SomeFlag { get; set; }

        public byte FollowSprite { get; set; }
        public byte HitPoints { get; set; }

        public EventType Type { get; set; }

        public byte HitSprite { get; set; }

        public byte[] Unk { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));
            CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));
            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));
            Layer = s.Serialize<ushort>(Layer, name: nameof(Layer));
            SomeIndex = s.Serialize<ushort>(SomeIndex, name: nameof(SomeIndex));
            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));
            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));
            SomeFlag = s.Serialize<byte>(SomeFlag, name: nameof(SomeFlag));
            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));
            HitPoints = s.Serialize<byte>(HitPoints, name: nameof(HitPoints));
            Type = s.Serialize<EventType>(Type, name: nameof(Type));
            HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));
            Unk = s.SerializeArray<byte>(Unk, 2, name: nameof(Unk));
        }
    }

    public class GBA_R1_EventGraphicsData : R1Serializable
    {
        // sub_50BA4((_BYTE*)eventPointer, 0, 116u);

        public Pointer ImageBufferPointer { get; set; }

        // Gets set to offset 30 in memory, which gets overwritten by the position
        public uint ImageBufferSize { get; set; }

        public Pointer ImageDescriptorsPointer { get; set; }

        // *(_WORD*)(eventOffsetInMemory + 50) = sub_4EC90(*(_DWORD*)(structOffset + 12), 0xCu);
        public uint ImageDescriptorCount { get; set; }

        public Pointer ETAPointer { get; set; }

        // Usually between 1-3
        public uint Unk { get; set; }

        public Pointer AnimDescriptorsPointer { get; set; }

        public uint AnimDescriptorCount { get; set; }

        // Some event values gets reset here:
        /*
         
            *(_BYTE*)(eventOffsetInMemory + 88) = 0;
            *(_WORD*)(eventOffsetInMemory + 72) = 0;
            *(_WORD*)(eventOffsetInMemory + 56) = -1;
            *(_WORD*)(eventOffsetInMemory + 66) = (unsigned int)&dword_2710;
             
             */

        public override void SerializeImpl(SerializerObject s)
        {
            ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
            ImageBufferSize = s.Serialize<uint>(ImageBufferSize, name: nameof(ImageBufferSize));
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            ImageDescriptorCount = s.Serialize<uint>(ImageDescriptorCount, name: nameof(ImageDescriptorCount));
            ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));
            Unk = s.Serialize<uint>(Unk, name: nameof(Unk));
            AnimDescriptorsPointer = s.SerializePointer(AnimDescriptorsPointer, name: nameof(AnimDescriptorsPointer));
            AnimDescriptorCount = s.Serialize<uint>(AnimDescriptorCount, name: nameof(AnimDescriptorCount));
        }
    }




    /*

    ARRAYS:
     
    Pointer array at 0x086DCE14 - 60 items
    ushort array at 0x08549774
    
    uint array at 0x08549674
    uint array at 0x0854925E
    ushort array at 0x08549200
    
    Loc strings begin at 0x08F1D4C
    
    (these are aligned weirdly)
    uint array at 0x0854925C
    uin array at 0x0854925D
    uint array at 0x0854925F
    

    SPLASH SCREENS:

    16 palettes for Ubi logo are     at 0x086EEDD8
    16 palettes for Eclipse logo are at 0x086EEFD8
    16 palettes for Rayman logo are  at 0x086EF188

    0x086DEC00 has 6 pointers. First 3 to image data and last 3 to the palettes for Ubi, Eclipse and RayLogo - where are the index tables?


    LOADING + CREDITS SCREENS:

    Palettes don't seem to exist in the rom - compressed?


    MEMORY LOCATIONS:

    0x02030394 - current world
    0x0202E5F0 - current level
    0x0202A2DA - some array where first two bytes are used to compare level and world somehow


    ROM LOCATIONS TO PARSE:

    0x081539A4 - index table for level offsets based on world in global level array

     */
}