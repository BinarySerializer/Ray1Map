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
        public Unity_ObjectManager_R1(Context context, DataContainer<DESData>[] des, DataContainer<R1_EventState[][]>[] eta, ushort[] linkTable, bool usesPointers = true, R1_ZDCEntry[] typeZDC = null, R1_ZDCData[] zdcData = null, R1_EventFlags[] eventFlags = null, bool hasDefinedDesEtaNames = false, Dictionary<WldObjType, R1_EventData> eventTemplates = null) : base(context)
        {
            // Set properties
            DES = des;
            ETA = eta;
            LinkTable = linkTable;
            UsesPointers = usesPointers;
            TypeZDC = typeZDC;
            ZDCData = zdcData;
            EventFlags = eventFlags;
            HasDefinedDesEtaNames = hasDefinedDesEtaNames;
            AvailableEvents = GetGeneralEventInfoData().ToArray();
            EventTemplates = eventTemplates ?? new Dictionary<WldObjType, R1_EventData>();

            // Set initial random index to a random value
            RandomIndex = (byte)new Random().Next(0, 256);

            // Create lookup tables
            for (int i = 0; i < DES.Length; i++)
                DESLookup[DES[i]?.PrimaryPointer?.AbsoluteOffset ?? 0] = i;

            for (int i = 0; i < ETA.Length; i++)
                ETALookup[ETA[i]?.PrimaryPointer?.AbsoluteOffset ?? 0] = i;

            // Parse random array
            RandomArray = context.Deserializer.SerializeFromBytes<Array<ushort>>(RandomArrayData, "RandomArrayData", onPreSerialize: x => x.Length = 256, name: nameof(RandomArray)).Value;
        }

        public DataContainer<DESData>[] DES { get; }
        public DataContainer<R1_EventState[][]>[] ETA { get; }
        public Dictionary<uint, int> DESLookup { get; } = new Dictionary<uint, int>();
        public Dictionary<uint, int> ETALookup { get; } = new Dictionary<uint, int>();

        public Dictionary<WldObjType, R1_EventData> EventTemplates { get; }

        public ushort[] RandomArray { get; }
        public byte RandomIndex { get; set; }
        public ushort GetNextRandom(int max)
        {
            RandomIndex++;
            return (ushort)(RandomArray[RandomIndex] % max);
        }

        public ushort[] LinkTable { get; set; }

        public bool UsesPointers { get; }

        public R1_ZDCEntry[] TypeZDC { get; }
        public R1_ZDCData[] ZDCData { get; }
        public R1_EventFlags[] EventFlags { get; }
        public bool HasDefinedDesEtaNames { get; }

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

            return LevelEditorData.EventInfoData.Where(x => 
                x.Worlds.Contains(Context.Settings.R1_World) && 
                x.Engines.Contains(engine) && 
                (!HasDefinedDesEtaNames || (DES.Any(d => d.Name == x.DES) && ETA.Any(e => e.Name == x.ETA))));
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

        public override int MaxObjectCount
        {
            get
            {
                switch (Context.Settings.EngineVersion)
                {
                    case EngineVersion.R1_PS1_JPDemoVol3:
                    case EngineVersion.R1_PS1_JPDemoVol6:
                    case EngineVersion.R1_PS1:
                    case EngineVersion.R1_PS1_JP:
                    case EngineVersion.R1_Saturn:
                        return 254; // Event index is a byte, 0xFF is Rayman

                    case EngineVersion.R2_PS1:
                        return 254; // Event index is a short, so might be higher

                    case EngineVersion.R1_PC:
                    case EngineVersion.R1_PocketPC:
                    case EngineVersion.R1_GBA:
                    case EngineVersion.R1_DSi:
                        return 254; // Event index is a short, so might be higher

                    case EngineVersion.R1_PC_Kit:
                    case EngineVersion.R1_PC_Edu:
                    case EngineVersion.R1_PS1_Edu:
                        return 700; // This is the max in KIT - same in EDU?

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public override string[] GetAvailableObjects => HasDefinedDesEtaNames ? AvailableEvents.Select(x => x.Name).ToArray() : new string[0];
        public override Unity_Object CreateObject(int index)
        {
            // Get the event
            var e = AvailableEvents[index];

            // Get the commands and label offsets
            R1_EventCommandCollection cmds = null;
            ushort[] labelOffsets = null;

            // If local (non-compiled) commands are used, attempt to get them from the event info or decompile the compiled ones
            if (UsesLocalCommands)
            {
                cmds = EventCommandCompiler.Decompile(new EventCommandCompiler.CompiledEventCommandData(R1_EventCommandCollection.FromBytes(e.Commands, Context.Settings), e.LabelOffsets), e.Commands);
            }
            else if (e.Commands.Any())
            {
                cmds = R1_EventCommandCollection.FromBytes(e.Commands, Context.Settings);
                labelOffsets = e.LabelOffsets.Any() ? e.LabelOffsets : null;
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

        public override int InitLinkGroups(IList<Unity_Object> objects) => InitR1LinkGroups(objects, LinkTable);
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
            var miniRayDESIndex = miniRay?.DESIndex;

            if (miniRayDESIndex == null && EventTemplates.ContainsKey(WldObjType.RayLittle))
                miniRayDESIndex = UsesPointers ? DESLookup.TryGetItem(EventTemplates[WldObjType.RayLittle].ImageDescriptorsPointer?.AbsoluteOffset ?? 0, -1) : (int)EventTemplates[WldObjType.RayLittle].PC_ImageDescriptorsIndex;

            if (miniRayDESIndex != null)
            {
                var miniRayDes = DES.ElementAtOrDefault(miniRayDESIndex.Value)?.Data.Graphics;

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

            // Set frames for linked events
            for (int i = 0; i < level.EventData.Count; i++)
            {
                // Recreated from allocateOtherPosts
                var baseEvent = (Unity_Object_R1)level.EventData[i];
                var linkedIndex = LinkTable[i];

                if (baseEvent.EventData.Type.UsesRandomFrame() && i != linkedIndex)
                {
                    var index = 0;

                    do
                    {
                        index++;

                        var e = (Unity_Object_R1)level.EventData[linkedIndex];
                        e.ForceFrame = (byte)((baseEvent.ForceFrame + index) % (e.CurrentAnimation?.Frames.Length ?? 1));
                        e.XPosition = (short)(baseEvent.EventData.XPosition + 32 * index * (baseEvent.EventData.HitPoints - 2));
                        e.YPosition = baseEvent.YPosition;

                        linkedIndex = LinkTable[linkedIndex];
                    } while (i != linkedIndex);
                }
            }

            // Set DES and ETA for worldmap
            if (Context.Settings.R1_World == R1_World.Menu)
            {
                var mapObj = EventTemplates[WldObjType.MapObj];
                var rayman = level.Rayman as Unity_Object_R1;

                // Change Rayman to small Rayman
                if (miniRayDESIndex != null && rayman != null)
                {
                    rayman.DESIndex = miniRayDESIndex.Value;
                    rayman.EventData.OffsetBX = (byte)(rayman.EventData.OffsetBX / 2);
                    rayman.EventData.OffsetBY = (byte)(rayman.EventData.OffsetBY / 2);
                }

                // Set Rayman's properties
                if (rayman != null)
                {
                    rayman.EventData.Etat = rayman.EventData.InitialEtat = 0;
                    rayman.EventData.SubEtat = rayman.EventData.InitialSubEtat = 0;

                    if (Context.Settings.EngineVersion == EngineVersion.R1_PC_Kit)
                    {
                        // ?
                        rayman.XPosition = (short)(level.EventData[0].XPosition + 42 - rayman.EventData.OffsetBX);
                        rayman.YPosition = (short)(level.EventData[0].YPosition + 48 - rayman.EventData.OffsetBY);
                    }
                    else if (Context.Settings.EngineVersion == EngineVersion.R1_PC_Edu ||Context.Settings.EngineVersion == EngineVersion.R1_PS1_Edu)
                    {
                        // ?
                        rayman.XPosition = (short)(level.EventData[0].XPosition + 42 + 44 - rayman.EventData.OffsetBX);
                        rayman.YPosition = (short)(level.EventData[0].YPosition + 48 + 24 - rayman.EventData.OffsetBY);
                        rayman.EventData.PC_Flags |= R1_EventData.PC_EventFlags.IsFlipped;
                    }
                    else
                    {
                        rayman.XPosition = (short)(level.EventData[0].XPosition + 70 - rayman.EventData.OffsetBX + 9); // The game does +4 instead of 9 - why?
                        rayman.YPosition = (short)(level.EventData[0].YPosition + 64 - rayman.EventData.OffsetBY + 8); // Is this correct?
                    }
                }

                foreach (var e in level.EventData.Cast<Unity_Object_R1>().Select(x => x.EventData))
                {
                    e.ImageDescriptorsPointer = mapObj.ImageDescriptorsPointer;
                    e.ImageBufferPointer = mapObj.ImageBufferPointer;
                    e.AnimDescriptorsPointer = mapObj.AnimDescriptorsPointer;
                    e.ETAPointer = mapObj.ETAPointer;

                    e.PC_ImageDescriptorsIndex = mapObj.PC_ImageDescriptorsIndex;
                    e.PC_ImageBufferIndex = mapObj.PC_ImageBufferIndex;
                    e.PC_AnimationDescriptorsIndex = mapObj.PC_AnimationDescriptorsIndex;
                    e.PC_ETAIndex = mapObj.PC_ETAIndex;
                }
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

        public override string[] LegacyDESNames => DES.Select(x => x.DisplayName).ToArray();
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

        public byte GetDisplayPrio(R1_EventType type, int hitPoints, byte originalDisplayPrio)
        {
            var typeValue = (ushort)type;

            if (typeValue > 255)
                return originalDisplayPrio;

            switch (typeValue)
            {
                default:
                    return 4;

                case 30:
                case 98:
                case 141:
                case 158:
                case 164:
                case 181:
                case 199:
                case 204:
                case 213:
                case 236:
                case 238:
                case 245:
                    return 0;

                case 123:
                    return (byte)(Context.Settings.World == 1 && Context.Settings.Level == 14 ? 3 : 2);

                case 2:
                case 31:
                case 55:
                case 82:
                case 95:
                case 137:
                case 142:
                case 148:
                case 173:
                    return 6;

                case 4:
                case 42:
                case 88:
                case 252:
                    return 7;

                case 147:
                    return (byte)(hitPoints < 1 ? 2 : 0);

                case 149:
                case 157:
                case 197:
                    return 1;

                case 7:
                case 20:
                case 96:
                case 109:
                case 111:
                case 112:
                case 246:
                case 251:
                    return 5;

                case 11:
                case 19:
                case 21:
                case 41:
                case 45:
                case 48:
                case 57:
                case 75:
                case 78:
                case 79:
                case 83:
                case 90:
                case 91:
                case 92:
                case 93:
                case 94:
                case 102:
                case 110:
                case 121:
                case 135:
                case 143:
                case 146:
                case 150:
                case 161:
                case 168:
                case 170:
                case 220:
                case 221:
                case 224:
                case 234:
                case 248:
                    return 2;

                case 28:
                case 44:
                case 46:
                case 58:
                case 59:
                case 66:
                case 72:
                case 73:
                case 74:
                case 77:
                case 86:
                case 97:
                case 119:
                case 133:
                case 138:
                case 154:
                case 155:
                case 180:
                case 183:
                case 187:
                case 190:
                case 198:
                case 200:
                case 201:
                case 203:
                case 211:
                case 239:
                case 249:
                    return 3;

                case 253:
                    return 4; // Note: This is 7 if there is no pirate ship event in the level
            }
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
                    var defaultName = PrimaryPointer != null ? PrimaryPointer.ToString() : Index.ToString();

                    // If we don't have a name, return the default name
                    if (Name == null)
                        return defaultName;

                    // The name is only official for Rayman Designer, so return that as is then
                    if (LevelEditorData.MainContext.Settings.EngineVersion == EngineVersion.R1_PC_Kit)
                        return Name;

                    // Otherwise show both versions
                    return $"{defaultName} ({Name})";
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

        public override bool IsObjectAlways(int index) {
            // Get the event
            var e = AvailableEvents[index];

            var flag = ((R1_EventType)e.Type).GetAttribute<ObjTypeInfoAttribute>()?.Flag;

            return flag == ObjTypeFlag.Always;
        }

        protected byte[] RandomArrayData { get; } = 
        {
            0xDE, 0x00, 0x25, 0x02, 0xC8, 0x00, 0xCD, 0x02, 0xCC, 0x03, 0x19, 0x01,
            0xC6, 0x01, 0x6F, 0x00, 0xCA, 0x02, 0x41, 0x02, 0x2A, 0x00, 0xA9, 0x00,
            0x43, 0x03, 0xBD, 0x02, 0x0E, 0x03, 0x4F, 0x03, 0xD6, 0x03, 0xE0, 0x00,
            0xB5, 0x01, 0xCF, 0x03, 0x5B, 0x03, 0xB1, 0x03, 0x3E, 0x03, 0xCD, 0x01,
            0x6B, 0x02, 0xA5, 0x02, 0x66, 0x02, 0x32, 0x02, 0xE1, 0x02, 0x74, 0x00,
            0x9F, 0x01, 0x7C, 0x00, 0xAF, 0x02, 0xE6, 0x01, 0xF6, 0x01, 0x41, 0x02,
            0x60, 0x01, 0x79, 0x03, 0x0E, 0x01, 0xB8, 0x00, 0xB1, 0x01, 0xC7, 0x02,
            0xA7, 0x00, 0x27, 0x02, 0x94, 0x02, 0x7E, 0x02, 0x03, 0x00, 0x26, 0x03,
            0x13, 0x01, 0xD8, 0x01, 0x8B, 0x01, 0x81, 0x01, 0x53, 0x02, 0x69, 0x02,
            0x1E, 0x01, 0xAE, 0x00, 0x38, 0x03, 0x2E, 0x01, 0x55, 0x01, 0xA2, 0x01,
            0xF6, 0x00, 0xA7, 0x01, 0x37, 0x00, 0xF9, 0x01, 0xEF, 0x03, 0x01, 0x00,
            0xA3, 0x01, 0x47, 0x00, 0x4B, 0x00, 0x04, 0x01, 0xE6, 0x03, 0x6B, 0x01,
            0x9E, 0x01, 0xC9, 0x00, 0xC9, 0x00, 0xD8, 0x00, 0xFF, 0x00, 0x08, 0x03,
            0x8E, 0x03, 0x9F, 0x03, 0xF1, 0x02, 0xD8, 0x01, 0x20, 0x02, 0x24, 0x00,
            0x85, 0x00, 0xD5, 0x01, 0xEF, 0x01, 0x63, 0x02, 0x03, 0x01, 0x75, 0x00,
            0xCE, 0x02, 0x98, 0x02, 0x8B, 0x03, 0x80, 0x03, 0x2B, 0x00, 0x36, 0x02,
            0x0A, 0x02, 0x7B, 0x02, 0x15, 0x03, 0x03, 0x01, 0xDE, 0x00, 0xF1, 0x00,
            0x57, 0x01, 0x43, 0x02, 0x88, 0x00, 0x74, 0x02, 0x1F, 0x00, 0xCB, 0x03,
            0xD9, 0x00, 0x3B, 0x03, 0x84, 0x02, 0xDC, 0x02, 0xED, 0x02, 0x35, 0x02,
            0x49, 0x02, 0xAC, 0x00, 0x54, 0x01, 0x7A, 0x03, 0x4C, 0x03, 0x75, 0x02,
            0xD0, 0x03, 0xF7, 0x03, 0xED, 0x03, 0xF3, 0x02, 0x0A, 0x02, 0x2E, 0x01,
            0xDF, 0x02, 0x24, 0x01, 0x8F, 0x02, 0xF8, 0x01, 0xB3, 0x00, 0x3C, 0x02,
            0x89, 0x01, 0x19, 0x03, 0x8E, 0x01, 0x9A, 0x02, 0x82, 0x00, 0x94, 0x00,
            0x58, 0x01, 0xAD, 0x03, 0x9E, 0x02, 0x98, 0x02, 0xD6, 0x02, 0x9F, 0x02,
            0xA5, 0x02, 0xE2, 0x02, 0xFD, 0x03, 0xAF, 0x01, 0x3F, 0x02, 0x81, 0x01,
            0xEF, 0x03, 0x0F, 0x00, 0xC4, 0x03, 0xCF, 0x01, 0xF3, 0x00, 0x2F, 0x02,
            0xFA, 0x01, 0x81, 0x02, 0xD4, 0x02, 0x55, 0x02, 0x28, 0x03, 0xBA, 0x01,
            0x03, 0x00, 0x57, 0x03, 0xDD, 0x02, 0xF2, 0x03, 0xD4, 0x01, 0x69, 0x01,
            0xC1, 0x03, 0x93, 0x02, 0x42, 0x02, 0xCB, 0x02, 0xE4, 0x00, 0x3D, 0x01,
            0x97, 0x03, 0x49, 0x00, 0xD4, 0x03, 0x73, 0x02, 0x53, 0x00, 0x63, 0x03,
            0xE8, 0x02, 0xB2, 0x02, 0xB3, 0x03, 0xF9, 0x01, 0x24, 0x01, 0xB9, 0x02,
            0x3D, 0x01, 0x6B, 0x01, 0x05, 0x03, 0xE8, 0x03, 0xAF, 0x03, 0xFA, 0x00,
            0xA4, 0x01, 0xA7, 0x03, 0xAD, 0x01, 0x5A, 0x01, 0x8B, 0x03, 0x95, 0x00,
            0x94, 0x00, 0x4A, 0x01, 0x9B, 0x00, 0x7F, 0x02, 0xCC, 0x03, 0x13, 0x01,
            0x66, 0x00, 0xEB, 0x03, 0xFB, 0x00, 0xDD, 0x00, 0x57, 0x00, 0x1C, 0x02,
            0x82, 0x03, 0x9E, 0x03, 0x0F, 0x01, 0x75, 0x02, 0x93, 0x03, 0x9F, 0x02,
            0x55, 0x00, 0x10, 0x02, 0x4A, 0x03, 0x64, 0x03, 0xF4, 0x02, 0x74, 0x02,
            0x30, 0x02, 0xE5, 0x03, 0xEE, 0x03, 0x41, 0x00, 0x77, 0x01, 0xEB, 0x02,
            0x63, 0x00, 0xB9, 0x02, 0x5A, 0x01, 0x76, 0x00, 0x84, 0x01, 0x02, 0x01,
            0x04, 0x02, 0x14, 0x00, 0xFC, 0x03, 0x01, 0x00, 0x54, 0x00, 0xFD, 0x00,
            0x2B, 0x02, 0xB0, 0x01, 0xE1, 0x00, 0xD6, 0x01, 0x95, 0x00, 0xD1, 0x00,
            0xA9, 0x01, 0x09, 0x00, 0xDB, 0x01, 0xD2, 0x01, 0xBA, 0x00, 0x0A, 0x00,
            0x07, 0x03, 0x1B, 0x07, 0x07, 0x03, 0x1B, 0x03
        };

        public enum WldObjType
        {
            Ray, // Template for Rayman
            RayLittle, // Template for small Rayman
            ClockObj, // The game over clock
            DivObj, // ?
            MapObj, // 24 map objects
        }
    }
}
