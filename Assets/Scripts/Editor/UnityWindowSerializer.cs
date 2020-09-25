using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R1Engine;
using R1Engine.Serialize;
using UnityEditor;

public class UnityWindowSerializer : SerializerObject
{
    public UnityWindowSerializer(Context context, UnityWindow window) : base(context)
    {
        Window = window;
        Foldouts = new Dictionary<R1Serializable, bool>();
    }

    public UnityWindow Window { get; }
    public Dictionary<R1Serializable, bool> Foldouts { get; }
    public override bool FullSerialize => false;

    public override uint CurrentLength => 0;
    public override Pointer CurrentPointer => null;
    public override void Goto(Pointer offset) { }

    public override void DoEncoded(IStreamEncoder encoder, Action action) { }

    public override T Serialize<T>(T obj, string name = null)
    {
        // Get the type
        var type = typeof(T);

        TypeCode typeCode = Type.GetTypeCode(type);

        if (type.IsEnum)
        {
            if (type.GetCustomAttributes(false).OfType<FlagsAttribute>().Any())
                return (T)(object)EditorGUI.EnumFlagsField(Window.GetNextRect(ref Window.YPos), name, (Enum)(object)obj);
            else
                return (T)(object)EditorGUI.EnumPopup(Window.GetNextRect(ref Window.YPos), name, (Enum)(object)obj);
        }

        switch (typeCode)
        {
            case TypeCode.Boolean:
                return (T)(object)Window.EditorField(name, (bool)(object)obj);

            case TypeCode.SByte:
                return (T)(object)(sbyte)Window.EditorField(name, (sbyte)(object)obj);

            case TypeCode.Byte:
                return (T)(object)(byte)Window.EditorField(name, (byte)(object)obj);

            case TypeCode.Int16:
                return (T)(object)(short)Window.EditorField(name, (short)(object)obj);

            case TypeCode.UInt16:
                return (T)(object)(ushort)Window.EditorField(name, (ushort)(object)obj);

            case TypeCode.Int32:
                return (T)(object)Window.EditorField(name, (int)(object)obj);

            case TypeCode.UInt32:
                return (T)(object)(uint)Window.EditorField(name, (uint)(object)obj);

            case TypeCode.Int64:
                return (T)(object)Window.EditorField(name, (long)(object)obj);

            case TypeCode.UInt64:
                return (T)(object)(ulong)Window.EditorField(name, (long)(ulong)(object)obj);

            case TypeCode.Single:
                return (T)(object)Window.EditorField(name, (float)(object)obj);

            case TypeCode.Double:
                return (T)(object)Window.EditorField(name, (double)(object)obj);

            case TypeCode.String:
                return (T)(object)Window.EditorField(name, (string)(object)obj);

            case TypeCode.Decimal:
            case TypeCode.Char:
            case TypeCode.DateTime:
            case TypeCode.Empty:
            case TypeCode.DBNull:
            default:
                throw new NotSupportedException($"The specified generic type ('{name}') can not be read from the reader");
        }

    }

    public override T SerializeChecksum<T>(T calculatedChecksum, string name = null) => default;

    public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null)
    {
        if (!Foldouts.ContainsKey(obj))
            Foldouts[obj] = true;

        Foldouts[obj] = EditorGUI.Foldout(Window.GetNextRect(ref Window.YPos), Foldouts[obj], $"{name}", true);

        if (Foldouts[obj])
        {
            EditorGUI.indentLevel++;
            obj.SerializeImpl(this);
            EditorGUI.indentLevel--;
        }

        return obj;
    }

    public override Pointer SerializePointer(Pointer obj, Pointer anchor = null, bool allowInvalid = false, string name = null)
    {
        EditorGUI.BeginDisabledGroup(true);
        Window.EditorField(name, $"{obj?.ToString()}");
        EditorGUI.EndDisabledGroup();
        //EditorGUI.LabelField(Window.GetNextRect(ref Window.YPos), $"{name} {obj}");

        return obj;
    }

    public override Pointer<T> SerializePointer<T>(Pointer<T> obj, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null,
        bool allowInvalid = false, string name = null)
    {
        EditorGUI.LabelField(Window.GetNextRect(ref Window.YPos), $"{name} {obj}");

        return obj;
    }

    public override T[] SerializeArray<T>(T[] obj, long count, string name = null)
    {
        if (obj != null)
        {
            for (int i = 0; i < obj.Length; i++)
                Serialize(obj[i], name: $"{name}[{i}]");
        }

        return obj;
    }

    public override T[] SerializeObjectArray<T>(T[] obj, long count, Action<T> onPreSerialize = null, string name = null)
    {
        if (obj != null)
        {
            for (int i = 0; i < obj.Length; i++)
                SerializeObject(obj[i], name: $"{name}[{i}]");
        }

        return obj;
    }

    public override Pointer[] SerializePointerArray(Pointer[] obj, long count, Pointer anchor = null, bool allowInvalid = false,
        string name = null)
    {
        if (obj != null)
        {
            for (int i = 0; i < obj.Length; i++)
                SerializePointer(obj[i], name: $"{name}[{i}]");
        }

        return obj;
    }

    public override Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, long count, Pointer anchor = null, bool resolve = false,
        Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null)
    {
        if (obj != null)
        {
            for (int i = 0; i < obj.Length; i++)
                SerializePointer(obj[i], name: $"{name}[{i}]");
        }

        return obj;
    }

    public override string SerializeString(string obj, long? length = null, Encoding encoding = null, string name = null)
    {
        return Window.EditorField(name, obj);
    }

    public override string[] SerializeStringArray(string[] obj, long count, int length, Encoding encoding = null, string name = null)
    {
        if (obj != null)
        {
            for (int i = 0; i < obj.Length; i++)
                SerializeString(obj[i], name: $"{name}[{i}]");
        }

        return obj;
    }

    public override T[] SerializeArraySize<T, U>(T[] obj, string name = null) => obj;

    public override void SerializeBitValues<T>(Action<SerializeBits> serializeFunc) {
        serializeFunc((value, length, name) => Window.EditorField(name, value));
    }

    public override void Log(string logString)
    {
        //EditorGUI.LabelField(Window.GetNextRect(ref Window.YPos), $"{logString}");
    }
}