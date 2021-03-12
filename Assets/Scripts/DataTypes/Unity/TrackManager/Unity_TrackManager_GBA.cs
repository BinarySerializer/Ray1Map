using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_TrackManager_GBA : Unity_TrackManager
    {
        public override bool IsAvailable(Context context, Unity_Level level)
        {
            // Make sure the game is Rayman 3 and Ssssam is in the level
            return context.Settings.Game == Game.GBA_Rayman3 && level.EventData.OfType<Unity_Object_GBA>().Any(x => x.Actor.ActorID == (byte)GBA_R3_ActorID.Ssssam);
        }

        public override Vector3 GetStartPosition(Unity_Level level)
        {
            // Reset the previous collision type
            PreviousCollisionType = GBA_TileCollisionType.Empty;

            // Get Ssssam
            var obj = level.EventData.OfType<Unity_Object_GBA>().First(x => x.Actor.ActorID == (byte)GBA_R3_ActorID.Ssssam);

            // Return position
            return new Vector3(obj.XPosition, obj.YPosition, 10);
        }

        protected GBA_TileCollisionType PreviousCollisionType { get; set; }

        public override Vector3 GetDirection(Unity_Level level, Vector3 pos)
        {
            // Get the collision type from the current position
            var col = GetCollisionType(level, pos);

            if (col == null)
                return Vector3.zero;

            var s = getDir(col.Value);

            if (s == null)
                s = getDir(PreviousCollisionType);
            else
                PreviousCollisionType = col.Value;

            return s ?? Vector3.zero;

            Vector3? getDir(GBA_TileCollisionType colType)
            {
                switch (colType)
                {
                    case GBA_TileCollisionType.Direction_Left: return new Vector3(-1, 0);
                    case GBA_TileCollisionType.Direction_Right: return new Vector3(1, 0);
                    case GBA_TileCollisionType.Direction_Up: return new Vector3(0, -1);
                    case GBA_TileCollisionType.Direction_Down: return new Vector3(0, 1);
                    case GBA_TileCollisionType.Direction_DownLeft: return new Vector3(-1, 1);
                    case GBA_TileCollisionType.Direction_DownRight: return new Vector3(1, 1);
                    case GBA_TileCollisionType.Direction_UpRight: return new Vector3(1, -1);
                    case GBA_TileCollisionType.Direction_UpLeft: return new Vector3(-1, -1);
                    default: return null;
                }
            }
        }

        public override bool HasReachedEnd(Unity_Level level, Vector3 pos)
        {
            // Get the collision type from the current position
            var col = GetCollisionType(level, pos);

            return col == null || col == GBA_TileCollisionType.EnemyTrigger_Down;
        }

        public GBA_TileCollisionType? GetCollisionType(Unity_Level level, Vector3 pos)
        {
            // Get the collision map
            var collisionLayer = level.Layers.FirstOrDefault(x => (x as Unity_Layer_Map)?.Map.Type.HasFlag(Unity_Map.MapType.Collision) ?? false);
            var collisionMap = (collisionLayer as Unity_Layer_Map)?.Map;
            if(collisionMap == null) return null;

            // Get the collision type from the current position
            return (GBA_TileCollisionType?)collisionMap.MapTiles.ElementAtOrDefault(Mathf.FloorToInt(pos.y / 8) * collisionMap.Width + Mathf.FloorToInt(pos.x / 8))?.Data.CollisionType;
        }
    }
}