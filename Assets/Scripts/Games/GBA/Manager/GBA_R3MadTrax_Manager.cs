using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Nintendo;
using UnityEngine;

namespace Ray1Map.GBA
{
    public class GBA_R3MadTrax_Manager : GBA_Manager
    {
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Export Mad Trax Sprites", false, true, (input, output) => ExportSpritesAsync(settings, output)), 
            }).ToArray();
        }

        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
        };

        public override string GetROMFilePath(Context context) => $"{(Files)context.GetR1Settings().World}.bin";

        public enum Files
        {
            client_pad_english,
            client_pad_french,
            client_pad_german,
            client_pad_italian,
            client_pad_spanish,

            // EU only
            client_pad145,
            client_pad2,
            client_pad3,
        }

        public override int[] MenuLevels => new int[0];
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => new int[0];
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new NotImplementedException();

        public override Unity_ObjectManager GetObjectManager(Context context, GBA_Scene scene, GBA_Data data) => new Unity_ObjectManager_GBAMadTrax(context, LoadMadTraxSprites(context).Select(x => new Unity_ObjectManager_GBAMadTrax.GraphicsData(x.ToTexture2D(), x.Offset)).ToArray());
        public override IEnumerable<Unity_SpriteObject> GetObjects(Context context, GBA_Scene scene, Unity_ObjectManager objManager, GBA_Data data) => new Unity_SpriteObject[]
        {
            new Unity_Object_GBAMadTrax((Unity_ObjectManager_GBAMadTrax)objManager)
            {
                CurrentSprite = 17,
                XPosition = 178,
                YPosition = 10
            }, 
        };

        public async UniTask ExportSpritesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Ray1MapContext(settings))
            {
                foreach (var world in GetLevels(settings).First().Worlds.Where(x => x.Maps.Length > 0).Select(x => x.Index))
                {
                    settings.World = world;

                    await context.AddMemoryMappedFile(GetROMFilePath(context), GBAConstants.Address_ROM);

                    int index = 0;

                    foreach (var spr in LoadMadTraxSprites(context))
                        Util.ByteArrayToFile(Path.Combine(outputDir, ((Files)world).ToString(), $"Sprite_{index++}_0x{spr.Offset.StringAbsoluteOffset}.png"), spr.ToTexture2D().EncodeToPNG());
                }
            }
        }

        public GBA_R3MadTraxSprite[] LoadMadTraxSprites(Context context)
        {
            var output = new List<GBA_R3MadTraxSprite>();

            var file = (Files)context.GetR1Settings().World;
            var pointerTable = PointerTables.GBA_PointerTable(context, context.GetFile(GetROMFilePath(context)));
            var s = context.Deserializer;

            s.Goto(pointerTable[DefinedPointer.MadTrax_Sprites]);

            int index = 0;

            loadSprites(width: 8, height: 8, length: 9); // Gumsi
            loadSprites(width: 8, height: 8, length: 9); // Rayman
            loadSprites(width: 2, height: 4, length: 5); // Big blocks

            if (file == Files.client_pad2 || file == Files.client_pad3)
            {
                loadSprites(width: 2, height: 4, length: 2); // Big blocks (are these correct?)
                loadSprites(width: 2, height: 4, length: 5); // Blocks
                loadSprites(width: 1, height: 2, length: 2); // Effect
                loadSprites(width: 4, height: 4, length: 1);
                loadSprites(width: 2, height: 2, length: 4);
                loadSprites(width: 2, height: 2, length: 1);
                loadSprites(width: 1, height: 1, length: 3);

                loadSprites(width: 8, height: 8, length: 1); // ?
                s.Goto(s.CurrentPointer + 0x20 * 1); // ?

                loadSprites(width: 4, height: 8, assmbleWidth: 8, assembleHeight: 2, length: 1); // Controls screen
            }
            else
            {
                loadSprites(width: 4, height: 4, length: 1); // Big blocks
                loadSprites(width: 2, height: 4, length: 1); // Missile
                loadSprites(width: 4, height: 4, length: file == Files.client_pad145 ? 6 : 21); // Blocks
                loadSprites(width: 2, height: 4, length: 6); // Missiles
                loadSprites(width: 2, height: 2, length: 1); // Missiles
                loadSprites(width: 2, height: 1, length: 1); // Effect
                loadSprites(width: 2, height: 4, length: 1); // Vertical bar

                if (file != Files.client_pad145)
                    loadSprites(width: 2, height: 1, length: 1); // Arrow

                loadSprites(width: 2, height: 2, length: 4);

                loadSprites(width: 4, height: 4, length: 7); // Explosion 
                loadSprites(width: 2, height: 2, length: 1);
                loadSprites(width: 4, height: 4, length: 1);
                loadSprites(width: 1, height: 1, length: 3);

                s.Goto(s.CurrentPointer + 0x20 * 4); // ?

                loadSprites(width: 4, height: 4, assmbleWidth: 8, assembleHeight: 5, length: 1); // Controls screen
            }

            if (file == Files.client_pad145 || file == Files.client_pad2 || file == Files.client_pad3)
                loadSprites(width: 4, height: 8, assmbleWidth: 8, assembleHeight: 1, length: 1); // Restart

            if (file == Files.client_pad2 || file == Files.client_pad3)
                loadSprites(width: 4, height: 8, assmbleWidth: 8, assembleHeight: 1, length: 1); // Try again
            else
                loadSprites(width: 8, height: 8, length: 1); // Fail

            if (file == Files.client_pad145 || file == Files.client_pad2 || file == Files.client_pad3)
                loadSprites(width: 4, height: 8, assmbleWidth: 8, assembleHeight: 1, length: 1); // Thank you
            else
                loadSprites(width: 4, height: 4, assmbleWidth: 8, assembleHeight: 3, length: 1); // Win

            loadSprites(width: 4, height: 8, assmbleWidth: 8, assembleHeight: 1, length: 1); // Connection failed

            if (file != Files.client_pad2 && file != Files.client_pad3)
            {
                loadSprites(width: 4, height: 8, assmbleWidth: 4, assembleHeight: 1, length: 1); // Pause
                loadSprites(width: 4, height: 4, length: 1); // Balloon small
                loadSprites(width: 8, height: 8, length: 1); // Balloon big
                loadSprites(width: 8, height: 4, length: 4); // 3, 2, 1, Go!
                loadSprites(width: 2, height: 2, length: 9); // 1-9
                loadSprites(width: 4, height: 2, assmbleWidth: 2, assembleHeight: 1, length: 1); // Level
            }

            void loadSprites(int width, int height, int assmbleWidth = 1, int assembleHeight = 1, int length = 1)
            {
                for (int i = 0; i < length; i++)
                    output.Add(s.SerializeObject<GBA_R3MadTraxSprite>(default, onPreSerialize: x =>
                    {
                        x.Width = width;
                        x.Height = height;
                        x.AssembleWidth = assmbleWidth;
                        x.AssembleHeight = assembleHeight;
                    }, name: $"Sprite[{index++}]"));
            }

            return output.ToArray();
        }
    }
}