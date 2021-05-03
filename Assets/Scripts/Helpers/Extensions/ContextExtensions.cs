using System.IO;
using BinarySerializer;
using Cysharp.Threading.Tasks;


namespace R1Engine
{
    public static class ContextExtensions
    {
        public static async UniTask<LinearSerializedFile> AddLinearSerializedFileAsync(this Context context, string filePath, Endian endianness = Endian.Little, bool recreateOnWrite = true, int? bigFileCacheLength = null)
        {
            var absolutePath = context.GetAbsoluteFilePath(filePath);

            if (bigFileCacheLength.HasValue) {
                await FileSystem.PrepareBigFile(absolutePath, bigFileCacheLength.Value);
            } else {
                await FileSystem.PrepareFile(absolutePath);
            }

            if (!FileSystem.FileExists(absolutePath))
                return null;

            var file = new LinearSerializedFile(context, filePath, endianness)
            {
                RecreateOnWrite = recreateOnWrite
            };

            context.AddFile(file);

            return file;
        }
        public static async UniTask<MemoryMappedFile> AddMemoryMappedFile(this Context context, string filePath, uint baseAddress, Endian endianness = Endian.Little, bool recreateOnWrite = true)
        {
            var absolutePath = context.GetAbsoluteFilePath(filePath);

            await FileSystem.PrepareFile(absolutePath);

            if (!FileSystem.FileExists(absolutePath))
                return null;

            var file = new MemoryMappedFile(context, filePath, baseAddress, endianness)
            {
                RecreateOnWrite = recreateOnWrite
            };

            context.AddFile(file);

            return file;
        }
        public static async UniTask<GBAMemoryMappedFile> AddGBAMemoryMappedFile(this Context context, string filePath, uint baseAddress, Endian endianness = Endian.Little, bool recreateOnWrite = true)
        {
            var absolutePath = context.GetAbsoluteFilePath(filePath);

            await FileSystem.PrepareFile(absolutePath);

            if (!FileSystem.FileExists(absolutePath))
                return null;

            var file = new GBAMemoryMappedFile(context, filePath, baseAddress, endianness)
            {
                RecreateOnWrite = recreateOnWrite
            };

            context.AddFile(file);

            return file;
        }
        public static StreamFile AddStreamFile(this Context context, string name, Stream stream, Endian endianness = Endian.Little, bool recreateOnWrite = true, bool allowLocalPointers = false)
        {
            var file = new StreamFile(context, name, stream, endianness, allowLocalPointers)
            {
                RecreateOnWrite = recreateOnWrite
            };

            context.AddFile(file);

            return file;
        }
        public static MemoryMappedByteArrayFile AddMemoryMappedByteArrayFile(this Context context, string name, uint length, uint baseAddress, Endian endianness = Endian.Little, bool recreateOnWrite = true)
        {
            var file = new MemoryMappedByteArrayFile(context, name, length, baseAddress, endianness)
            {
                RecreateOnWrite = recreateOnWrite
            };

            context.AddFile(file);

            return file;
        }
        public static MemoryMappedByteArrayFile AddMemoryMappedByteArrayFile(this Context context, string name, byte[] bytes, uint baseAddress, Endian endianness = Endian.Little, bool recreateOnWrite = true)
        {
            var file = new MemoryMappedByteArrayFile(context, name, baseAddress, bytes, endianness)
            {
                RecreateOnWrite = recreateOnWrite
            };

            context.AddFile(file);

            return file;
        }
    }
}