using System.IO;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public static class ContextExtensions
    {
        public static async UniTask<LinearSerializedFile> AddLinearSerializedFileAsync(this Context context, string filePath, BinaryFile.Endian endianness = BinaryFile.Endian.Little)
        {
            await FileSystem.PrepareFile(context.BasePath + filePath);

            if (!FileSystem.FileExists(context.BasePath + filePath))
                return null;

            var file = new LinearSerializedFile(context)
            {
                filePath = filePath,
                Endianness = endianness
            };

            context.AddFile(file);

            return file;
        }
        public static async UniTask<MemoryMappedFile> AddMemoryMappedFile(this Context context, string filePath, uint baseAddress, BinaryFile.Endian endianness = BinaryFile.Endian.Little)
        {
            await FileSystem.PrepareFile(context.BasePath + filePath);

            if (!FileSystem.FileExists(context.BasePath + filePath))
                return null;

            var file = new MemoryMappedFile(context, baseAddress)
            {
                filePath = filePath,
                Endianness = endianness
            };

            context.AddFile(file);

            return file;
        }
        public static async UniTask<GBAMemoryMappedFile> AddGBAMemoryMappedFile(this Context context, string filePath, uint baseAddress, BinaryFile.Endian endianness = BinaryFile.Endian.Little)
        {
            await FileSystem.PrepareFile(context.BasePath + filePath);

            if (!FileSystem.FileExists(context.BasePath + filePath))
                return null;

            var file = new GBAMemoryMappedFile(context, baseAddress)
            {
                filePath = filePath,
                Endianness = endianness
            };

            context.AddFile(file);

            return file;
        }
        public static StreamFile AddStreamFile(this Context context, string name, Stream stream, BinaryFile.Endian endianness = BinaryFile.Endian.Little)
        {
            var file = new StreamFile(name, stream, context)
            {
                Endianness = endianness
            };

            context.AddFile(file);

            return file;
        }
        public static MemoryMappedByteArrayFile AddMemoryMappedByteArrayFile(this Context context, string name, uint length, uint baseAddress, BinaryFile.Endian endianness = BinaryFile.Endian.Little)
        {
            var file = new MemoryMappedByteArrayFile(name, length, context, baseAddress)
            {
                Endianness = endianness
            };

            context.AddFile(file);

            return file;
        }
        public static MemoryMappedByteArrayFile AddMemoryMappedByteArrayFile(this Context context, string name, byte[] bytes, uint baseAddress, BinaryFile.Endian endianness = BinaryFile.Endian.Little)
        {
            var file = new MemoryMappedByteArrayFile(name, bytes, context, baseAddress)
            {
                Endianness = endianness
            };

            context.AddFile(file);

            return file;
        }
    }
}