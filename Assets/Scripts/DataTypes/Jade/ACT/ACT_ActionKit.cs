using BinarySerializer;

namespace R1Engine.Jade 
{
	public class ACT_ActionKit : Jade_File 
    {
		public override string Export_Extension => "ack";
		public ActionRef[] Actions { get; set; }

        protected override void SerializeFile(SerializerObject s) 
        {
            Actions = s.SerializeObjectArray(Actions, FileSize / (Loader.IsBinaryData ? 4 : 8), name: nameof(Actions));
        }

        public class ActionRef : BinarySerializable {
            public Jade_Reference<ACT_Action> Action { get; set; }
            public Jade_FileType Type { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
			    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                Action = s.SerializeObject<Jade_Reference<ACT_Action>>(Action, name: nameof(Action));
                if (!Loader.IsBinaryData) {
                    Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
                } else {
                    if (!Action.IsNull) {
                        Type = new Jade_FileType() { Extension = ".act" };
                    }
                }

                if (Action.Key.Key != 1)
                    Action?.Resolve();
            }
        }

		protected override void OnChangedIsBinaryData() {
			base.OnChangedIsBinaryData();
            if (CurrentIsBinaryData == false) {
                FileSize *= 2;
            } else FileSize /= 2;
        }
	}
}