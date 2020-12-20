using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R1Engine;
using R1Engine.Serialize;
using UnityEditor;
using UnityEngine;

public class UnityWindowSerializer : SerializerObject
{
    public UnityWindowSerializer(Context context, UnityWindow window, HashSet<string> forceWrite) : base(context)
    {
        Window = window;
        ForceWrite = forceWrite;
        Foldouts = new Dictionary<string, bool>();
        CurrentName = new List<string>();
    }

    public UnityWindow Window { get; }
    public HashSet<string> ForceWrite { get; }
    public Dictionary<string, bool> Foldouts { get; }
    public override bool FullSerialize => false;
    private bool tempFlag = false;
    protected List<string> CurrentName { get; }
    public string GetFullName(string name) => String.Join(".", CurrentName.Append(name));

    public override uint CurrentLength => 0;
    public override Pointer CurrentPointer => null;
    public override void Goto(Pointer offset) { }

    public override void DoEncoded(IStreamEncoder encoder, Action action, BinaryFile.Endian? endianness = null) {
        action();
    }

    public override void DoEndian(BinaryFile.Endian endianness, Action action) {
        action();
    }

    protected Rect PrefixEditorField(string name)
    {
        Rect rect = Window.GetNextRect(ref Window.YPos);
        rect = EditorGUI.PrefixLabel(rect, new GUIContent(name));

        if (ForceWrite != null)
        {
            var fullName = GetFullName(name);

            tempFlag = ForceWrite.Contains(fullName);

            rect = Window.AffixToggle(rect, ref tempFlag);

            if (tempFlag)
                ForceWrite.Add(fullName);
            else
                ForceWrite.Remove(fullName);
        }

        return rect;
    }

    public override T Serialize<T>(T obj, string name = null)
    {
        var rect = PrefixEditorField(name);

        // Get the type
        var type = typeof(T);

        TypeCode typeCode = Type.GetTypeCode(type);

        if (type.IsEnum)
        {
            if (type.GetCustomAttributes(false).OfType<FlagsAttribute>().Any())
                return (T)(object)EditorGUI.EnumFlagsField(rect, String.Empty, (Enum)(object)obj);
            else
                return (T)(object)EditorGUI.EnumPopup(rect, String.Empty, (Enum)(object)obj);
        }

        switch (typeCode)
        {
            case TypeCode.Boolean:
                return (T)(object)Window.EditorField(String.Empty, (bool)(object)obj, rect: rect);

            case TypeCode.SByte:
                return (T)(object)(sbyte)Window.EditorField(String.Empty, (sbyte)(object)obj, rect: rect);

            case TypeCode.Byte:
                return (T)(object)(byte)Window.EditorField(String.Empty, (byte)(object)obj, rect: rect);

            case TypeCode.Int16:
                return (T)(object)(short)Window.EditorField(String.Empty, (short)(object)obj, rect: rect);

            case TypeCode.UInt16:
                return (T)(object)(ushort)Window.EditorField(String.Empty, (ushort)(object)obj, rect: rect);

            case TypeCode.Int32:
                return (T)(object)Window.EditorField(String.Empty, (int)(object)obj, rect: rect);

            case TypeCode.UInt32:
                return (T)(object)(uint)Window.EditorField(String.Empty, (uint)(object)obj, rect: rect);

            case TypeCode.Int64:
                return (T)(object)Window.EditorField(String.Empty, (long)(object)obj, rect: rect);

            case TypeCode.UInt64:
                return (T)(object)(ulong)Window.EditorField(String.Empty, (long)(ulong)(object)obj, rect: rect);

            case TypeCode.Single:
                return (T)(object)Window.EditorField(String.Empty, (float)(object)obj, rect: rect);

            case TypeCode.Double:
                return (T)(object)Window.EditorField(String.Empty, (double)(object)obj, rect: rect);

            case TypeCode.String:
                return (T)(object)Window.EditorField(String.Empty, (string)(object)obj, rect: rect);

            case TypeCode.Decimal:
            case TypeCode.Char:
            case TypeCode.DateTime:
            case TypeCode.Empty:
            case TypeCode.DBNull:

            case TypeCode.Object:
                if (type == typeof(UInt24)) {
                    return (T)(object)(UInt32)(uint)Window.EditorField(String.Empty, (uint)(UInt24)(object)obj, rect: rect);
                } else if(type == typeof(byte?)) {
                    var b = (byte?)(object)obj;
                    byte value = 0;
                    bool hasValue = b.HasValue;
                    if(hasValue) {
                        rect = Window.PrefixToggle(rect, ref hasValue);
                        value = (byte)Window.EditorField(String.Empty, b.Value, rect: rect);
                    } else {
                        rect = Window.PrefixToggle(rect, ref hasValue);
                        value = 0;
                    }
                    if(hasValue) return (T)(object)(byte?)value;
                    return (T)(object)(byte?)null;
                } else {
                    throw new NotSupportedException($"The specified generic type ('{name}') can not be read from the reader");
                }
            default:
                throw new NotSupportedException($"The specified generic type ('{name}') can not be read from the reader");
        }

    }

