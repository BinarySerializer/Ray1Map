using BinarySerializer;
using System;

public class UnityWindowBitSerializer : BitSerializerObject {
    public UnityWindowBitSerializer(SerializerObject serializerObject, string logPrefix, long value) : base(serializerObject, logPrefix, value) { }

    public override T SerializeBit<T>(T value, int length, string name = null) {
        T t = SerializerObject.Serialize<T>(value, name);

        Position += length;

        return t;
    }
}
