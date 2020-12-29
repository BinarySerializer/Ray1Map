using System;
using R1Engine.Serialize;

namespace R1Engine
{
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
            ObjParamsPointer = data.RaymanBehaviorPointer,
            CollisionDataPointer = data.RaymanCollisionDataPointer,
            AnimGroupPointer = data.RaymanAnimGroupPointer,
            XPosition = (short)(rayPos != null ? (rayPos.XPosition + rayPos.CollisionData.OffsetBX - data.RaymanCollisionData.OffsetBX) : 100),
            YPosition = (short)(rayPos != null ? (rayPos.YPosition + rayPos.CollisionData.OffsetBY - data.RaymanCollisionData.OffsetBY) : 10),
            Etat = 0, // It's supposed to be Etat 2, SubEtat 2, but the ray pos state has a better looking speed
            SubEtat = 19,
            MapLayer = ObjMapLayer.Front,
            Unk2 = new byte[17],
            EventType = R1_R2EventType.Rayman,
            DisplayPrio = 7,
            Bytes_5B = new byte[10],
            Bytes_65 = new byte[3],
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
        
        public Pointer ObjParamsPointer { get; set; } // The data struct here depends on the object type and acts as additional parameters. Several of these are edited during runtime. The max size is 44 bytes, which all empty always slot events use.

        // Leads to 16-byte long structures for collision data
        public Pointer CollisionDataPointer { get; set; }

        public Pointer AnimGroupPointer { get; set; }

        // Always 0 in file - gets set to the object function struct when initialized, based on the type
        public uint RuntimeHandlersPointer { get; set; }
        
        // 28 (0x1C)

        public short InitialXPosition { get; set; }
        public short InitialYPosition { get; set; }

        // 32 (0x20)

        public byte InitialEtat { get; set; }
        public byte InitialSubEtat { get; set; }
        public byte InitialHitPoints { get; set; }

        // 24 (0x22)

        public byte InitialDisplayPrio { get; set; }

        public ObjMapLayer MapLayer { get; set; }

        // 26 (0x24)

        public byte Byte_25 { get; set; }
        public PS1_R2Demo_EventFlags Flags { get; set; }
        public byte Byte_27 { get; set; }
        
        public int Float_XPos { get; set; }
        public int Float_YPos { get; set; }

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

        public short Float_SpeedX { get; set; }
        public short Float_SpeedY { get; set; }
        public short Short_50 { get; set; }
        public byte Byte_52 { get; set; }

        public byte RuntimeCurrentAnimIndex { get; set; }

        // 84 (0x54)

        public byte RuntimeCurrentAnimFrame { get; set; }

        public byte Etat { get; set; }
        public byte SubEtat { get; set; }
        public byte HitPoints { get; set; }
        public byte Byte_58 { get; set; }

        // The layer to appear on (0-7)
        public byte DisplayPrio { get; set; }

        // 90 (0x5A)

        public R2_TileCollisionType RuntimeCurrentCollisionType { get; set; }

        public byte[] Bytes_5B { get; set; }

        // 100 (0x64)

        public ObjMapLayer RuntimeMapLayer { get; set; }

        public byte[] Bytes_65 { get; set; }

        public PS1_R2Demo_EventRuntimeFlags1 RuntimeFlags1 { get; set; }

        // 104 (0x68)

        public bool RuntimeFlipX { get; set; }
        public bool RuntimeUnkFlag { get; set; }
        public byte ZDCFlags { get; set; }

        // First bit determines if the sprite should be faded
        public byte RuntimeFlags3 { get; set; }

        public byte[] Unk5 { get; set; }

        #endregion

        #region Pointer Data

        public R1_R2EventCollision CollisionData { get; set; }
        public R1_R2EventAnimGroup AnimGroup { get; set; }

        public byte[] ParamsGeneric { get; set; }
        public Params_Gendoor ParamsGendoor { get; set; }
        public Params_Trigger ParamsTrigger { get; set; }

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
            ObjParamsPointer = s.SerializePointer(ObjParamsPointer, name: nameof(ObjParamsPointer));
            CollisionDataPointer = s.SerializePointer(CollisionDataPointer, name: nameof(CollisionDataPointer));
            AnimGroupPointer = s.SerializePointer(AnimGroupPointer, name: nameof(AnimGroupPointer));
            RuntimeHandlersPointer = s.Serialize<uint>(RuntimeHandlersPointer, name: nameof(RuntimeHandlersPointer));

