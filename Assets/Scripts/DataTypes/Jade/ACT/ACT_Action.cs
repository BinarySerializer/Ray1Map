using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class ACT_Action : Jade_File {
        public override bool HasHeaderBFFile => true;

        public byte ActionItemsCount { get; set; }
        public byte ActionItemNumberForLoop { get; set; }
        public ushort Counter { get; set; }
        public ACT_ActionItem[] ActionItems { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            ActionItemsCount = s.Serialize<byte>(ActionItemsCount, name: nameof(ActionItemsCount));
            ActionItemNumberForLoop = s.Serialize<byte>(ActionItemNumberForLoop, name: nameof(ActionItemNumberForLoop));
            if (!Loader.IsBinaryData) Counter = s.Serialize<ushort>(Counter, name: nameof(Counter));

            ActionItems = s.SerializeObjectArray<ACT_ActionItem>(ActionItems, ActionItemsCount, name: nameof(ActionItems));
            foreach (var item in ActionItems) {
                item.SerializeTransitions(s);
            }
        }

        public class ACT_ActionItem : BinarySerializable
        {
            public Jade_Reference<EVE_ListTracks> TrackList { get; set; }
            public uint TransitionsPointer { get; set; }
            public Jade_Reference<ANI_Shape> Shape { get; set; }
            public byte Repetition { get; set; }
            public byte FramesCountForBlend { get; set; }
            public byte Flag { get; set; }
            public byte CustomBit { get; set; }
            public ushort DesignFlags { get; set; }
            public byte Color { get; set; }
            public byte UseCounter { get; set; }

            public BAS_Array_Transitions Transitions { get; set; }

            // Montreal

            public Jade_Reference<EVE_ListTracks>[] TrackLists { get; set; }
            public float Float_00 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
			    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                    TrackLists = s.SerializeObjectArray<Jade_Reference<EVE_ListTracks>>(TrackLists, 8, name: nameof(TrackLists))?.Resolve();
					Float_00 = s.Serialize<float>(Float_00, name: nameof(Float_00));
				} else {
                    TrackList = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(TrackList, name: nameof(TrackList))?.Resolve();
                }
                TransitionsPointer = s.Serialize<uint>(TransitionsPointer, name: nameof(TransitionsPointer));
                Shape = s.SerializeObject<Jade_Reference<ANI_Shape>>(Shape, name: nameof(Shape))?.Resolve();
                Repetition = s.Serialize<byte>(Repetition, name: nameof(Repetition));
                FramesCountForBlend = s.Serialize<byte>(FramesCountForBlend, name: nameof(FramesCountForBlend));
                Flag = s.Serialize<byte>(Flag, name: nameof(Flag));
                CustomBit = s.Serialize<byte>(CustomBit, name: nameof(CustomBit));
                DesignFlags = s.Serialize<ushort>(DesignFlags, name: nameof(DesignFlags));
                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier) || !Loader.IsBinaryData) {
                    Color = s.Serialize<byte>(Color, name: nameof(Color));
                    if (!Loader.IsBinaryData) UseCounter = s.Serialize<byte>(UseCounter, name: nameof(UseCounter));
                }
            }

            public void SerializeTransitions(SerializerObject s) {
                if (TransitionsPointer != 0) {
                    Transitions = s.SerializeObject<BAS_Array_Transitions>(Transitions, name: nameof(Transitions));
                }
            }
        }
        public class BAS_Array_Transitions : BinarySerializable {
            public uint BAS_PreviousPointer { get; set; }
            public uint Count { get; set; }
            public uint BAS_Size { get; set; }
            public uint Gran { get; set; }

            public BAS_Key[] Keys { get; set; } // Keys for the lookup
            public Transition[] Values { get; set; } // Values for the lookup

            public override void SerializeImpl(SerializerObject s) {
			    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                if(!Loader.IsBinaryData) BAS_PreviousPointer = s.Serialize<uint>(BAS_PreviousPointer, name: nameof(BAS_PreviousPointer));
                Count = s.Serialize<uint>(Count, name: nameof(Count));
                if (!Loader.IsBinaryData) BAS_Size = s.Serialize<uint>(BAS_Size, name: nameof(BAS_Size));
                Gran = s.Serialize<uint>(Gran, name: nameof(Gran));

                Keys = s.SerializeObjectArray<BAS_Key>(Keys, Count, name: nameof(Keys));
                Values = s.SerializeObjectArray<Transition>(Values, Count, name: nameof(Values));
            }

			public class BAS_Key : BinarySerializable {
                public uint Key { get; set; }
                public uint Value { get; set; }
				public override void SerializeImpl(SerializerObject s) {
                    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                    Key = s.Serialize<uint>(Key, name: nameof(Key));
                    if (!Loader.IsBinaryData) Value = s.Serialize<uint>(Value, name: nameof(Value));
                }
            }

            public class Transition : BinarySerializable {
                public ushort Action { get; set; }
                public byte Flag { get; set; }
                public byte Blend { get; set; }
                public override void SerializeImpl(SerializerObject s) {
                    Action = s.Serialize<ushort>(Action, name: nameof(Action));
                    Flag = s.Serialize<byte>(Flag, name: nameof(Flag));
                    Blend = s.Serialize<byte>(Blend, name: nameof(Blend));
                }
            }
        }
    }
}