    public override T SerializeChecksum<T>(T calculatedChecksum, string name = null) => default;

    public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null)
    {
        CurrentName.Add(name);

        var fullName = GetFullName(name);

        if (!Foldouts.ContainsKey(fullName))
            Foldouts[fullName] = true;

        Foldouts[fullName] = EditorGUI.Foldout(Window.GetNextRect(ref Window.YPos), Foldouts[fullName], $"{name}", true);

        if (Foldouts[fullName])
        {
            Depth++;
            Window.IndentLevel++;
            obj.SerializeImpl(this);
            Window.IndentLevel--;
            Depth--;
        }

        CurrentName.RemoveAt(CurrentName.Count - 1);

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
        bool allowInvalid = false, string name = null) {
        EditorGUI.LabelField(Window.GetNextRect(ref Window.YPos), $"{name} {obj}");

        return obj;
    }

    public override T[] SerializeArray<T>(T[] obj, long count, string name = null) {
        if (obj == null) obj = new T[count];
        else if (count != obj.Length) Array.Resize(ref obj, (int)count);
        for (int i = 0; i < obj.Length; i++)
			Serialize(obj[i], name: $"{name}[{i}]");
        return obj;
    }

    public override T[] SerializeObjectArray<T>(T[] obj, long count, Action<T> onPreSerialize = null, string name = null) {
        if (obj == null) obj = new T[count];
        else if (count != obj.Length) Array.Resize(ref obj, (int)count);
        for (int i = 0; i < obj.Length; i++)
			SerializeObject(obj[i], name: $"{name}[{i}]");

        return obj;
    }

    public override Pointer[] SerializePointerArray(Pointer[] obj, long count, Pointer anchor = null, bool allowInvalid = false,
        string name = null) {
        if (obj == null) obj = new Pointer[count];
        else if (count != obj.Length) Array.Resize(ref obj, (int)count);
		for (int i = 0; i < obj.Length; i++)
			SerializePointer(obj[i], name: $"{name}[{i}]");

        return obj;
    }

    public override Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, long count, Pointer anchor = null, bool resolve = false,
        Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null)
    {
        if (obj == null) obj = new Pointer<T>[count];
        else if (count != obj.Length) Array.Resize(ref obj, (int)count);
		for (int i = 0; i < obj.Length; i++)
			SerializePointer(obj[i], name: $"{name}[{i}]");

        return obj;
    }

    public override string SerializeString(string obj, long? length = null, Encoding encoding = null, string name = null)
    {
        var rect = PrefixEditorField(name);

        return Window.EditorField(String.Empty, obj, rect: rect);
    }

    public override string[] SerializeStringArray(string[] obj, long count, int length, Encoding encoding = null, string name = null)
    {
        if (obj == null) obj = new string[count];
        else if (count != obj.Length) Array.Resize(ref obj, (int)count);
		for (int i = 0; i < obj.Length; i++)
			SerializeString(obj[i], name: $"{name}[{i}]");

        return obj;
    }

    public override T[] SerializeArraySize<T, U>(T[] obj, string name = null) => obj;

    public override void SerializeBitValues<T>(Action<SerializeBits> serializeFunc) 
    {
        serializeFunc((value, length, name) =>
        {
            var rect = PrefixEditorField(name);

            return Window.EditorField(String.Empty, value, rect: rect);
        });
    }

    public override void Log(string logString)
    {
        //EditorGUI.LabelField(Window.GetNextRect(ref Window.YPos), $"{logString}");
    }
}