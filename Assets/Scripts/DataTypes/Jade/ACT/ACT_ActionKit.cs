using BinarySerializer;

namespace R1Engine.Jade 
{
	public class ACT_ActionKit : Jade_File 
    {
        public ActionRef[] Actions { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            Actions = s.SerializeObjectArray(Actions, FileSize / (Loader.IsBinaryData ? 4 : 8), name: nameof(Actions));
        }

        public class ActionRef : BinarySerializable {
            public Jade_Reference<ACT_Action> Action { get; set; }
            public uint Uint_04_Editor { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
			    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                Action = s.SerializeObject<Jade_Reference<ACT_Action>>(Action, name: nameof(Action));
                if (!Loader.IsBinaryData) Uint_04_Editor = s.Serialize<uint>(Uint_04_Editor, name: nameof(Uint_04_Editor));
                
                if (Action.Key.Key != 1)
                    Action?.Resolve();
            }
        }
    }
}