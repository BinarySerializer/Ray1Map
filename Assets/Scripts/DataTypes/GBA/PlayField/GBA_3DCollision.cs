namespace R1Engine
{
    public class GBA_3DCollision : R1Serializable
    {
        public byte Height { get; set; }
        public byte Type { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            s.SerializeBitValues<byte>(serializeFunc => {
                Height = (byte)serializeFunc(Height, 5, name: nameof(Height));
                Type = (byte)serializeFunc(Type, 3, name: nameof(Type));
            });
        }

        public Unity_IsometricCollisionTile ToIsometricCollisionTile()
        {
            Unity_IsometricCollisionTile.CollisionType getCollisionType()
            {
                switch (Type)
                {
                    case 0: return Unity_IsometricCollisionTile.CollisionType.Solid;
                    case 1: return Unity_IsometricCollisionTile.CollisionType.Type_1;
                    case 2: return Unity_IsometricCollisionTile.CollisionType.Type_2;
                    case 3: return Unity_IsometricCollisionTile.CollisionType.Type_3;
                    case 4: return Unity_IsometricCollisionTile.CollisionType.Type_4;
                    case 5: return Unity_IsometricCollisionTile.CollisionType.Type_5;
                    case 6: return Unity_IsometricCollisionTile.CollisionType.Type_6;
                    case 7: return Unity_IsometricCollisionTile.CollisionType.Type_7;
                    default: return Unity_IsometricCollisionTile.CollisionType.Unknown;
                }
            }

            return new Unity_IsometricCollisionTile()
            {
                Type = Height > 10 ? Unity_IsometricCollisionTile.CollisionType.Wall : getCollisionType(),
                Height = Height > 10 ? 0 : Height,
            };
        }
    }
}