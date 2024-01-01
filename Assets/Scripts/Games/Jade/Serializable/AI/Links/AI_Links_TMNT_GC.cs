namespace Ray1Map.Jade {
	public class AI_Links_TMNT_GC : AI_Links_PoP_WW_iOS_HD {

		protected override void InitTypes() {
			#region Types
			Types = new AI_Link[] {
				new AI_Link(32, 4, "AI_EvalType_GetBool", AI_VarType.Bool),
				new AI_Link(33, 4, "AI_EvalType_GetInt", AI_VarType.Int),
				new AI_Link(34, 4, "AI_EvalType_GetInt", AI_VarType.Float),
				new AI_Link(37, 0xC, "AI_EvalType_GetVector", AI_VarType.Vector),
				new AI_Link(38, 4, "AI_EvalType_GetString", AI_VarType.String),
				new AI_Link(39, 4, "AI_EvalType_GetPointerRef", AI_VarType.Function),
				new AI_Link(40, 4, "AI_EvalType_GetGAO", AI_VarType.GAO),
				new AI_Link(41, 0x34, "AI_EvalType_GetMessage", AI_VarType.Message),
				new AI_Link(42, 4, "AI_EvalType_GetPointerRef", AI_VarType.Model),
				new AI_Link(43, 4, "AI_EvalType_GetPointerRef", AI_VarType.Network),
				new AI_Link(44, 8, "AI_EvalType_GetText", AI_VarType.Text),
				new AI_Link(45, 4, "AI_EvalType_GetKey", AI_VarType.Key),
				new AI_Link(46, 4, "AI_EvalType_GetColor", AI_VarType.Color),
				new AI_Link(47, 4, "AI_EvalType_GetSound", AI_VarType.Sound),
				new AI_Link(48, 4, "AI_EvalType_GetColor", AI_VarType.Byref),
				new AI_Link(49, 4, "AI_EvalType_GetColor", AI_VarType.Byrefarr),
				new AI_Link(50, 8, "AI_EvalType_GetMessageId", AI_VarType.MessageId),
				new AI_Link(51, 8, "AI_EvalType_GetLong64", AI_VarType.Long64),

				new AI_Link(53, 0x7C, "AI_EvalType_GetTrigger", AI_VarType.Trigger),
				
				new AI_Link(54, 4, "AI_EvalType_GetInt", AI_VarType.Int), // TODO: what are these?
				new AI_Link(55, 0xC, "AI_EvalType_GetVector", AI_VarType.Vector),
				new AI_Link(56, 4, "AI_EvalType_GetInt", AI_VarType.Int),

				new AI_Link(124, 0xC, "AI_EvalType_GetVector", AI_VarType.Void),
				new AI_Link(125, 0xC, "AI_EvalType_GetVector", AI_VarType.Every),
				new AI_Link(126, 0xC, "AI_EvalType_GetVector", AI_VarType.Hexa),
				new AI_Link(127, 0xC, "AI_EvalType_GetVector", AI_VarType.Binary),
				new AI_Link(128, 0xC, "AI_EvalType_GetVector", AI_VarType.Private),
				new AI_Link(129, 0xC, "AI_EvalType_GetVector", AI_VarType.Separator),
				new AI_Link(130, 0xC, "AI_EvalType_GetVector", AI_VarType.Enum),
				new AI_Link(131, 0xC, "AI_EvalType_GetVector", AI_VarType.Save),
				new AI_Link(132, 4, "AI_EvalType_GetVector", AI_VarType.Reinit),
				new AI_Link(133, 0xC, "AI_EvalType_GetVector", AI_VarType.Saveal),
				new AI_Link(134, 0xC, "AI_EvalType_GetVector", AI_VarType.Optim),
				new AI_Link(135, 0xC, "AI_EvalType_GetVector", AI_VarType.Unknown),
			};
			#endregion
		}
	}
}
