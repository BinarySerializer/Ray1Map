using System;

namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public abstract class OnyxFile : BinarySerializable
    {
        public bool Pre_ResolveDependencies { get; set; } = true;
        public long Pre_FileSize { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            OnyxFileResolver resolver = s.Context.GetRequiredStoredObject<OnyxFileResolver>(OnyxFileResolver.Key);

            Type type = GetType();
            string expectedID = resolver.NDS_FileTypeIDs.TryGetValue(type, out string id) ? id : null;

            if (expectedID == null)
                throw new BinarySerializableException(this, $"The NDS type {type} is not supported");

            string formatID = s.SerializeString(expectedID.Reverse(), length: 4, name: "FormatID").Reverse();

            if (expectedID != formatID)
                throw new BinarySerializableException(this, $"Invalid format ID {formatID}. Expected {expectedID}.");

            SerializeFile(s);

            // TODO: Verify full file was serialized

            if (Pre_ResolveDependencies)
                ResolveDependencies(s);
        }

        public abstract void SerializeFile(SerializerObject s);
        public virtual void ResolveDependencies(SerializerObject s) { }
    }
}