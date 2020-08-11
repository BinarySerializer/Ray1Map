using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    public class NGage_R3_Manager : GBA_R3_Manager
    {
        public override string GetROMFilePath => $"rayman3.dat";

        public override async UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading data";
            await Controller.WaitIfNecessary();

            // Read the rom
            var rom = FileFactory.Read<GBA_Data>(GetROMFilePath, context);

            var commonLev = await CreateCommonLev(context, rom.LevelBlock);

            return new GBA_EditorManager(commonLev, context);
        }

        public override async UniTask LoadFilesAsync(Context context)
        {
            await FileSystem.PrepareFile(context.BasePath + GetROMFilePath);

            var file = new LinearSerializedFile(context)
            {
                filePath = GetROMFilePath,
            };
            context.AddFile(file);
        }
    }
}