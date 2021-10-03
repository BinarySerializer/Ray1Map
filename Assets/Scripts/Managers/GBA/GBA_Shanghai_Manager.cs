using System.Collections.Generic;
using System.Linq;
using BinarySerializer;


namespace R1Engine
{
    public class GBA_Shanghai_Manager : GBA_BatmanVengeance_Manager
    {
        public override Unity_ObjectManager GetObjectManager(Context context, GBA_Scene scene, GBA_Data data) => new Unity_ObjectManager_GBC(context, data.Shanghai_Scene.Actors.ActorModels.Select((x, i) => new Unity_ObjectManager_GBC.ActorModel(i, x.Shanghai_ActionTable.ActionTable.Actions, GetCommonDesign(x.Shanghai_ActionTable.Puppet, false, data, new GBA_Animation[0]))).ToArray());

        public override IEnumerable<Unity_SpriteObject> GetObjects(Context context, GBA_Scene scene, Unity_ObjectManager objManager, GBA_Data data) => data.Shanghai_Scene.Actors.Actors.Concat(data.Shanghai_Scene.Captors.Captors).Select(x => new Unity_Object_GBC(x, (Unity_ObjectManager_GBC)objManager)).ToArray();

        public override Unity_Sector[] GetSectors(GBA_Scene scene, GBA_Data data) => data.Shanghai_Scene.Knots.Knots.Select(x => new Unity_Sector(x.Actors.Select(v => v - 1).ToList())).ToArray();

        protected override BaseColor[] GetSpritePalette(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) => puppet.IsObjAnimation ? data.Shanghai_Scene.ObjPal.Palette : data.Shanghai_Scene.TilePal.Palette;
    }
}