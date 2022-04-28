using System;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_TileCollision : BinarySerializable
    {
        public byte Height { get; set; }
        public CollisionType_RHR Type { get; set; }
        public AdditionalTypeFlags_RHR AddType { get; set; }
        public byte Layer1 { get; set; }
        public byte Layer2 { get; set; }
        public byte Layer3 { get; set; }
        public ShapeType_RHR Shape { get; set; }
        public byte[] Data { get; set; }


        // Spyro
        public byte HeightFlags { get; set; }
        public byte Depth { get; set; } // Higher value = closer to bottom of level
        public CollisionType_Spyro Type_Spyro { get; set; }
        public AdditionalTypeFlags_Spyro AddType_Spyro { get; set; }
        public ShapeType_Spyro Shape_Spyro { get; set; }
        public byte Unk_Spyro { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_RHR) {
                Height = s.Serialize<byte>(Height, name: nameof(Height));
                s.DoBits<byte>(b => {
                    Type = b.SerializeBits<CollisionType_RHR>(Type, 4, name: nameof(Type));
                    AddType = b.SerializeBits<AdditionalTypeFlags_RHR>(AddType, 4, name: nameof(AddType));
                });
                s.DoBits<ushort>(b => {
                    Layer1 = b.SerializeBits<byte>(Layer1, 4, name: nameof(Layer1));
                    Layer2 = b.SerializeBits<byte>(Layer2, 4, name: nameof(Layer2));
                    Layer3 = b.SerializeBits<byte>(Layer3, 4, name: nameof(Layer3));
                    Shape = b.SerializeBits<ShapeType_RHR>(Shape, 4, name: nameof(Shape));
                });
            } else if(s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro3 || s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2 || s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Tron2) {
                if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2) {
                    s.DoBits<byte>(b => {
                        Height = (byte)b.SerializeBits<int>(Height,6,name: nameof(Height));
                        HeightFlags = (byte)b.SerializeBits<int>(HeightFlags, 2, name: nameof(HeightFlags));
                    });
                } else {
                    Height = s.Serialize<byte>(Height, name: nameof(Height));
                }
                Depth = s.Serialize<byte>(Depth, name: nameof(Depth));
                s.DoBits<byte>(b => {
                    Shape_Spyro = b.SerializeBits<ShapeType_Spyro>(Shape_Spyro, 4, name: nameof(Shape_Spyro));
                    Unk_Spyro = b.SerializeBits<byte>(Unk_Spyro, 4, name: nameof(Unk_Spyro)); // Could be part of shape, or just padding
                });
                s.DoBits<byte>(b => {
                    Type_Spyro = b.SerializeBits<CollisionType_Spyro>(Type_Spyro, 4, name: nameof(Type_Spyro));
                    AddType_Spyro = b.SerializeBits<AdditionalTypeFlags_Spyro>(AddType_Spyro, 4, name: nameof(AddType_Spyro));
                });
            } else {
                Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
            }
        }

        [Flags]
        public enum AdditionalTypeFlags_RHR : byte {
            None = 0,
            FenceUpLeft = 1 << 0,
            FenceUpRight = 1 << 1,
            ClimbUpLeft = 1 << 2,
            ClimbUpRight = 1 << 3
        }

        [Flags]
        public enum AdditionalTypeFlags_Spyro : byte {
            None = 0,
            FenceUpLeft = 1 << 0,
            FenceDownRight = 1 << 1,
            FenceUpRight = 1 << 2,
            FenceDownLeft = 1 << 3
        }

        public enum CollisionType_RHR : byte {
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



        public enum CollisionType_Spyro : byte {
            Solid = 0,
            Water = 1,
            Unk2 = 2,
            FreezableWater = 3,
            Trigger = 4,
            Lava = 5,
            Pit = 6,
            Unk7 = 7,
            HubworldPit = 8,
            Unk9 = 9,
            Wall = 10,
            Unk11 = 11,
            Unk12 = 12,
            Unk13 = 13,
            Unk14 = 14,
            Unk15 = 15
        }

        public enum ShapeType_RHR : byte {
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

        public enum ShapeType_Spyro : byte {
            None = 0,
            Unk1 = 1,
            Normal = 2,
            SlopeUpRight = 3,
            SlopeUpLeft = 4,
            OutOfBounds = 5,
            LevelEdgeTop = 6,
            LevelEdgeBottom = 7,
            LevelEdgeLeft = 8,
            LevelEdgeRight = 9,
            Unk10 = 10,
            Unk11 = 11,
            Unk12 = 12,
            Unk13 = 13,
            Unk14 = 14,
            Unk15 = 15
        }
    }
}