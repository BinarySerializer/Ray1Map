using System;

namespace R1Engine
{
    public class GBAIsometric_TileCollision : R1Serializable
    {
        public byte Height { get; set; }
        public CollisionType Type { get; set; }
        public AdditionalTypeFlags AddType { get; set; }
        public byte Layer1 { get; set; }
        public byte Layer2 { get; set; }
        public byte Layer3 { get; set; }
        public ShapeType Shape { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_RHR) {
                Height = s.Serialize<byte>(Height, name: nameof(Height));
                s.SerializeBitValues<byte>(serializeFunc => {
                    Type = (CollisionType)serializeFunc((int)Type, 4, name: nameof(Type));
                    AddType = (AdditionalTypeFlags)serializeFunc((int)AddType, 4, name: nameof(AddType));
                });
                s.SerializeBitValues<ushort>(serializeFunc => {
                    Layer1 = (byte)serializeFunc(Layer1, 4, name: nameof(Layer1));
                    Layer2 = (byte)serializeFunc(Layer2, 4, name: nameof(Layer2));
                    Layer3 = (byte)serializeFunc(Layer3, 4, name: nameof(Layer3));
                    Shape = (ShapeType)serializeFunc((int)Shape, 4, name: nameof(Shape));
                });
            } else {
                Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
            }
        }

        [Flags]
        public enum AdditionalTypeFlags : byte {
            None = 0,
            FenceUpLeft = 1 << 0,
            FenceUpRight = 1 << 1,
            ClimbUpLeft = 1 << 2,
            ClimbUpRight = 1 << 3
        }

        public enum CollisionType : byte {
            Solid = 0,
            Water = 1,
            Lava = 2,
            Wall = 3,
            ObstacleHurt = 4,
            Pit = 5,
            WaterFlowBottomLeft = 6,
            WaterFlowBottomRight = 7,
            ExitTrigger = 8,
            NearExitTrigger = 9, // Found before exit triggers in LotLD & in Menhirs of Power. If the other character is standing on this, game won't say "I shouldn't leave my good friend Globox behind!"
            DialogueTrigger1 = 10,
            DialogueTrigger2 = 11,
            DialogueTrigger3 = 12,
            DialogueTrigger4 = 13,
            Unk14 = 14, // Found in the pits next to the exit sign in Scalding Cascade, in Fairy Council & Clearleaf Forest
            Unused15 = 15
        }

        public enum ShapeType : byte {
            None = 0,
            Normal = 1,
            SlopeUpRight = 2,
            SlopeUpLeft = 3,
            Unk4 = 4,
            Unk5 = 5,
            LevelEdgeTop = 6,
            LevelEdgeBottom = 7,
            LevelEdgeLeft = 8,
            LevelEdgeRight = 9,
            Pit = 10,
            Unk11 = 11,
            Unk12 = 12,
            Unk13 = 13,
            Unk14 = 14,
            Unk15 = 15
        }
    }
}