using System.Linq;
using BinarySerializer.Klonoa.DTP;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public sealed class Unity_Object_PSKlonoa_DTP_CutsceneSprite : Unity_Object_BasePSKlonoa_DTP
    {
        public Unity_Object_PSKlonoa_DTP_CutsceneSprite(Unity_ObjectManager_PSKlonoa_DTP objManager, Vector3 pos, int animIndex, int cutsceneIndex, int objIndex, bool flipX) : base(objManager)
        {
            CutsceneIndex = cutsceneIndex;
            ObjIndex = objIndex;
            Position = pos;
            SpriteSetIndex = GetSpriteSetIndex(Unity_ObjectManager_PSKlonoa_DTP.SpritesType.Cutscene);
            AnimIndex = animIndex;
            FlipHorizontally = flipX;
            CutsceneScriptIndex = objManager.CutsceneScripts.FindItemIndex(x => x.CutsceneIndex == cutsceneIndex && x.IsNormalCutscene);
        }

        public int CutsceneIndex { get; }
        public int ObjIndex { get; }

        public override string PrimaryName => $"Cutscene_{CutsceneIndex}_{ObjIndex}";

        public override PrimaryObjectType PrimaryType => PrimaryObjectType.CutsceneObject;
        public override int SecondaryType => 0;

        public override bool IsAlways => true;

        public override bool FlipHorizontally { get; }

        public int CutsceneScriptIndex { get; set; }
        public Unity_ObjectManager_PSKlonoa_DTP.CutsceneScript CutsceneScript => 
            ObjManager.CutsceneScripts.ElementAtOrDefault(CutsceneScriptIndex);
    }
}