            // Serialize positions
            InitialXPosition = s.Serialize<short>(InitialXPosition, name: nameof(InitialXPosition));
            InitialYPosition = s.Serialize<short>(InitialYPosition, name: nameof(InitialYPosition));

            // Serialize state data
            InitialEtat = s.Serialize<byte>(InitialEtat, name: nameof(InitialEtat));
            InitialSubEtat = s.Serialize<byte>(InitialSubEtat, name: nameof(InitialSubEtat));
            InitialHitPoints = s.Serialize<byte>(InitialHitPoints, name: nameof(InitialHitPoints));
            InitialDisplayPrio = s.Serialize<byte>(InitialDisplayPrio, name: nameof(InitialDisplayPrio));
            MapLayer = s.Serialize<ObjMapLayer>(MapLayer, name: nameof(MapLayer));

            Byte_25 = s.Serialize<byte>(Byte_25, name: nameof(Byte_25));
            Flags = s.Serialize<PS1_R2Demo_EventFlags>(Flags, name: nameof(Flags));
            Byte_27 = s.Serialize<byte>(Byte_27, name: nameof(Byte_27));

            Float_XPos = s.Serialize<int>(Float_XPos, name: nameof(Float_XPos));
            Float_YPos = s.Serialize<int>(Float_YPos, name: nameof(Float_YPos));

            Unk2 = s.SerializeArray(Unk2, 8, name: nameof(Unk2)); // Two floats?

            RuntimeCurrentStatePointer = s.Serialize<uint>(RuntimeCurrentStatePointer, name: nameof(RuntimeCurrentStatePointer));
            RuntimeCurrentAnimDescriptorPointer = s.Serialize<uint>(RuntimeCurrentAnimDescriptorPointer, name: nameof(RuntimeCurrentAnimDescriptorPointer));

            EventIndex = s.Serialize<ushort>(EventIndex, name: nameof(EventIndex));

