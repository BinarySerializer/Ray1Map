using System;
using System.Collections.Generic;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// The editor manager for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_EditorManager : PS1_EditorManager
    {
        public PS1_R2Demo_EditorManager(Common_Lev level, Context context, IDictionary<Pointer, Common_Design> des, IDictionary<Pointer, Common_EventState[][]> eta) : base(level, context, des, eta, null)
        { }

        public override Enum GetCollisionTypeAsEnum(byte collisionType) => (R2_TileCollsionType)collisionType;

        public override TileCollisionTypeGraphic GetCollisionTypeGraphic(byte collisionType) => ((R2_TileCollsionType)collisionType).GetCollisionTypeGraphic();
    }
}