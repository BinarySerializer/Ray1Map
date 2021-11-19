using System.Linq;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public class VertexAnimationComponent : ObjectAnimationComponent
    {
        public MeshData[] meshes;
        public Frame[] frames;

        public struct Frame
        {
            public Vector3[] vertices;
            public Vector3[] normals;
        }

        public struct MeshData
        {
            public Mesh mesh;
            public ushort[] vertexIndices;
            public ushort[] normalIndices;
        }

        protected override void UpdateAnimation()
        {
            if (meshes == null || frames == null)
                return;

            speed.Update(frames.Length, loopMode);

            int frameInt = speed.CurrentFrameInt;

            int nextFrameIndex = frameInt + 1 * speed.Direction;

            if (nextFrameIndex >= frames.Length)
            {
                switch (loopMode)
                {
                    case AnimLoopMode.Repeat:
                        nextFrameIndex = 0;
                        break;

                    case AnimLoopMode.PingPong:
                        nextFrameIndex = frames.Length - 1;
                        break;
                }
            }
            else if (nextFrameIndex < 0)
            {
                switch (loopMode)
                {
                    case AnimLoopMode.PingPong:
                        nextFrameIndex = 1;
                        break;
                }
            }

            Frame currentFrame = frames[frameInt];
            Frame nextFrame = frames[nextFrameIndex];

            float lerpFactor = speed.CurrentFrame - frameInt;

            Vector3[] vertices = Enumerable.Range(0, currentFrame.vertices.Length)
                .Select(x => Vector3.Lerp(currentFrame.vertices[x], nextFrame.vertices[x], lerpFactor)).ToArray();

            foreach (MeshData meshData in meshes)
            {
                meshData.mesh.SetVertices(meshData.vertexIndices.Select(x => vertices[x]).ToArray());
                // TODO: Set normals when needed
            }
        }
    }
}