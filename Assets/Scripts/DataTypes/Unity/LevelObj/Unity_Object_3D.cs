using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_Object_3D : Unity_Object
    {
        // Position
        public abstract Vector3 Position { get; set; }
        public override float Scale => 1f;//1f/2.85f;
    }
}