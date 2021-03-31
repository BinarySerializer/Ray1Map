
using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class Unity_ObjectManager_R1Jaguar : Unity_ObjectManager
    {
        public Unity_ObjectManager_R1Jaguar(Context context, List<EventDefinition> eventDefinitions) : base(context)
        {
            EventDefinitions = eventDefinitions;
        }

        public List<EventDefinition> EventDefinitions { get; }

        // TODO: Change this to use Jaguars link system instead
        public override int InitLinkGroups(IList<Unity_Object> objects)
        {
            int currentId = 1;

            for (int i = 0; i < objects.Count; i++)
            {
                // No link
                if (((Unity_Object_R1Jaguar)objects[i]).LinkIndex == i)
                {
                    objects[i].EditorLinkGroup = 0;
                }
                else
                {
                    // Ignore already assigned ones
                    if (objects[i].EditorLinkGroup != 0)
                        continue;

                    // Link found, loop through everyone on the link chain
                    int nextEvent = ((Unity_Object_R1Jaguar)objects[i]).LinkIndex;
                    objects[i].EditorLinkGroup = currentId;
                    int prevEvent = i;
                    while (nextEvent != i && nextEvent != prevEvent)
                    {
                        prevEvent = nextEvent;
                        objects[nextEvent].EditorLinkGroup = currentId;
                        nextEvent = ((Unity_Object_R1Jaguar)objects[nextEvent]).LinkIndex;
                    }
                    currentId++;
                }
            }

            return currentId;
        }

        public override string[] LegacyDESNames => EventDefinitions.Select(x => x.DisplayName).ToArray();

        public class State
        {
            public State(byte animationIndex, byte animSpeed, string name)
            {
                AnimationIndex = animationIndex;
                AnimSpeed = animSpeed;
                Name = name;
            }

            public byte AnimationIndex { get; }
            public byte AnimSpeed { get; }
            public string Name { get; }
        }

        public class EventDefinition
        {
            public EventDefinition(Pointer pointer, Unity_ObjGraphics des, State[][] eta, string name, R1Jaguar_EventDefinition definition)
            {
                Pointer = pointer;
                DES = des;
                ETA = eta;
                Name = name;
                Definition = definition;
            }

            public Pointer Pointer { get; }
            public Unity_ObjGraphics DES { get; }
            public State[][] ETA { get; }
            protected string Name { get; }
            public string DisplayName => Name ?? $"MS_0x{Pointer.FileOffset:X8}";
            public R1Jaguar_EventDefinition Definition { get; }
        }
    }
}