            // Serialize the type
            EventType = s.Serialize<R1_R2EventType>(EventType, name: nameof(EventType));

            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));

            ScreenXPosition = s.Serialize<short>(ScreenXPosition, name: nameof(ScreenXPosition));
            ScreenYPosition = s.Serialize<short>(ScreenYPosition, name: nameof(ScreenYPosition));

            Float_SpeedX = s.Serialize<short>(Float_SpeedX, name: nameof(Float_SpeedX));
            Float_SpeedY = s.Serialize<short>(Float_SpeedY, name: nameof(Float_SpeedY));
            Short_50 = s.Serialize<short>(Short_50, name: nameof(Short_50));

            Byte_52 = s.Serialize<byte>(Byte_52, name: nameof(Byte_52));

            RuntimeCurrentAnimIndex = s.Serialize<byte>(RuntimeCurrentAnimIndex, name: nameof(RuntimeCurrentAnimIndex));
            RuntimeCurrentAnimFrame = s.Serialize<byte>(RuntimeCurrentAnimFrame, name: nameof(RuntimeCurrentAnimFrame));

            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            HitPoints = s.Serialize<byte>(HitPoints, name: nameof(HitPoints));
            Byte_58 = s.Serialize<byte>(Byte_58, name: nameof(Byte_58));

            DisplayPrio = s.Serialize<byte>(DisplayPrio, name: nameof(DisplayPrio));

            RuntimeCurrentCollisionType = s.Serialize<R2_TileCollisionType>(RuntimeCurrentCollisionType, name: nameof(RuntimeCurrentCollisionType));

            Bytes_5B = s.SerializeArray(Bytes_5B, 9, name: nameof(Bytes_5B));

            RuntimeMapLayer = s.Serialize<ObjMapLayer>(RuntimeMapLayer, name: nameof(RuntimeMapLayer));

            Bytes_65 = s.SerializeArray(Bytes_65, 2, name: nameof(Bytes_65));

            RuntimeFlags1 = s.Serialize<PS1_R2Demo_EventRuntimeFlags1>(RuntimeFlags1, name: nameof(RuntimeFlags1));

            s.SerializeBitValues<byte>(bitFunc =>
            {
                RuntimeFlipX = bitFunc(RuntimeFlipX ? 1 : 0, 1, name: nameof(RuntimeFlipX)) == 1;
                RuntimeUnkFlag = bitFunc(RuntimeUnkFlag ? 1 : 0, 1, name: nameof(RuntimeUnkFlag)) == 1;
                ZDCFlags = (byte)bitFunc(ZDCFlags, 6, name: nameof(ZDCFlags));
            });

            RuntimeFlags3 = s.Serialize<byte>(RuntimeFlags3, name: nameof(RuntimeFlags3));

            Unk5 = s.SerializeArray(Unk5, 2, name: nameof(Unk5));

            // Parse data from pointers

            // Serialize collision data
            if (CollisionDataPointer != null)
                s.DoAt(CollisionDataPointer, () => CollisionData = s.SerializeObject<R1_R2EventCollision>(CollisionData, name: nameof(CollisionData)));

            // Serialize object params
            s.DoAt(ObjParamsPointer, () =>
            {
                if (EventType == R1_R2EventType.Gendoor || EventType == R1_R2EventType.Killdoor)
                    ParamsGendoor = s.SerializeObject<Params_Gendoor>(ParamsGendoor, name: nameof(ParamsGendoor));
                else if (EventType == R1_R2EventType.Trigger)
                    ParamsTrigger = s.SerializeObject<Params_Trigger>(ParamsTrigger, name: nameof(ParamsTrigger));
                else
                    ParamsGeneric = s.SerializeArray<byte>(ParamsGeneric, 44, name: nameof(ParamsGeneric)); // 44 bytes is the max length for object params
            });

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

        #region Object Params

        public class Params_Gendoor : R1Serializable
        {
            public Pointer LinkedObjectsPointer { get; set; }
            public Pointer TriggerObjectsPointer { get; set; } // Objects that can trigger the gendoor when in range. We don't show these in the editor right now since they're unused in the prototype, even though the engine supports them.

            public ushort LinkedObjectsCount { get; set; }
            public ushort TriggerObjectsCount { get;set; }
            public GendoorFlags Flags_0 { get;set; } // Might be something different here?
            public GendoorFlags Flags_1 { get;set; }
            public byte RuntimeHasTriggered { get;set; } // Keeps track if the gendoor has triggered during runtime

            public short[] LinkedObjects { get; set; }
            public short[] TriggerObjects { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                LinkedObjectsPointer = s.SerializePointer(LinkedObjectsPointer, name: nameof(LinkedObjectsPointer));
                TriggerObjectsPointer = s.SerializePointer(TriggerObjectsPointer, name: nameof(TriggerObjectsPointer));
                LinkedObjectsCount = s.Serialize<ushort>(LinkedObjectsCount, name: nameof(LinkedObjectsCount));
                TriggerObjectsCount = s.Serialize<ushort>(TriggerObjectsCount, name: nameof(TriggerObjectsCount));
                Flags_0 = s.Serialize<GendoorFlags>(Flags_0, name: nameof(Flags_0));
                Flags_1 = s.Serialize<GendoorFlags>(Flags_1, name: nameof(Flags_1));
                RuntimeHasTriggered = s.Serialize<byte>(RuntimeHasTriggered, name: nameof(RuntimeHasTriggered));
                s.Serialize<byte>(default, name: "Padding"); // Not referenced by the code

                LinkedObjects = s.DoAt(LinkedObjectsPointer, () => s.SerializeArray<short>(LinkedObjects, LinkedObjectsCount, name: nameof(LinkedObjects)));
                TriggerObjects = s.DoAt(TriggerObjectsPointer, () => s.SerializeArray<short>(TriggerObjects, TriggerObjectsCount, name: nameof(TriggerObjects)));
            }

            [Flags]
            public enum GendoorFlags : byte
            {
                None = 0,

                TriggeredByRayman = 1 << 0,
                TriggeredByPoing = 1 << 1, // Triggered by Rayman's fist
                MultiTriggered = 1 << 2, // Indicates if the gendoor can be triggered multiple times
            }
        }

        public class Params_Trigger : R1Serializable
        {
            public Pointer LinkedObjectsPointer { get; set; }
            public ushort LinkedObjectsCount { get; set; }
            public ushort Flags { get; set; }

            // 4 more bytes? They seem unreferenced.

            public short[] LinkedObjects { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                LinkedObjectsPointer = s.SerializePointer(LinkedObjectsPointer, name: nameof(LinkedObjectsPointer));
                LinkedObjectsCount = s.Serialize<ushort>(LinkedObjectsCount, name: nameof(LinkedObjectsCount));
                Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));

                LinkedObjects = s.DoAt(LinkedObjectsPointer, () => s.SerializeArray<short>(LinkedObjects, LinkedObjectsCount, name: nameof(LinkedObjects)));
            }
        }

        #endregion
    }
}