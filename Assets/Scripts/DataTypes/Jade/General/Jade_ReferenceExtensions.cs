using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public static class Jade_ReferenceExtensions
    {
        public static Jade_Reference<T>[] Resolve<T>(this Jade_Reference<T>[] refs,
            Action<SerializerObject, T> onPreSerialize = null, Action<SerializerObject, T> onPostSerialize = null,
            bool immediate = false,
            LOA_Loader.QueueType queue = LOA_Loader.QueueType.Current,
            LOA_Loader.CacheType cache = LOA_Loader.CacheType.Current,
            LOA_Loader.ReferenceFlags flags = LOA_Loader.ReferenceFlags.Log)
            where T : Jade_File, new()
        {
            foreach (var r in refs)
                r?.Resolve(onPreSerialize: onPreSerialize, onPostSerialize: onPostSerialize,
                    immediate: immediate, queue: queue, cache: cache, flags: flags);

            return refs;
        }
    }
}