using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public class Unity_ObjectManager_R1 : Unity_ObjectManager
    {
        public Unity_ObjectManager_R1(Context context, DataContainer<DESData>[] des, DataContainer<R1_EventState[][]>[] eta, ushort[] linkTable, bool usesPointers = true, R1_ZDCEntry[] typeZDC = null, R1_ZDCData[] zdcData = null, R1_EventFlags[] eventFlags = null) : base(context)
        {
            // Set properties
            DES = des;
            ETA = eta;
            LinkTable = linkTable;
            UsesPointers = usesPointers;
            TypeZDC = typeZDC;
            ZDCData = zdcData;
            EventFlags = eventFlags;
            AvailableEvents = GetGeneralEventInfoData().ToArray();
            TypeZDC = typeZDC;
            ZDCData = zdcData;

            for (int i = 0; i < DES.Length; i++) {
                DESLookup[DES[i]?.PrimaryPointer?.AbsoluteOffset ?? 0] = i;
            }
            for (int i = 0; i < ETA.Length; i++) {
                ETALookup[ETA[i]?.PrimaryPointer?.AbsoluteOffset ?? 0] = i;
            }
        }

        public DataContainer<DESData>[] DES { get; }
        public DataContainer<R1_EventState[][]>[] ETA { get; }
        public Dictionary<uint, int> DESLookup { get; } = new Dictionary<uint, int>();
        public Dictionary<uint, int> ETALookup { get; } = new Dictionary<uint, int>();

        public ushort[] LinkTable { get; set; }

        public bool UsesPointers { get; }

        public R1_ZDCEntry[] TypeZDC { get; }
        public R1_ZDCData[] ZDCData { get; }
        public R1_EventFlags[] EventFlags { get; }

        public GeneralEventInfoData[] AvailableEvents { get; }

        public bool UsesLocalCommands => Context.Settings.GameModeSelection == GameModeSelection.MapperPC || 
                                         Context.Settings.EngineVersion == EngineVersion.R1_GBA || 
                                         Context.Settings.EngineVersion == EngineVersion.R1_DSi ||
                                         Context.Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 ||
                                         Context.Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6;

        protected IEnumerable<GeneralEventInfoData> GetGeneralEventInfoData()
        {
            var engine = GeneralEventInfoData.Engine.R1;

            if (Context.Settings.EngineVersion == EngineVersion.R1_PC_Edu || Context.Settings.EngineVersion == EngineVersion.R1_PS1_Edu)
                engine = GeneralEventInfoData.Engine.EDU;
            else if (Context.Settings.EngineVersion == EngineVersion.R1_PC_Kit)
                engine = GeneralEventInfoData.Engine.KIT;

            return LevelEditorData.EventInfoData.Where(x => x.Worlds.Contains(Context.Settings.R1_World) && x.Engines.Contains(engine) && DES.Any(d => d.Name == x.DES) && ETA.Any(e => e.Name == x.ETA));
        }
        
        public GeneralEventInfoData FindMatchingEventInfo(R1_EventData e)
        {
            byte[] compiledCmds;
            ushort[] labelOffsets;

            if (UsesLocalCommands)
            {
                var compiledData = e.Commands == null ? null : EventCommandCompiler.Compile(e.Commands, e.Commands.ToBytes(Context.Settings));
                compiledCmds = compiledData?.Commands?.ToBytes(Context.Settings) ?? new byte[0];
                labelOffsets = compiledData?.LabelOffsets ?? new ushort[0];
            }
            else
            {
                compiledCmds = e.Commands?.ToBytes(Context.Settings) ?? new byte[0];
                labelOffsets = e.LabelOffsets ?? new ushort[0];
            }

            // Helper method for comparing the commands
            bool compareCommands(GeneralEventInfoData eventInfo) =>
                eventInfo.LabelOffsets.SequenceEqual(labelOffsets) &&
                eventInfo.Commands.SequenceEqual(compiledCmds);

            // Find a matching item
            var match = AvailableEvents.FindItem(x => x.Type == (ushort)e.Type &&
                                                      x.Etat == e.Etat &&
                                                      x.SubEtat == e.SubEtat &&
                                                      x.OffsetBX == e.OffsetBX &&
                                                      x.OffsetBY == e.OffsetBY &&
                                                      x.OffsetHY == e.OffsetHY &&
                                                      x.FollowSprite == e.FollowSprite &&
                                                      x.HitPoints == e.ActualHitPoints &&
                                                      x.HitSprite == e.HitSprite &&
                                                      x.FollowEnabled == e.GetFollowEnabled(Context.Settings) &&
                                                      compareCommands(x));

            // Create dummy item if not found
            if (match == null && AvailableEvents.Any())
                Debug.LogWarning($"Matching event not found for event with type {e.Type}, etat {e.Etat} & subetat {e.SubEtat} in level {Settings.World}-{Settings.Level}");

            // Return the item
            return match;
        }

        public override string[] GetAvailableObjects => AvailableEvents.Select(x => x.Name).ToArray();
        public override Unity_Object CreateObject(int index)
        {
            // Get the event
            var e = AvailableEvents[index];

            // Get the commands and label offsets
            R1_EventCommandCollection cmds;
            ushort[] labelOffsets;

            // If local (non-compiled) commands are used, attempt to get them from the event info or decompile the compiled ones
            if (UsesLocalCommands)
            {
                cmds = EventCommandCompiler.Decompile(new EventCommandCompiler.CompiledEventCommandData(R1_EventCommandCollection.FromBytes(e.Commands, Context.Settings), e.LabelOffsets), e.Commands);

                // Local commands don't use label offsets
                labelOffsets = new ushort[0];
            }
            else
            {
                if (e.Commands.Any())
                {
                    cmds = R1_EventCommandCollection.FromBytes(e.Commands, Context.Settings);
                    labelOffsets = e.LabelOffsets;
                }
                else
                {
                    cmds = new R1_EventCommandCollection()
                    {
                        Commands = new R1_EventCommand[0]
                    };
                    labelOffsets = new ushort[0];
                }
            }

            var eventData = new Unity_Object_R1(new R1_EventData()
            {
                Type = (R1_EventType)e.Type,
                Etat = e.Etat,
                SubEtat = e.SubEtat,
                OffsetBX = e.OffsetBX,
                OffsetBY = e.OffsetBY,
                OffsetHY = e.OffsetHY,
                FollowSprite = e.FollowSprite,
                DisplayPrio = 0,
                HitSprite = e.HitSprite,
                Commands = cmds,
                LabelOffsets = labelOffsets
            }, this, ETAIndex: ETA.FindItemIndex(x => x.Name == e.ETA));

            eventData.EventData.SetFollowEnabled(Context.Settings, e.FollowEnabled);

            // We need to set the hit points after the type
            eventData.EventData.ActualHitPoints = e.HitPoints;

            // Set DES
            eventData.DESIndex = DES.FindItemIndex(x => x.Name == e.DES);

            return eventData;
        }

        public override void InitR1LinkGroups(IList<Unity_Object> objects) => InitR1LinkGroups(objects, LinkTable);
        public override void SaveLinkGroups(IList<Unity_Object> objects) => LinkTable = SaveR1LinkGroups(objects);

        public override void InitObjects(Unity_Level level)
        {
            // Hard-code event animations for the different Rayman types
            Unity_ObjGraphics rayDes = null;

            var rayEvent = (Unity_Object_R1)level.Rayman ?? level.EventData.Cast<Unity_Object_R1>().FirstOrDefault(x => x.EventData.Type == R1_EventType.TYPE_RAY_POS);

            if (rayEvent != null)
                rayDes = DES.ElementAtOrDefault(rayEvent.DESIndex)?.Data.Graphics;

            if (rayDes == null)
                return;

            var miniRay = level.EventData.Cast<Unity_Object_R1>().FirstOrDefault(x => x.EventData.Type == R1_EventType.TYPE_DEMI_RAYMAN);

            if (miniRay != null)
            {
                var miniRayDes = DES.ElementAtOrDefault(miniRay.DESIndex)?.Data.Graphics;

                if (miniRayDes != null)
                {
                    miniRayDes.Animations = rayDes.Animations.Select(anim =>
                    {
                        var newAnim = new Unity_ObjAnimation
                        {
                            Frames = anim.Frames.Select(x => new Unity_ObjAnimationFrame(x.SpriteLayers.Select(l => new Unity_ObjAnimationPart()
                            {
                                ImageIndex = l.ImageIndex,
                                XPosition = l.XPosition / 2,
                                YPosition = l.YPosition / 2,
                                IsFlippedHorizontally = l.IsFlippedHorizontally,
                                IsFlippedVertically = l.IsFlippedVertically,
                            }).ToArray())).ToArray()
                        };

                        return newAnim;
                    }).ToList();
                }
            }

            var badRay = level.EventData.Cast<Unity_Object_R1>().FirstOrDefault(x => x.EventData.Type == R1_EventType.TYPE_BLACK_RAY);

            if (badRay != null)
            {
                var badRayDes = DES.ElementAtOrDefault(badRay.DESIndex)?.Data.Graphics;

                if (badRayDes != null)
                    badRayDes.Animations = rayDes.Animations;
            }
        }

        public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.Cast<Unity_Object_R1>().FindItem(x => x.EventData.Type == R1_EventType.TYPE_RAY_POS || x.EventData.Type == R1_EventType.TYPE_PANCARTE);

        public override void SaveObjects(IList<Unity_Object> objects)
        {
            foreach (var obj in objects.OfType<Unity_Object_R1>())
            {
                obj.EventData.Etat = obj.EventData.InitialEtat;
                obj.EventData.SubEtat = obj.EventData.InitialSubEtat;
                obj.EventData.DisplayPrio = obj.EventData.InitialDisplayPrio;

                // TODO: Set other runtime values like hp etc.?
            }
        }

        [Obsolete]
        public override string[] LegacyDESNames => DES.Select(x => x.DisplayName).ToArray();
        [Obsolete]
        public override string[] LegacyETANames => ETA.Select(x => x.DisplayName).ToArray();

        public string GetEventFlagsDebugInfo()
        {
            if (EventFlags == null)
                return String.Empty;

            var str = new StringBuilder();

            for (int i = 0; i < EventFlags.Length; i++)
            {
                var type = (R1_EventType)i;
                var attrFlag = type.GetAttribute<ObjTypeInfoAttribute>()?.Flag ?? ObjTypeFlag.Normal;

                var line = $"{Convert.ToString((int)EventFlags[i], 2).PadLeft(32, '0')} - {type}{(attrFlag != ObjTypeFlag.Normal ? $" ({attrFlag})" : String.Empty)}";

                str.AppendLine($"{line,-75} - {EventFlags[i]}");
            }

            return str.ToString();
        }

        // Global data (for memory loading)
        public R1MemoryData GameMemoryData { get; } = new R1MemoryData();
        public R1_RuntimeGlobalData GlobalData { get; set; } = new R1_RuntimeGlobalData();
        public bool GlobalPendingEdits { get; set; }
        public HashSet<string> GlobalDataForceWrite { get; } = new HashSet<string>();

        protected override void InitMemoryLoading(SerializerObject s, Pointer offset)
        {
            s.Goto(offset);
            GameMemoryData.Update(s);

            s.Goto(offset);
            GlobalData.SetPointers(s);
        }
        protected override bool DoMemoryLoading(Context gameMemoryContext, Pointer offset)
        {
            var lvl = LevelEditorData.Level;
            bool madeEdits = false;
            Pointer currentOffset;
            SerializerObject s;

            void SerializeEvent(Unity_Object_R1 ed, Context context)
            {
                s = ed.HasPendingEdits ? (SerializerObject)context.Serializer : context.Deserializer;
                s.Goto(currentOffset);
                ed.EventData.Init(s.CurrentPointer);
                ed.EventData.Serialize(s);

                if (s is BinarySerializer)
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
                foreach (var ed in lvl.EventData.OfType<Unity_Object_R1>())
                    SerializeEvent(ed, gameMemoryContext);
            }

            // Rayman
            if (GameMemoryData.RayEventOffset != null && lvl.Rayman is Unity_Object_R1 r1Ray)
            {
                currentOffset = GameMemoryData.RayEventOffset;
                SerializeEvent(r1Ray, gameMemoryContext);
            }

            // Tiles
            if (GameMemoryData.TileArrayOffset != null)
            {
                currentOffset = GameMemoryData.TileArrayOffset;
                var map = lvl.Maps[0];

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

                        if (s is BinarySerializer)
                            madeEdits = true;

                        mapTile.HasPendingEdits = false;

                        if (prevX != mapTile.Data.TileMapX || prevY != mapTile.Data.TileMapY)
                            Controller.obj.levelController.controllerTilemap.SetTileAtPos(x, y, mapTile);

                        currentOffset = s.CurrentPointer;

                        // On PC we need to also update the BigMap pointer table
                        if (GameMemoryData.BigMap != null && s is BinarySerializer)
                        {
                            var pointerOffset = GameMemoryData.BigMap.MapTileTexturesPointersPointer + (4 * tileIndex);
                            var newPointer = GameMemoryData.BigMap.TileTexturesPointer + (lvl.Maps[0].PCTileOffsetTable[mapTile.Data.TileMapY]).SerializedOffset;
                            s.Goto(pointerOffset);

                            s.SerializePointer(newPointer);
                        }
                    }
                }
            }

            // Global values
            GlobalData.Update(new ToggleSerializer(gameMemoryContext, x => GlobalPendingEdits || GlobalDataForceWrite.Contains(x), offset));

            GlobalPendingEdits = false;

            return madeEdits;
        }

        public class DataContainer<T>
        {
            public DataContainer(T data, Pointer primaryPointer, string name = null)
            {
                Data = data;
                PrimaryPointer = primaryPointer;
                Name = name;
            }
            public DataContainer(T data, int index, string name = null)
            {
                Data = data;
                Index = index;
                Name = name;
            }

            public T Data { get; }
            public string Name { get; }
            public int Index { get; }
            public Pointer PrimaryPointer { get; }
            public string DisplayName
            {
                get
                {
                    if (Name != null && 
                        // Only display the official KIT names for web
                        (LevelEditorData.MainContext.Settings.EngineVersion == EngineVersion.R1_PC_Kit || FileSystem.mode == FileSystem.Mode.Normal))
                        return Name;

                    return (PrimaryPointer != null ? PrimaryPointer.ToString() : Index.ToString());
                }
            }
        }

        public class DESData
        {
            public DESData(Unity_ObjGraphics graphics, R1_ImageDescriptor[] imageDescriptors, Pointer imageDescriptorPointer = null, Pointer animationDescriptorPointer = null, Pointer imageBufferPointer = null)
            {
                Graphics = graphics;
                ImageDescriptors = imageDescriptors ?? new R1_ImageDescriptor[0];
                ImageDescriptorPointer = imageDescriptorPointer;
                AnimationDescriptorPointer = animationDescriptorPointer;
                ImageBufferPointer = imageBufferPointer;
            }

            public Unity_ObjGraphics Graphics { get; }
            public R1_ImageDescriptor[] ImageDescriptors { get; }
            public Pointer ImageDescriptorPointer { get; }
            public Pointer AnimationDescriptorPointer { get; }
            public Pointer ImageBufferPointer { get; }
        }

        public class R1_RuntimeGlobalData
        {
            public Dictionary<string, Pointer> Pointers { get; set; } = new Dictionary<string, Pointer>();

            public int MapTime { get; set; }
            public R1_Poing Poing { get; set; }
            public R1_StatusBar StatusBar { get; set; }
            public short ActiveObjCount { get; set; }
            public R1_RayEvtsFlags RayEventFlags { get; set; }
            public short NumLevelChoice { get; set; }
            public short NumWorldChoice { get; set; }
            public R1_RayMode RayMode { get; set; }
            public short RayWindForce { get; set; }
            public short NumLevel { get; set; }
            public short NumWorld { get; set; }
            public short NewWorld { get; set; }
            public short HelicoTime { get; set; }
            public short XMap { get; set; }
            public short YMap { get; set; }
            public bool RayOnPoelle { get; set; }
            public byte RayModeSpeed { get; set; }
            public byte DeadTime { get; set; }
            public byte CurrentPalID { get; set; }
            public short OldNumLevelChoice { get; set; }

            public void SetPointers(SerializerObject s)
            {
                // Get the game memory offset
                Pointer gameMemoryOffset = s.CurrentPointer;

                // Rayman 1 (PC - 1.21)
                if (s.GameSettings.GameModeSelection == GameModeSelection.RaymanPC_1_21)
                {
                    // In IDA with 1.21 the difference from memory is 0xA1000

                    Pointers = new Dictionary<string, Pointer>()
                    {
                        [nameof(MapTime)] = gameMemoryOffset + 0x16E8C0,
                        [nameof(Poing)] = gameMemoryOffset + 0x16F770,
                        [nameof(StatusBar)] = gameMemoryOffset + 0x16FF52,
                        [nameof(ActiveObjCount)] = gameMemoryOffset + 0x170024,
                        [nameof(RayEventFlags)] = gameMemoryOffset + 0x17081A,
                        [nameof(NumLevelChoice)] = gameMemoryOffset + 0x17082E,
                        [nameof(NumWorldChoice)] = gameMemoryOffset + 0x170838,
                        [nameof(RayMode)] = gameMemoryOffset + 0x170868,
                        [nameof(RayWindForce)] = gameMemoryOffset + 0x170870,
                        [nameof(NumLevel)] = gameMemoryOffset + 0x17087C,
                        [nameof(NumWorld)] = gameMemoryOffset + 0x17088C,
                        [nameof(NewWorld)] = gameMemoryOffset + 0x170892,
                        [nameof(HelicoTime)] = gameMemoryOffset + 0x170898,
                        [nameof(XMap)] = gameMemoryOffset + 0x17089E,
                        [nameof(YMap)] = gameMemoryOffset + 0x1708A6,
                        [nameof(RayOnPoelle)] = gameMemoryOffset + 0x170A54,
                        [nameof(RayModeSpeed)] = gameMemoryOffset + 0x170A73,
                        [nameof(DeadTime)] = gameMemoryOffset + 0x170A7E,
                        [nameof(CurrentPalID)] = gameMemoryOffset + 0x170A82,
                        [nameof(OldNumLevelChoice)] = gameMemoryOffset + 0x17F80E,
                    };
                }
                else  if (s.GameSettings.GameModeSelection == GameModeSelection.RaymanPS1US)
                {
                    Pointers = new Dictionary<string, Pointer>()
                    {
                        [nameof(RayMode)] = gameMemoryOffset + 0x801E5420,
                    };
                }
            }

            public void Update(SerializerObject s)
            {
                s.DoAt(Pointers.TryGetItem(nameof(MapTime)), () => MapTime = s.Serialize<int>(MapTime, name: nameof(MapTime)));
                s.DoAt(Pointers.TryGetItem(nameof(Poing)), () => Poing = s.SerializeObject<R1_Poing>(Poing, name: nameof(Poing)));
                s.DoAt(Pointers.TryGetItem(nameof(StatusBar)), () => StatusBar = s.SerializeObject<R1_StatusBar>(StatusBar, name: nameof(StatusBar)));
                s.DoAt(Pointers.TryGetItem(nameof(ActiveObjCount)), () => ActiveObjCount = s.Serialize<short>(ActiveObjCount, name: nameof(ActiveObjCount)));
                s.DoAt(Pointers.TryGetItem(nameof(RayEventFlags)), () => RayEventFlags = s.Serialize<R1_RayEvtsFlags>(RayEventFlags, name: nameof(RayEventFlags)));
                s.DoAt(Pointers.TryGetItem(nameof(NumLevelChoice)), () => NumLevelChoice = s.Serialize<short>(NumLevelChoice, name: nameof(NumLevelChoice)));
                s.DoAt(Pointers.TryGetItem(nameof(NumWorldChoice)), () => NumWorldChoice = s.Serialize<short>(NumWorldChoice, name: nameof(NumWorldChoice)));
                s.DoAt(Pointers.TryGetItem(nameof(RayMode)), () => RayMode = s.Serialize<R1_RayMode>(RayMode, name: nameof(RayMode)));
                s.DoAt(Pointers.TryGetItem(nameof(RayWindForce)), () => RayWindForce = s.Serialize<short>(RayWindForce, name: nameof(RayWindForce)));
                s.DoAt(Pointers.TryGetItem(nameof(NumLevel)), () => NumLevel = s.Serialize<short>(NumLevel, name: nameof(NumLevel)));
                s.DoAt(Pointers.TryGetItem(nameof(NumWorld)), () => NumWorld = s.Serialize<short>(NumWorld, name: nameof(NumWorld)));
                s.DoAt(Pointers.TryGetItem(nameof(NewWorld)), () => NewWorld = s.Serialize<short>(NewWorld, name: nameof(NewWorld)));
                s.DoAt(Pointers.TryGetItem(nameof(HelicoTime)), () => HelicoTime = s.Serialize<short>(HelicoTime, name: nameof(HelicoTime)));
                s.DoAt(Pointers.TryGetItem(nameof(XMap)), () => XMap = s.Serialize<short>(XMap, name: nameof(XMap)));
                s.DoAt(Pointers.TryGetItem(nameof(YMap)), () => YMap = s.Serialize<short>(YMap, name: nameof(YMap)));
                s.DoAt(Pointers.TryGetItem(nameof(RayOnPoelle)), () => RayOnPoelle = s.Serialize<bool>(RayOnPoelle, name: nameof(RayOnPoelle)));
                s.DoAt(Pointers.TryGetItem(nameof(RayModeSpeed)), () => RayModeSpeed = s.Serialize<byte>(RayModeSpeed, name: nameof(RayModeSpeed)));
                s.DoAt(Pointers.TryGetItem(nameof(DeadTime)), () => DeadTime = s.Serialize<byte>(DeadTime, name: nameof(DeadTime)));
                s.DoAt(Pointers.TryGetItem(nameof(CurrentPalID)), () => CurrentPalID = s.Serialize<byte>(CurrentPalID, name: nameof(CurrentPalID)));
                s.DoAt(Pointers.TryGetItem(nameof(OldNumLevelChoice)), () => OldNumLevelChoice = s.Serialize<short>(OldNumLevelChoice, name: nameof(OldNumLevelChoice)));
            }
        }
    }
}
