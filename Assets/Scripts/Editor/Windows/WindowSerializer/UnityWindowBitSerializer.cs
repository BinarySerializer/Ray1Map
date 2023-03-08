using BinarySerializer;
using System;
using UnityEditor;

public class UnityWindowBitSerializer : BitSerializerObject 
{
    public new UnityWindowSerializer SerializerObject => (UnityWindowSerializer)base.SerializerObject;

    public UnityWindowBitSerializer(SerializerObject serializerObject, Pointer valueOffset, string logPrefix, long value) 
        : base(serializerObject, valueOffset, logPrefix, value) { }

    public override T SerializeBits<T>(T value, int length, SignedNumberRepresentation sign = SignedNumberRepresentation.Unsigned, string name = null) 
    {
        T t = SerializerObject.Serialize<T>(value, name);

        Position += length;

        return t;
    }

    public override T? SerializeNullableBits<T>(T? value, int length, string name = null)
    {
        T? t = SerializerObject.SerializeNullable<T>(value, name);

        Position += length;

        return t;
    }

    public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null) {
        SerializerObject.CurrentName.Add(name);

        var fullName = SerializerObject.GetFullName(name);

        if (!SerializerObject.Foldouts.ContainsKey(fullName))
            SerializerObject.Foldouts[fullName] = true;

        SerializerObject.Foldouts[fullName] = EditorGUI.Foldout(SerializerObject.Window.GetNextRect(ref SerializerObject.Window.YPos), SerializerObject.Foldouts[fullName], $"{name}", true);

        if (SerializerObject.Foldouts[fullName]) {
            Depth++;
            SerializerObject.Window.IndentLevel++;
            if (obj == null) obj = new T();
            obj.SerializeImpl(this);
            SerializerObject.Window.IndentLevel--;
            Depth--;
        }

        SerializerObject.CurrentName.RemoveAt(SerializerObject.CurrentName.Count - 1);

        return obj;
    }
}