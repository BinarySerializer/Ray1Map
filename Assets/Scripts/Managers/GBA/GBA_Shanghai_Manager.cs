using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    public class GBA_Shanghai_Manager : GBA_BatmanVengeance_Manager
    {
        public override Unity_ObjectManager GetObjectManager(Context context, GBA_Scene scene, GBA_Data data) => new Unity_ObjectManager_GBC(context, data.Shanghai_Level.Scene.ActorModels.Select((x, i) => new Unity_ObjectManager_GBC.ActorModel(i, x.Shanghai_ActionTable.ActionTable.Actions, GetCommonDesign(x.Shanghai_ActionTable.Puppet, data))).ToArray());

        public override IEnumerable<Unity_Object> GetObjects(Context context, GBA_Scene scene, Unity_ObjectManager objManager, GBA_Data data) => data.Shanghai_Level.Scene.GameObjects.Select(x => new Unity_Object_GBC(x, (Unity_ObjectManager_GBC)objManager)).ToArray();

        protected override GBA_SpritePalette GetSpritePalette(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) => data.Shanghai_Level.ObjPal;
    }
}