using R1Engine.Serialize;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBA_Milan_Manager : GBA_BatmanVengeance_Manager
    {
        public override Unity_ObjectManager GetObjectManager(Context context, GBA_Scene scene, GBA_Data data) => 
            new Unity_ObjectManager_GBA(
                context: context, 
                actorModels: data.Milan_SceneList.Scene.ActorsBlock.ActorModels.Select((x, i) => 
                    new Unity_ObjectManager_GBA.ModelData(
                        index: i, 
                        actions: x.ActionTable.ActionBlocks.Select(y => y.Action).ToArray(), 
                        graphics: GetCommonDesign(x.BasePuppet.Puppet, x.BasePuppet.Puppet.Milan_TileKit.Is8bpp), 
                        name: x.ActorID)).ToArray());

        public override IEnumerable<Unity_Object> GetObjects(Context context, GBA_Scene scene, Unity_ObjectManager objManager, GBA_Data data) => data.Milan_SceneList.Scene.ActorsBlock.Actors.Concat(data.Milan_SceneList.Scene.CaptorsBlock.Actors).Select(x => new Unity_Object_GBAMilan(x, (Unity_ObjectManager_GBA)objManager));

        public override Unity_Sector[] GetSectors(GBA_Scene scene, GBA_Data data) => null;

        protected override BaseColor[] GetSpritePalette(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) => null;
    }
}