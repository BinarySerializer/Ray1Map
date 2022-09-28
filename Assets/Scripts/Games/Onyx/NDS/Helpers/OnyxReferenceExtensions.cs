using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public static class OnyxReferenceExtensions
    {
        public static void Resolve<T>(this IEnumerable<FileReference<T>> refs, SerializerObject s)
            where T : OnyxFile, new()
        {
            if (refs == null)
                return;

            foreach (FileReference<T> r in refs)
                r?.Resolve(s);
        }
    }
}