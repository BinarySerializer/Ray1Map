using BinarySerializer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ray1Map.GBA
{
    public class Unity_TrackManager_GBA : Unity_TrackManager
    {

		protected override float Height => 10f;

		public override bool Loop => IsLoop;
        protected bool IsLoop { get; set; } = false;
		protected override int Resolution => 1;

		public override bool IsAvailable(Context context, Unity_Level level)
        {
            // Make sure the game is Rayman 3 and Ssssam is in the level
            return context.GetR1Settings().Game == Game.GBA_Rayman3 && level.EventData.OfType<Unity_Object_GBA>().Any(x => x.Actor.ActorID == (byte)GBA_R3_ActorID.Ssssam);
        }

        public GBA_TileCollisionType? GetCollisionType(Unity_Level level, Vector2Int pos)
        {
            // Get the collision map
            var collisionLayer = level.Layers.FirstOrDefault(x => (x as Unity_Layer_Map)?.Map.Type.HasFlag(Unity_Map.MapType.Collision) ?? false);
            var collisionMap = (collisionLayer as Unity_Layer_Map)?.Map;
            if(collisionMap == null) return null;

            // Get the collision type from the current position
            return (GBA_TileCollisionType?)collisionMap.MapTiles.ElementAtOrDefault(pos.y * collisionMap.Width + pos.x)?.Data.CollisionType;
        }

        protected Vector2Int? GetCollisionDirection(GBA_TileCollisionType colType) {
            switch (colType) {
                case GBA_TileCollisionType.Direction_Left: return new Vector2Int(-1, 0);
                case GBA_TileCollisionType.Direction_Right: return new Vector2Int(1, 0);
                case GBA_TileCollisionType.Direction_Up: return new Vector2Int(0, -1);
                case GBA_TileCollisionType.Direction_Down: return new Vector2Int(0, 1);
                case GBA_TileCollisionType.Direction_DownLeft: return new Vector2Int(-1, 1);
                case GBA_TileCollisionType.Direction_DownRight: return new Vector2Int(1, 1);
                case GBA_TileCollisionType.Direction_UpRight: return new Vector2Int(1, -1);
                case GBA_TileCollisionType.Direction_UpLeft: return new Vector2Int(-1, -1);
                default: return null;
            }
        }
        protected override Vector3[] GetPoints(Unity_Level level) {
            var collisionLayer = level.Layers.FirstOrDefault(x => (x as Unity_Layer_Map)?.Map.Type.HasFlag(Unity_Map.MapType.Collision) ?? false);
            var collisionMap = (collisionLayer as Unity_Layer_Map)?.Map;
            if(collisionMap == null) return null;

            var obj = level.EventData.OfType<Unity_Object_GBA>().First(x => x.Actor.ActorID == (byte)GBA_R3_ActorID.Ssssam);
            Vector2Int pos = new Vector2Int(obj.XPosition / 8, obj.YPosition / 8);
            var startPos = pos;
            var lastgoodPos = pos;
            var previousCollisionType = GBA_TileCollisionType.Empty;
            List<Vector2Int> points = new List<Vector2Int>();
            int stepsSinceLastAdd = 0;
            while (true) {
                if (pos.x < 0 || pos.y < 0 || pos.x > collisionMap.Width || pos.y >= collisionMap.Height) break;
                var col = GetCollisionType(level, pos);
                if (col == null || points.Contains(pos)) break;
                if(col.Value == GBA_TileCollisionType.EnemyTrigger_Down) break;
                var dir = GetCollisionDirection(col.Value);
                if (dir == null) {
                    dir = GetCollisionDirection(previousCollisionType);
                } else {
                    /*if (col.Value != previousCollisionType || stepsSinceLastAdd > 5) {
                        points.Add(pos);
                        stepsSinceLastAdd = 0;
                    }*/
                    previousCollisionType = col.Value;
                }
                if (!dir.HasValue || dir.Value == Vector2Int.zero) break;
                if (stepsSinceLastAdd >= 8) {
                    points.Add(pos);
                    stepsSinceLastAdd = 0;
                }
                lastgoodPos = pos;
                pos += dir.Value;
                stepsSinceLastAdd++;
            }
            if (stepsSinceLastAdd < 8) {
                points.RemoveAt(points.Count-1);
            }
            points.Add(lastgoodPos);
            if (pos == startPos) IsLoop = true;

            return points.Select(p => new Vector3(p.x * 8 + 4, p.y * 8 + 4, 0f)).ToArray();
        }
    }
}