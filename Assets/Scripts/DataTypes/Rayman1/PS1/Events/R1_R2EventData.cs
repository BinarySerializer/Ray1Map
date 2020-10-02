using System;
using R1Engine.Serialize;

namespace R1Engine
{
    // TODO: Clean up this class

    /// <summary>
    /// Event data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R1_R2EventData : R1Serializable 
    {
        #region Static Methods

        /// <summary>
        /// Gets a new event instance for Rayman
        /// </summary>
        public static R1_R2EventData GetRayman(R1_R2EventData rayPos, R1_R2AllfixFooter data) => new R1_R2EventData()
        {
            // Gets loaded at 0x80178DF0 during runtime
            UnkPointer = data.RaymanBehaviorPointer,
            CollisionDataPointer = data.RaymanCollisionDataPointer,
            AnimGroupPointer = data.RaymanAnimGroupPointer,
            XPosition = (short)(rayPos != null ? (rayPos.XPosition + rayPos.CollisionData.OffsetBX - data.RaymanCollisionData.OffsetBX) : 100),
            YPosition = (short)(rayPos != null ? (rayPos.YPosition + rayPos.CollisionData.OffsetBY - data.RaymanCollisionData.OffsetBY) : 0),
            Etat = 0,
            SubEtat = 19,
            MapLayer = ObjMapLayer.Front,
            Unk2 = new byte[17],
            EventType = R1_R2EventType.Rayman,
            RuntimeBytes1 = new byte[7],
            DisplayPrio = 7,
            Unk3 = new byte[10],
            Unk4 = new byte[3],
            Unk5 = new byte[2],
            CollisionData = data.RaymanCollisionData,
            AnimGroup = data.RaymanAnimGroup
        };

        #endregion

        #region Event Data

        public ushort UShort_00 { get; set; }
        public ushort UShort_02 { get; set; }
        public ushort UShort_04 { get; set; }
        public ushort UShort_06 { get; set; }
        public ushort UShort_08 { get; set; }
        public ushort UShort_0A { get; set; }
        
        // 12 (0xC)
        
        // Points to structs of varying sizes
        public Pointer UnkPointer { get; set; }

        // Leads to 16-byte long structures for collision data
        public Pointer CollisionDataPointer { get; set; }

        public Pointer AnimGroupPointer { get; set; }

        // Always 0 in file - gets set to a pointer to a function during runtime which gets called whenever the event is initialized
        public uint RuntimeStHandlersPointer { get; set; }
        
        // 28 (0x1C)

        public short InitialXPosition { get; set; }
        public short InitialYPosition { get; set; }

        // 32 (0x20)

        public byte InitialEtat { get; set; }
        public byte InitialSubEtat { get; set; }

        // Appears to closely resemble the hitpoints value from R1
        public byte UnkStateRelatedValue { get; set; }

        // 24 (0x22)

        public byte InitialDisplayPrio { get; set; }

        public ObjMapLayer MapLayer { get; set; }

        // 26 (0x24)

        public byte Unk1 { get; set; }

        public PS1_R2Demo_EventFlags Flags { get; set; }

        public byte[] Unk2 { get; set; }

        // 56 (0x38)

        // Dev pointer in file - gets set to a pointer to the current event state during runtime
        public uint RuntimeCurrentStatePointer { get; set; }

        // Always 0 in file - gets set to a pointer during runtime
        public uint RuntimeCurrentAnimDescriptorPointer { get; set; }

        // 64 (0x40)

        public ushort EventIndex { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        public R1_R2EventType EventType { get; set; }

        // 68 (0x44)

        public short XPosition { get; set; }
        public short YPosition { get; set; }

        // 72 (0x48)

        public short ScreenXPosition { get; set; }
        public short ScreenYPosition { get; set; }

        // 76 (0x4C)

        // 0x4C and 0x4E are ushorts

        // Second byte in here determines horizontal speed and fourth byte the vertical speed
        public byte[] RuntimeBytes1 { get; set; }

        public byte RuntimeCurrentAnimIndex { get; set; }

        // 84 (0x54)

        /// <summary>
        /// The current animation frame during runtime
        /// </summary>
        public byte RuntimeCurrentAnimFrame { get; set; }

        public byte Etat { get; set; }
        public byte SubEtat { get; set; }
        public byte InitialUnkStateRelatedValue { get; set; }
        public byte Unk_58 { get; set; }

        // The layer to appear on (0-7)
        public byte DisplayPrio { get; set; }

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

        // 100 (0x64)

        public ObjMapLayer RuntimeMapLayer { get; set; }

        public byte[] Unk4 { get; set; }

        public PS1_R2Demo_EventRuntimeFlags1 RuntimeFlags1 { get; set; }

        // 104 (0x68)

        public PS1_R2Demo_EventRuntimeFlags2 RuntimeFlags2 { get; set; }
        // First bit determines if the sprite should be faded
        public byte RuntimeFlags3 { get; set; }

        public byte[] Unk5 { get; set; }

        #endregion

        #region Pointer Data

        /// <summary>
        /// The collision data
        /// </summary>
        public R1_R2EventCollision CollisionData { get; set; }

        /// <summary>
        /// The current animation group
        /// </summary>
        public R1_R2EventAnimGroup AnimGroup { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize initial unknown values
            UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
            UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));
            UShort_04 = s.Serialize<ushort>(UShort_04, name: nameof(UShort_04));
            UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
            UShort_08 = s.Serialize<ushort>(UShort_08, name: nameof(UShort_08));
            UShort_0A = s.Serialize<ushort>(UShort_0A, name: nameof(UShort_0A));

            // Serialize pointers
            UnkPointer = s.SerializePointer(UnkPointer, name: nameof(UnkPointer));
            CollisionDataPointer = s.SerializePointer(CollisionDataPointer, name: nameof(CollisionDataPointer));
            AnimGroupPointer = s.SerializePointer(AnimGroupPointer, name: nameof(AnimGroupPointer));
            RuntimeStHandlersPointer = s.Serialize<uint>(RuntimeStHandlersPointer, name: nameof(RuntimeStHandlersPointer));

            // Serialize positions
            InitialXPosition = s.Serialize<short>(InitialXPosition, name: nameof(InitialXPosition));
            InitialYPosition = s.Serialize<short>(InitialYPosition, name: nameof(InitialYPosition));

            // Serialize state data
            InitialEtat = s.Serialize<byte>(InitialEtat, name: nameof(InitialEtat));
            InitialSubEtat = s.Serialize<byte>(InitialSubEtat, name: nameof(InitialSubEtat));
            UnkStateRelatedValue = s.Serialize<byte>(UnkStateRelatedValue, name: nameof(UnkStateRelatedValue));
            InitialDisplayPrio = s.Serialize<byte>(InitialDisplayPrio, name: nameof(InitialDisplayPrio));
            MapLayer = s.Serialize<ObjMapLayer>(MapLayer, name: nameof(MapLayer));

            Unk1 = s.Serialize<byte>(Unk1, name: nameof(Unk1));
            Flags = s.Serialize<PS1_R2Demo_EventFlags>(Flags, name: nameof(Flags));

            Unk2 = s.SerializeArray(Unk2, 17, name: nameof(Unk2));

            RuntimeCurrentStatePointer = s.Serialize<uint>(RuntimeCurrentStatePointer, name: nameof(RuntimeCurrentStatePointer));
            RuntimeCurrentAnimDescriptorPointer = s.Serialize<uint>(RuntimeCurrentAnimDescriptorPointer, name: nameof(RuntimeCurrentAnimDescriptorPointer));

            EventIndex = s.Serialize<ushort>(EventIndex, name: nameof(EventIndex));

            // Serialize the type
            EventType = s.Serialize<R1_R2EventType>(EventType, name: nameof(EventType));

            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));

            ScreenXPosition = s.Serialize<short>(ScreenXPosition, name: nameof(ScreenXPosition));
            ScreenYPosition = s.Serialize<short>(ScreenYPosition, name: nameof(ScreenYPosition));

            RuntimeBytes1 = s.SerializeArray(RuntimeBytes1, 7, name: nameof(RuntimeBytes1));
            RuntimeCurrentAnimIndex = s.Serialize<byte>(RuntimeCurrentAnimIndex, name: nameof(RuntimeCurrentAnimIndex));
            RuntimeCurrentAnimFrame = s.Serialize<byte>(RuntimeCurrentAnimFrame, name: nameof(RuntimeCurrentAnimFrame));

            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            InitialUnkStateRelatedValue = s.Serialize<byte>(InitialUnkStateRelatedValue, name: nameof(InitialUnkStateRelatedValue));
            Unk_58 = s.Serialize<byte>(Unk_58, name: nameof(Unk_58));

            DisplayPrio = s.Serialize<byte>(DisplayPrio, name: nameof(DisplayPrio));

            Unk3 = s.SerializeArray(Unk3, 10, name: nameof(Unk3));

            RuntimeMapLayer = s.Serialize<ObjMapLayer>(RuntimeMapLayer, name: nameof(RuntimeMapLayer));

            Unk4 = s.SerializeArray(Unk4, 2, name: nameof(Unk4));

            RuntimeFlags1 = s.Serialize<PS1_R2Demo_EventRuntimeFlags1>(RuntimeFlags1, name: nameof(RuntimeFlags1));
            RuntimeFlags2 = s.Serialize<PS1_R2Demo_EventRuntimeFlags2>(RuntimeFlags2, name: nameof(RuntimeFlags2));
            RuntimeFlags3 = s.Serialize<byte>(RuntimeFlags3, name: nameof(RuntimeFlags3));

            Unk5 = s.SerializeArray(Unk5, 2, name: nameof(Unk5));

            // Parse data from pointers

            // Serialize collision data
            if (CollisionDataPointer != null)
                s.DoAt(CollisionDataPointer, () => CollisionData = s.SerializeObject<R1_R2EventCollision>(CollisionData, name: nameof(CollisionData)));

            if (!s.FullSerialize || Offset.file is ProcessMemoryStreamFile)
                return;

            // Serialize the animation group data
            if (AnimGroupPointer != null)
                s.DoAt(AnimGroupPointer, () => AnimGroup = s.SerializeObject<R1_R2EventAnimGroup>(AnimGroup, name: nameof(AnimGroup)));
        }

        #endregion

        /// <summary>
        /// Flags for <see cref="R1_R2EventData"/>
        /// </summary>
        [Flags]
        public enum PS1_R2Demo_EventFlags : byte
        {
            None = 0,
            UnkFlag_0 = 1 << 0,
            UnkFlag_1 = 1 << 1,
            UnkFlag_2 = 1 << 2,
            UnkFlag_3 = 1 << 3,
            UnkFlag_4 = 1 << 4,
            UnkFlag_5 = 1 << 5,
            UnkFlag_6 = 1 << 6,
            FlippedHorizontally = 1 << 7,
        }

        public enum ObjMapLayer : byte
        {
            None = 0,
            Front = 1,
            Back = 2
        }

        [Flags]
        public enum PS1_R2Demo_EventRuntimeFlags1 : byte
        {
            None = 0,
            UnkFlag_0 = 1 << 0,
            UnkFlag_1 = 1 << 1,
            UnkFlag_2 = 1 << 2,
            UnkFlag_3 = 1 << 3,

            /// <summary>
            /// Indicates if the event should be drawn on screen
            /// </summary>
            SwitchedOn = 1 << 4,

            UnkFlag_5 = 1 << 5,
            UnkFlag_6 = 1 << 6,
            UnkFlag_7 = 1 << 7,
        }

        [Flags]
        public enum PS1_R2Demo_EventRuntimeFlags2 : byte
        {
            None = 0,

            /// <summary>
            /// Indicates if the event should be flipped
            /// </summary>
            DetectZone = 1 << 0,

            UnkFlag_1 = 1 << 1,

            // For collision
            UnkFlag_2 = 1 << 2,
            UnkFlag_3 = 1 << 3,
            UnkFlag_4 = 1 << 4,
            UnkFlag_5 = 1 << 5,
            UnkFlag_6 = 1 << 6,
            UnkFlag_7 = 1 << 7,
        }
    }
}