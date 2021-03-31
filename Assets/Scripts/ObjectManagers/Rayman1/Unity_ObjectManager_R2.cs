
using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_R2 : Unity_ObjectManager
    {
        public Unity_ObjectManager_R2(Context context, ushort[] linkTable, AnimGroup[] animGroups, Sprite[] sprites, R1_ImageDescriptor[] imageDescriptors, R1_R2LevDataFile levData) : base(context)
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
        public Dictionary<uint, int> AnimGroupsLookup { get; } = new Dictionary<uint, int>();
        public Sprite[] Sprites { get; }
        public R1_ImageDescriptor[] ImageDescriptors { get; }
        public R1_R2LevDataFile LevData { get; }

        public ushort[] LinkTable { get; }

        public override int InitLinkGroups(IList<Unity_Object> objects) => InitR1LinkGroups(objects, LinkTable);

        public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.Cast<Unity_Object_R2>().FindItem(x => x.EventData.EventType == R1_R2EventType.RaymanPosition);

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
                ed.EventData.Serialize(s);

                if (s is BinarySerializer.BinarySerializer)
                {
                    Debug.Log($"Edited event {ed.EventData.EventIndex}");
                    madeEdits = true;
                }

                ed.HasPendingEdits = false;
                currentOffset = s.CurrentPointer;
            }

            // Events
            if (GameMemoryData.EventArrayOffset != null)
            {
                currentOffset = GameMemoryData.EventArrayOffset;
                foreach (var ed in lvl.EventData.OfType<Unity_Object_R2>().Take(LevData.LoadedEventCount))
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
            public AnimGroup(Pointer pointer, R1_EventState[][] eta, Unity_ObjAnimation[] animations, string filePath)
            {
                Pointer = pointer;
                ETA = eta;
                Animations = animations ?? new Unity_ObjAnimation[0];
                FilePath = filePath;
            }

            public Pointer Pointer { get; }

            public R1_EventState[][] ETA { get; }

            public Unity_ObjAnimation[] Animations { get; }

            public string FilePath { get; }
        }
    }
}