using R1Engine.Serialize;
using System.Collections.Generic;

namespace R1Engine
{
    public class GBA_Milan_Manager : GBA_BatmanVengeance_Manager
    {
        public override Unity_ObjectManager GetObjectManager(Context context, GBA_Scene scene, GBA_Data data) => new Unity_ObjectManager(context);

        public override IEnumerable<Unity_Object> GetObjects(Context context, GBA_Scene scene, Unity_ObjectManager objManager, GBA_Data data) => new Unity_Object[0];

        public override Unity_Sector[] GetSectors(GBA_Scene scene, GBA_Data data) => null;

        protected override BaseColor[] GetSpritePalette(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) => null;
    }
}