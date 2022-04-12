using PsychoPortal;
using UnityEngine;

namespace Ray1Map.Psychonauts
{
    public static class PsychonautsExtensions
    {
        public static Vector3 ToInvVector3(this Vec3 v) => new Vector3(v.X, -v.Z, v.Y);
    }
}