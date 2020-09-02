using System;
using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    public class Unity_ObjectManager_R1 : Unity_ObjectManager
    {
        public Unity_ObjectManager_R1(Context context, DataContainer<Unity_ObjGraphics>[] des, DataContainer<R1_EventState[][]>[] eta, ushort[] linkTable, bool usesPointers = true) : base(context)
        {
            DES = des;
            ETA = eta;
            LinkTable = linkTable;
            UsesPointers = usesPointers;
        }

        public DataContainer<Unity_ObjGraphics>[] DES { get; }
        public DataContainer<R1_EventState[][]>[] ETA { get; }
        
        public ushort[] LinkTable { get; }

        public bool UsesPointers { get; }

        public bool UsesLocalCommands => Context.Settings.EngineVersion == EngineVersion.R1_PC_Kit || Context.Settings.EngineVersion == EngineVersion.R1_GBA || Context.Settings.EngineVersion == EngineVersion.R1_DSi;

        public IEnumerable<GeneralEventInfoData> GetGeneralEventInfoData()
        {
            // TODO: Where available in world...
            return LevelEditorData.EventInfoData.Where(x => true);
        }

        public override Unity_Object CreateObject(int index)
        {
            // Get the event
            var e = GetGeneralEventInfoData().ElementAt(index);

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
                Layer = 0,
                HitSprite = e.HitSprite,
                Commands = cmds,
                LabelOffsets = labelOffsets
            }, this);

            eventData.EventData.SetFollowEnabled(Context.Settings, e.FollowEnabled);

            // We need to set the hit points after the type
            eventData.EventData.ActualHitPoints = e.HitPoints;

            return eventData;
        }

        public override void InitLinkGroups(IList<Unity_Object> objects)
        {
            int currentId = 1;

            for (int i = 0; i < objects.Count; i++)
            {
                // No link
                if (LinkTable[i] == i)
                {
                    objects[i].EditorLinkGroup = 0;
                }
                else
                {
                    // Ignore already assigned ones
                    if (objects[i].EditorLinkGroup != 0)
                        continue;

                    // Link found, loop through everyone on the link chain
                    int nextEvent = LinkTable[i];
                    objects[i].EditorLinkGroup = currentId;
                    int prevEvent = i;
                    while (nextEvent != i && nextEvent != prevEvent)
                    {
                        prevEvent = nextEvent;
                        objects[nextEvent].EditorLinkGroup = currentId;
                        nextEvent = LinkTable[nextEvent];
                    }
                    currentId++;
                }
            }
        }
        public override void SaveLinkGroups(IList<Unity_Object> objects)
        {
            /*
            List<int> alreadyChained = new List<int>();
            foreach (Unity_ObjBehaviour ee in Controller.obj.levelController.Events)
            {
                // No link
                if (ee.ObjData.EditorLinkGroup == 0)
                {
                    ee.Data.LinkIndex = Controller.obj.levelController.Events.IndexOf(ee);
                }
                else
                {
                    // Skip if already chained
                    if (alreadyChained.Contains(Controller.obj.levelController.Events.IndexOf(ee)))
                        continue;

                    // Find all the events with the same linkId and store their indexes
                    List<int> indexesOfSameId = new List<int>();
                    int cur = ee.ObjData.EditorLinkGroup;
                    foreach (Unity_ObjBehaviour e in Controller.obj.levelController.Events.Where<Unity_ObjBehaviour>(e => e.ObjData.EditorLinkGroup == cur))
                    {
                        indexesOfSameId.Add(Controller.obj.levelController.Events.IndexOf(e));
                        alreadyChained.Add(Controller.obj.levelController.Events.IndexOf(e));
                    }
                    // Loop through and chain them
                    for (int j = 0; j < indexesOfSameId.Count; j++)
                    {
                        int next = j + 1;
                        if (next == indexesOfSameId.Count)
                            next = 0;

                        Controller.obj.levelController.Events[indexesOfSameId[j]].Data.LinkIndex = indexesOfSameId[next];
                    }
                }
            }*/
        }

        public override void InitEvents(Unity_Level level)
        {
            // Hard-code event animations for the different Rayman types
            Unity_ObjGraphics rayDes = null;

            var rayEvent = (Unity_Object_R1)level.Rayman ?? level.EventData.Cast<Unity_Object_R1>().FirstOrDefault(x => x.EventData.Type == R1_EventType.TYPE_RAY_POS);

            if (rayEvent != null)
                rayDes = DES.ElementAtOrDefault(rayEvent.DESIndex)?.Data;

            if (rayDes == null)
                return;

            var miniRay = level.EventData.Cast<Unity_Object_R1>().FirstOrDefault(x => x.EventData.Type == R1_EventType.TYPE_DEMI_RAYMAN);

            if (miniRay != null)
            {
                var miniRayDes = DES.ElementAtOrDefault(miniRay.DESIndex)?.Data;

                if (miniRayDes != null)
                {
                    miniRayDes.Animations = rayDes.Animations.Select(anim =>
                    {
                        var newAnim = new Unity_ObjAnimation
                        {
                            Frames = anim.Frames.Select(x => new Unity_ObjAnimationFrame()
                            {
                                FrameData = new R1_AnimationFrame
                                {
                                    XPosition = (byte)(x.FrameData.XPosition / 2),
                                    YPosition = (byte)(x.FrameData.YPosition / 2),
                                    Width = (byte)(x.FrameData.Width / 2),
                                    Height = (byte)(x.FrameData.Height / 2)
                                },
                                Layers = x.Layers.Select(l => new Unity_ObjAnimationPart()
                                {
                                    ImageIndex = l.ImageIndex,
                                    XPosition = l.XPosition / 2,
                                    YPosition = l.YPosition / 2,
                                    IsFlippedHorizontally = l.IsFlippedHorizontally,
                                    IsFlippedVertically = l.IsFlippedVertically,
                                }).ToArray()
                            }).ToArray()
                        };

                        return newAnim;
                    }).ToList();
                }
            }

            var badRay = level.EventData.Cast<Unity_Object_R1>().FirstOrDefault(x => x.EventData.Type == R1_EventType.TYPE_BLACK_RAY);

            if (badRay != null)
            {
                var badRayDes = DES.ElementAtOrDefault(badRay.DESIndex)?.Data;

                if (badRayDes != null)
                    badRayDes.Animations = rayDes.Animations;
            }
        }

        public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.Cast<Unity_Object_R1>().FindItem(x => x.EventData.Type == R1_EventType.TYPE_RAY_POS || x.EventData.Type == R1_EventType.TYPE_PANCARTE);

        [Obsolete]
        public override string[] LegacyDESNames => DES.Select(x => x.DisplayName).ToArray();
        [Obsolete]
        public override string[] LegacyETANames => ETA.Select(x => x.DisplayName).ToArray();

        public class DataContainer<T>
        {
            public DataContainer(T data, Pointer pointer, string name = null)
            {
                Data = data;
                Pointer = pointer;
                Name = name;
            }
            public DataContainer(T data, int index, string name = null)
            {
                Data = data;
                Index = index;
                Name = name;
            }

            public T Data { get; }
            public Pointer Pointer { get; }
            protected string Name { get; }
            public int Index { get; }
            public string DisplayName => Name ?? (Pointer != null ? Pointer.ToString() : Index.ToString());
        }
    }
}