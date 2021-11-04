using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public static class KlonoaHelpers
    {
        public static Vector3 GetPosition(float x, float y, float z, float scale) => new Vector3(x / scale, -z / scale, -y / scale);

        public static Vector3 GetPosition(this MovementPathBlock[] path, int position, Vector3 relativePos, float scale)
        {
            var blockIndex = 0;
            int blockPosOffset;

            if (position < 0)
            {
                blockIndex = 0;
                blockPosOffset = position;
            }
            else
            {
                var iVar6 = 0;

                do
                {
                    var iVar2 = path[blockIndex].BlockLength;

                    if (iVar2 == 0x7ffe)
                    {
                        blockIndex = 0;
                    }
                    else
                    {
                        if (iVar2 == 0x7fff)
                        {
                            iVar6 -= path[blockIndex - 1].BlockLength;
                            break;
                        }

                        iVar6 += iVar2;
                        blockIndex++;
                    }
                } while (iVar6 <= position);

                iVar6 -= position;

                blockIndex--;

                if (iVar6 < 0)
                    blockPosOffset = -iVar6;
                else
                    blockPosOffset = path[blockIndex].BlockLength - iVar6;
            }

            var block = path[blockIndex];

            float xPos = block.XPos + block.DirectionX * blockPosOffset + relativePos.x;
            float yPos = block.YPos + block.DirectionY * blockPosOffset + relativePos.y;
            float zPos = block.ZPos + block.DirectionZ * blockPosOffset + relativePos.z;

            return GetPosition(xPos, yPos, zPos, scale);
        }

        public static Vector3 GetPositionVector(this KlonoaVector16 pos, Vector3? posOffset, float scale)
        {
            if (posOffset == null)
                return new Vector3(pos.X / scale, -pos.Y / scale, pos.Z / scale);
            else
                return new Vector3((pos.X + posOffset.Value.x) / scale, -(pos.Y + posOffset.Value.y) / scale, (pos.Z + posOffset.Value.z) / scale);
        }

        public static Bounds GetDimensions(this PS1_TMD tmd, float scale)
        {
            var verts = tmd.Objects.SelectMany(x => x.Vertices).ToArray();
            var min = new Vector3(verts.Min(v => v.X), verts.Min(v => v.Y), verts.Min(v => v.Z)) / scale;
            var max = new Vector3(verts.Max(v => v.X), verts.Max(v => v.Y), verts.Max(v => v.Z)) / scale;
            var center = Vector3.Lerp(min, max, 0.5f);

            return new Bounds(center, max - min);
        }

        public static float GetRotationInDegrees(float value)
        {
            if (value > 0x800)
                value -= 0x1000;

            return value * (360f / 0x1000);
        }

        public static Quaternion GetQuaternion(this KlonoaVector16 rot)
        {
            return GetQuaternion(rot.X, rot.Y, rot.Z);
        }

        public static Quaternion GetQuaternion(float rotX, float rotY, float rotZ)
        {
            return
                Quaternion.Euler(-GetRotationInDegrees(rotX), 0, 0) *
                Quaternion.Euler(0, GetRotationInDegrees(rotY), 0) *
                Quaternion.Euler(0, 0, -GetRotationInDegrees(rotZ));
        }

        public static Vector3[] GetPositions(this ModelBoneAnimation_ArchiveFile anim, int boneIndex, float scale)
        {
            return anim.Positions.Vectors.
                Select(x => x[boneIndex].GetPositionVector(Vector3.zero, scale)).
                ToArray();
        }

        public static Quaternion[] GetRotations(this ModelBoneAnimation_ArchiveFile anim, int boneIndex)
        {
            int[] rotX = anim.Rotations.GetValues(boneIndex * 3 + 0);
            int[] rotY = anim.Rotations.GetValues(boneIndex * 3 + 1);
            int[] rotZ = anim.Rotations.GetValues(boneIndex * 3 + 2);

            return Enumerable.Range(0, anim.Rotations.FramesCount).
                Select(x => KlonoaHelpers.GetQuaternion(rotX[x], rotY[x], rotZ[x])).
                ToArray();
        }

        public static bool ApplyTransform(GameObject gameObj, IReadOnlyList<ModelAnimation_ArchiveFile> transforms, float scale, int objIndex = 0, AnimSpeed animSpeed = null, AnimLoopMode animLoopMode = AnimLoopMode.Repeat)
        {
            if (transforms?.Any() == true && transforms[0].Positions.Vectors[0].Length == 1)
                objIndex = 0;

            if (transforms != null && transforms.Any() && transforms[0].Positions.ObjectsCount > objIndex)
            {
                gameObj.transform.localPosition = transforms[0].Positions.Vectors[0][objIndex].GetPositionVector(null, scale);
                gameObj.transform.localRotation = transforms[0].Rotations.Vectors[0][objIndex].GetQuaternion();
            }
            else
            {
                gameObj.transform.localPosition = Vector3.zero;
                gameObj.transform.localRotation = Quaternion.identity;
            }

            if (!(transforms?.FirstOrDefault()?.Positions.Vectors.Length > 1))
                return false;

            var mtComponent = gameObj.AddComponent<AnimatedTransformComponent>();
            mtComponent.animatedTransform = gameObj.transform;

            if (animSpeed != null)
                mtComponent.speed = animSpeed;

            mtComponent.loopMode = animLoopMode;

            var positions = transforms.SelectMany(x => x.Positions.Vectors).Select(x =>
                x.Length > objIndex ? x[objIndex].GetPositionVector(null, scale) : (Vector3?)null).ToArray();
            var rotations = transforms.SelectMany(x => x.Rotations.Vectors)
                .Select(x => x.Length > objIndex ? x[objIndex].GetQuaternion() : (Quaternion?)null).ToArray();

            var frameCount = Math.Max(positions.Length, rotations.Length);
            mtComponent.frames = new AnimatedTransformComponent.Frame[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                mtComponent.frames[i] = new AnimatedTransformComponent.Frame()
                {
                    Position = positions[i] ?? Vector3.zero,
                    Rotation = rotations[i] ?? Quaternion.identity,
                    Scale = Vector3.one,
                    IsHidden = positions[i] == null || rotations[i] == null,
                };
            }

            return true;
        }

        public static void GenerateCutsceneTextTranslation(Loader loader, Dictionary<string, char> d, int cutscene, int instruction, string text)
        {
            var c = loader.LevelPack.CutscenePack.Cutscenes[cutscene];
            var i = (CutsceneInstructionData_DrawText)c.Cutscene_Normal.Instructions[instruction].Data;

            var textIndex = 0;

            foreach (var cmd in i.TextCommands)
            {
                if (cmd.Type != CutsceneInstructionData_DrawText.TextCommand.CommandType.DrawChar)
                    continue;

                var charImgData = c.Font.CharactersImgData[cmd.Command];

                using SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

                var hash = Convert.ToBase64String(sha1.ComputeHash(charImgData));

                if (d.ContainsKey(hash))
                {
                    if (d[hash] != text[textIndex])
                        Debug.LogWarning($"Character {text[textIndex]} has multiple font textures!");
                }
                else
                {
                    d[hash] = text[textIndex];
                }

                textIndex++;
            }

            var str = new StringBuilder();

            foreach (var v in d.OrderBy(x => x.Value))
            {
                var value = v.Value.ToString();

                if (value == "\"" || value == "\'")
                    value = $"\\{value}";

                str.AppendLine($"[\"{v.Key}\"] = '{value}',");
            }

            str.ToString().CopyToClipboard();
        }
    }
}