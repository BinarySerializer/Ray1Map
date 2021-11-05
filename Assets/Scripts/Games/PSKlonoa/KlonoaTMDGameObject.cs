using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using System.Linq;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public class KlonoaTMDGameObject : TMDGameObject
    {
        public KlonoaTMDGameObject(
            PS1_TMD tmd, 
            PS1_VRAM vram,
            float scale,
            KlonoaObjectsLoader objectsLoader,
            bool isPrimaryObj,
            ModelAnimation_ArchiveFile[] animations = null,
            AnimSpeed animSpeed = null,
            AnimLoopMode animLoopMode = AnimLoopMode.Repeat,
            ArchiveFile<ModelBoneAnimation_ArchiveFile> boneAnimations = null) : base(tmd, vram, scale)
        {
            ObjectsLoader = objectsLoader;
            IsPrimaryObj = isPrimaryObj;
            Animations = animations;
            AnimSpeed = animSpeed;
            AnimLoopMode = animLoopMode;
            BoneAnimations = boneAnimations;
        }

        public KlonoaObjectsLoader ObjectsLoader { get; }
        public bool IsPrimaryObj { get; }
        public ModelAnimation_ArchiveFile[] Animations { get; }
        public AnimSpeed AnimSpeed { get; }
        public AnimLoopMode AnimLoopMode { get; }
        public ArchiveFile<ModelBoneAnimation_ArchiveFile> BoneAnimations { get; }

        protected override void OnGetTextureBounds(PS1_TMD_Packet packet, PS1VRAMTexture tex)
        {
            // Expand with UV scroll
            if (IsPrimaryObj && packet.UV.Any(x => ObjectsLoader.ScrollAnimations.SelectMany(a => a.UVOffsets).Contains((int)(x.Offset.FileOffset - TMD.Objects[0].Offset.FileOffset))))
            {
                tex.Bounds = new RectInt(
                    tex.Bounds.x,
                    0,
                    tex.Bounds.width,
                    192);
            }
        }

        protected override void OnCreateTexture(PS1VRAMTexture tex)
        {
            // Check if the texture is animated
            var vramAnims = ObjectsLoader.Anim_GetAnimationsFromRegion(tex.TextureRegion, tex.PaletteRegion).ToArray();

            if (!vramAnims.Any())
                return;

            var animatedTexture = new PS1VRAMAnimatedTexture(tex.Bounds.width, tex.Bounds.height, true, t =>
            {
                tex.GetTexture(VRAM, t);
            }, vramAnims);

            ObjectsLoader.Anim_Manager.AddAnimatedTexture(animatedTexture);
            tex.SetAnimatedTexture(animatedTexture);
        }

        protected override void OnCreatedBones(GameObject gameObject, PS1_TMD_Object obj, Transform[] bones)
        {
            if (BoneAnimations == null)
            {
                Debug.LogWarning($"Object has bones but no animation");
                return;
            }

            var animComponent = gameObject.AddComponent<SkeletonAnimationComponent>();
            animComponent.animations = new SkeletonAnimationComponent.Animation[BoneAnimations.Files.Length];

            if (AnimSpeed != null)
                animComponent.speed = AnimSpeed;

            animComponent.loopMode = AnimLoopMode;

            for (int animIndex = 0; animIndex < BoneAnimations.Files.Length; animIndex++)
            {
                ModelBoneAnimation_ArchiveFile anim = BoneAnimations.Files[animIndex];
                animComponent.animations[animIndex].bones = new SkeletonAnimationComponent.Bone[obj.Bones.Length];

                short frameCount = anim.Rotations.FramesCount;

                for (int boneIndex = 0; boneIndex < animComponent.animations[animIndex].bones.Length; boneIndex++)
                {
                    animComponent.animations[animIndex].bones[boneIndex].animatedTransform = bones[boneIndex + 1];

                    Vector3[] positions = anim.GetPositions(boneIndex, Scale);
                    Quaternion[] rotations = anim.GetRotations(boneIndex);

                    animComponent.animations[animIndex].bones[boneIndex].frames = new SkeletonAnimationComponent.Frame[frameCount];

                    for (int i = 0; i < frameCount; i++)
                    {
                        animComponent.animations[animIndex].bones[boneIndex].frames[i] = new SkeletonAnimationComponent.Frame()
                        {
                            Position = positions[i],
                            Rotation = rotations[i],
                            Scale = Vector3.one,
                        };
                    }
                }
            }

            // TODO: Support selecting multiple animations
            animComponent.CombineAnimations(obj.BonesCount);
        }

        protected override void OnCreateObject(GameObject gameObject, PS1_TMD_Object obj, int objIndex)
        {
            var isTransformAnimated = KlonoaHelpers.ApplyTransform(
                gameObj: gameObject,
                transforms: Animations,
                scale: Scale,
                objIndex: objIndex,
                animSpeed: AnimSpeed?.CloneAnimSpeed(),
                animLoopMode: AnimLoopMode);

            if (isTransformAnimated)
                HasAnimations = true;
        }

        protected override void OnAppliedTexture(GameObject packetGameObject, PS1_TMD_Object obj, PS1_TMD_Packet packet, Material mat, PS1VRAMTexture tex)
        {
            // Check for UV scroll animations
            if (IsPrimaryObj && packet.UV.Any(x => ObjectsLoader.ScrollAnimations.SelectMany(a => a.UVOffsets).Contains((int)(x.Offset.FileOffset - TMD.Objects[0].Offset.FileOffset))))
            {
                HasAnimations = true;
                var animTex = packetGameObject.AddComponent<AnimatedTextureComponent>();
                animTex.material = mat;
                animTex.scrollV = -2f * 60f / (tex?.Bounds.height ?? 256);
            }
        }
    }
}