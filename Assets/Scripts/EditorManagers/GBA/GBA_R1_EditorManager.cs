using System.Collections.Generic;
using System.Collections.ObjectModel;
using R1Engine.Serialize;

namespace R1Engine
{
    public class GBA_R1_EditorManager : BaseEditorManager
    {
        public GBA_R1_EditorManager(Common_Lev level, Context context) : base(level, context, new ReadOnlyDictionary<string, Common_Design>(new Dictionary<string, Common_Design>()), new ReadOnlyDictionary<string, Common_EventState[][]>(new Dictionary<string, Common_EventState[][]>()))
        { }

        protected override bool UsesLocalCommands => true;

        public override string GetDesKey(GeneralEventInfoData eventInfoData) => null;

        public override string GetEtaKey(GeneralEventInfoData eventInfoData) => null;

        public override bool IsAvailableInWorld(GeneralEventInfoData eventInfoData) => false;
    }
}