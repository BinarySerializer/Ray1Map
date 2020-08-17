using System;
using System.Collections.Generic;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R1_PS1R2_EditorManager : R1_PS1_EditorManager
    {
        public R1_PS1R2_EditorManager(Unity_Level level, Context context, IDictionary<Pointer, Unity_ObjGraphics> des, IDictionary<Pointer, R1_EventState[][]> eta) : base(level, context, des, eta, null)
        { }

        public override Enum GetCollisionTypeAsEnum(byte collisionType) => (R2_TileCollsionType)collisionType;

        public override Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(byte collisionType) => ((R2_TileCollsionType)collisionType).GetCollisionTypeGraphic();
    }
}