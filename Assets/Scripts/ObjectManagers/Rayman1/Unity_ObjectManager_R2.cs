
using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Ray1;
using UnityEngine;
using Sprite = BinarySerializer.Ray1.Sprite;

namespace R1Engine
{
    public class Unity_ObjectManager_R2 : Unity_ObjectManager
    {
        public Unity_ObjectManager_R2(Context context, ushort[] linkTable, AnimGroup[] animGroups, UnityEngine.Sprite[] sprites, Sprite[] imageDescriptors, R2_LevDataFile levData) : base(context)
        {
            LinkTable = linkTable;
            AnimGroups = animGroups;
            Sprites = sprites;
            ImageDescriptors = imageDescriptors;
            LevData = levData;
            for (int i = 0; i < AnimGroups.Length; i++) {
                AnimGroupsLookup[AnimGroups[i].Pointer?.AbsoluteOffset ?? 0] = i;
            }
        }

        public AnimGroup[] AnimGroups { get; }
        public Dictionary<long, int> AnimGroupsLookup { get; } = new Dictionary<long, int>();
        public UnityEngine.Sprite[] Sprites { get; }
        public Sprite[] ImageDescriptors { get; }
        public R2_LevDataFile LevData { get; }

        public ushort[] LinkTable { get; }

        public override int InitLinkGroups(IList<Unity_Object> objects) => InitR1LinkGroups(objects, LinkTable);

        public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.Cast<Unity_Object_R2>().FindItem(x => x.EventData.ObjType == R2_ObjType.RaymanPosition);

        public override string[] LegacyDESNames => AnimGroups.Select(x => x.Pointer?.ToString() ?? "N/A").ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        // Global data (for memory loading)
        public R1MemoryData GameMemoryData { get; } = new R1MemoryData();

        protected override void InitMemoryLoading(SerializerObject s, Pointer offset)
        {
            s.Goto(offset);
            GameMemoryData.Update(s);
        }

        protected override bool DoMemoryLoading(Context gameMemoryContext, Pointer offset)
        {
            var lvl = LevelEditorData.Level;
            bool madeEdits = false;
            Pointer currentOffset;
            SerializerObject s;

            void SerializeEvent(Unity_Object_R2 ed, Context context)
            {
                s = ed.HasPendingEdits ? (SerializerObject)context.Serializer : context.Deserializer;
                s.Goto(currentOffset);
                ed.EventData.Init(s.CurrentPointer);
                ed.EventData.Pre_IsSerializingFromMemory = true;

                try
                {
                    ed.EventData.Serialize(s);
                }
                finally
                {
                    ed.EventData.Pre_IsSerializingFromMemory = false;
                }

                if (s is BinarySerializer.BinarySerializer)
                {
                    Debug.Log($"Edited event {ed.EventData.Index}");
                    madeEdits = true;
                }

                ed.HasPendingEdits = false;
                currentOffset = s.CurrentPointer;
            }

            // Events
            if (GameMemoryData.EventArrayOffset != null)
            {
                currentOffset = GameMemoryData.EventArrayOffset;
                foreach (var ed in lvl.EventData.OfType<Unity_Object_R2>().Take(LevData.LoadedObjectsCount))
                    SerializeEvent(ed, gameMemoryContext);
            }

            // Rayman
            if (GameMemoryData.RayEventOffset != null && lvl.Rayman is Unity_Object_R2 r1Ray)
            {
                currentOffset = GameMemoryData.RayEventOffset;
                SerializeEvent(r1Ray, gameMemoryContext);
            }

            // Tiles
            if (GameMemoryData.TileArrayOffset != null)
            {
                currentOffset = GameMemoryData.TileArrayOffset;
                var layerIndex = lvl.DefaultLayer;
                var layer = lvl.Layers[layerIndex] as Unity_Layer_Map;
                var map = layer.Map;

                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        var tileIndex = y * map.Width + x;
                        var mapTile = map.MapTiles[tileIndex];

                        s = mapTile.HasPendingEdits ? (SerializerObject)gameMemoryContext.Serializer : gameMemoryContext.Deserializer;

                        s.Goto(currentOffset);

                        var prevX = mapTile.Data.TileMapX;
                        var prevY = mapTile.Data.TileMapY;

                        mapTile.Data.Init(s.CurrentPointer);
                        mapTile.Data.Serialize(s);

                        if (s is BinarySerializer.BinarySerializer)
                            madeEdits = true;

                        mapTile.HasPendingEdits = false;

                        if (prevX != mapTile.Data.TileMapX || prevY != mapTile.Data.TileMapY)
                            Controller.obj.levelController.controllerTilemap.SetTileAtPos(layerIndex, x, y, mapTile);

                        currentOffset = s.CurrentPointer;
                    }
                }
            }

            return madeEdits;
        }

        public class AnimGroup
        {
            public AnimGroup(Pointer pointer, ObjState[][] eta, Unity_ObjAnimation[] animations, string filePath)
            {
                Pointer = pointer;
                ETA = eta;
                Animations = animations ?? new Unity_ObjAnimation[0];
                FilePath = filePath;
            }

            public Pointer Pointer { get; }

            public ObjState[][] ETA { get; }

            public Unity_ObjAnimation[] Animations { get; }

            public string FilePath { get; }
        }
    }
}