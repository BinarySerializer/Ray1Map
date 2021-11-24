using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_3DCollision : BinarySerializable
    {
        public byte Height { get; set; }
        public byte Type { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            s.DoBits<byte>(b => 
            {
                Height = b.SerializeBits<byte>(Height, 5, name: nameof(Height));
                Type = b.SerializeBits<byte>(Type, 3, name: nameof(Type));
            });
        }

        public Unity_IsometricCollisionTile ToIsometricCollisionTile()
        {
            Unity_IsometricCollisionTile.CollisionType getCollisionType()
            {
                return Type switch
                {
                    0 => Unity_IsometricCollisionTile.CollisionType.Solid,
                    1 => Unity_IsometricCollisionTile.CollisionType.Type_1,
                    2 => Unity_IsometricCollisionTile.CollisionType.Type_2,
                    3 => Unity_IsometricCollisionTile.CollisionType.Type_3,
                    4 => Unity_IsometricCollisionTile.CollisionType.Type_4,
                    5 => Unity_IsometricCollisionTile.CollisionType.Type_5,
                    6 => Unity_IsometricCollisionTile.CollisionType.Type_6,
                    7 => Unity_IsometricCollisionTile.CollisionType.Type_7,
                    _ => Unity_IsometricCollisionTile.CollisionType.Unknown
                };
            }

            return new Unity_IsometricCollisionTile()
            {
                Type = Height > 10 ? Unity_IsometricCollisionTile.CollisionType.Wall : getCollisionType(),
                Height = Height > 10 ? 0 : Height,
            };
        }
    }
}