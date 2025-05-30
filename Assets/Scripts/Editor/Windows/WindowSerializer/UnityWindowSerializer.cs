﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;
using UnityEditor;
using UnityEngine;

// TODO: Enable nullable

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
    public override bool UsesSerializeNames => true;
    public override bool FullSerialize => false;
    private bool tempFlag = false;
    public List<string> CurrentName { get; }
    public string GetFullName(string name) => String.Join(".", CurrentName.Append(name));
	protected BinaryFile? CurrentFile { get; set; }

	public override long CurrentLength => 0;
	public override bool HasCurrentPointer => CurrentFile != null;
	public override BinaryFile CurrentBinaryFile => CurrentFile ?? throw new SerializerMissingCurrentPointerException();
	public override long CurrentFileOffset => 0;

    public override void Goto(Pointer offset) {
		if (offset == null) {
			CurrentFile = null;
		} else {
			BinaryFile newFile = offset.File;

			if (newFile != CurrentFile || !HasCurrentPointer) {
				CurrentFile = newFile;
			}
		}
	}
    public override void Align(int alignBytes = 4, Pointer baseOffset = null, bool? logIfNotNull = null) { }

    public override void DoEncoded(IStreamEncoder encoder, Action action, Endian? endianness = null, bool allowLocalPointers = false, string filename = null) {
        action();
    }

    public override void BeginProcessed(BinaryProcessor processor)
    {
        throw new NotImplementedException();
    }

    public override void EndProcessed(BinaryProcessor processor)
    {
        throw new NotImplementedException();
    }

    public override T GetProcessor<T>() 
        where T : class
    {
        throw new NotImplementedException();
    }

    public override Pointer BeginEncoded(IStreamEncoder encoder, Endian? endianness = null, bool allowLocalPointers = false, string filename = null) => null;
    public override void EndEncoded(Pointer endPointer) {}

    public override void DoEndian(Endian endianness, Action action) {
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

            case TypeCode.Object when type == typeof(UInt24):
                return (T)(object)(UInt32)(uint)Window.EditorField(String.Empty, (uint)(UInt24)(object)obj, rect: rect);

            case TypeCode.String:
            case TypeCode.Decimal:
            case TypeCode.Char:
            case TypeCode.DateTime:
            case TypeCode.Empty:
            case TypeCode.DBNull:
            default:
                throw new NotSupportedException($"The specified generic type ('{name}') can not be read from the reader");
        }

    }

    public override T? SerializeNullable<T>(T? obj, string name = null)
    {
        throw new NotImplementedException();
    }

    public override bool SerializeBoolean<T>(bool obj, string name = null)
    {
        var rect = PrefixEditorField(name);
        return Window.EditorField(String.Empty, obj, rect: rect);
    }

    public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null)
    {
        CurrentName.Add(name);

        var fullName = GetFullName(name);

        if (!Foldouts.ContainsKey(fullName))
            Foldouts[fullName] = true;

        Foldouts[fullName] = EditorGUI.Foldout(Window.GetNextRect(ref Window.YPos), Foldouts[fullName], $"{name}", true);

        if (Foldouts[fullName])
        {
            try
            {
                Depth++;
                Window.IndentLevel++;
                obj ??= new T();
                obj.SerializeImpl(this);
            }
            finally
            {
                Window.IndentLevel--;
                Depth--;
            }
        }

        CurrentName.RemoveAt(CurrentName.Count - 1);

        return obj;
    }

    public override Pointer SerializePointer(Pointer obj, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null, string name = null)
    {
        EditorGUI.BeginDisabledGroup(true);
        Window.EditorField(name, $"{obj?.ToString()}");
        EditorGUI.EndDisabledGroup();
        //EditorGUI.LabelField(Window.GetNextRect(ref Window.YPos), $"{name} {obj}");

        return obj;
    }

    public override Pointer<T> SerializePointer<T>(Pointer<T> obj, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null, string name = null) {
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

    public override T?[] SerializeNullableArray<T>(T?[] obj, long count, string name = null)
    {
        if (obj == null) obj = new T?[count];
        else if (count != obj.Length) Array.Resize(ref obj, (int)count);
        for (int i = 0; i < obj.Length; i++)
            SerializeNullable(obj[i], name: $"{name}[{i}]");
        return obj;
    }

    public override T[] SerializeObjectArray<T>(T[] obj, long count, Action<T, int> onPreSerialize = null, string name = null) {
        if (obj == null) obj = new T[count];
        else if (count != obj.Length) Array.Resize(ref obj, (int)count);
        for (int i = 0; i < obj.Length; i++)
            SerializeObject(obj[i], name: $"{name}[{i}]");

        return obj;
    }

    public override Pointer[] SerializePointerArray(Pointer[] obj, long count, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null,
        string name = null) {
        if (obj == null) obj = new Pointer[count];
        else if (count != obj.Length) Array.Resize(ref obj, (int)count);
        for (int i = 0; i < obj.Length; i++)
            SerializePointer(obj[i], size: size, anchor: anchor, allowInvalid: allowInvalid, nullValue: nullValue, name: $"{name}[{i}]");

        return obj;
    }

    public override Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, long count, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null, string name = null)
    {
        if (obj == null) obj = new Pointer<T>[count];
        else if (count != obj.Length) Array.Resize(ref obj, (int)count);
        for (int i = 0; i < obj.Length; i++)
            SerializePointer(obj[i], size: size, anchor: anchor, allowInvalid: allowInvalid, nullValue: nullValue, name: $"{name}[{i}]");

        return obj;
    }

    public override string SerializeString(string obj, long? length = null, Encoding encoding = null, string name = null)
    {
        var rect = PrefixEditorField(name);

        return Window.EditorField(String.Empty, obj, rect: rect);
    }

    public override string SerializeLengthPrefixedString<T>(string obj, Encoding encoding = null, string name = null)
    {
        throw new NotImplementedException();
    }

    public override T SerializeInto<T>(T obj, SerializeInto<T> serializeFunc, string name = null) where T : default
    {
        throw new NotImplementedException();
    }

    public override string[] SerializeStringArray(string[] obj, long count, long? length = null, Encoding encoding = null, string name = null)
    {
        if (obj == null) obj = new string[count];
        else if (count != obj.Length) Array.Resize(ref obj, (int)count);
        for (int i = 0; i < obj.Length; i++)
            SerializeString(obj[i], name: $"{name}[{i}]");

        return obj;
    }

    public override string[] SerializeLengthPrefixedStringArray<T>(string[] obj, long count, Encoding encoding = null,
        string name = null)
    {
        throw new NotImplementedException();
    }

    public override T[] SerializeIntoArray<T>(T[] obj, long count, SerializeInto<T> serializeFunc, string name = null) where T : default
    {
        throw new NotImplementedException();
    }

    public override T[] SerializeArraySize<T, U>(T[] obj, string name = null) => obj;

    public override void SerializeBitValues(Action<SerializeBits64> serializeFunc)
    {
        serializeFunc((value, length, name) =>
        {
            var rect = PrefixEditorField(name);

            return Window.EditorField(String.Empty, value, rect: rect);
        });
    }

    public override void DoBits<T>(Action<BitSerializerObject> serializeFunc) 
    {
        serializeFunc(new UnityWindowBitSerializer(this, HasCurrentPointer ? CurrentPointer : null, null, 0));
    }

    public override void Log(string logString, params object[] args)
    {
        //EditorGUI.LabelField(Window.GetNextRect(ref Window.YPos), $"{logString}");
    }

    public override T[] SerializeArrayUntil<T>(T[] obj, Func<T, bool> conditionCheckFunc, Func<T> getLastObjFunc = null, string name = null) {

        T[] array = obj;

        return SerializeArray<T>(array, array.Length, name: name);
    }

    public override T?[] SerializeNullableArrayUntil<T>(T?[] obj, Func<T?, bool> conditionCheckFunc, Func<T?> getLastObjFunc = null,
        string name = null)
    {
        T?[] array = obj;

        return SerializeNullableArray<T>(array, array.Length, name: name);
    }

    public override T[] SerializeObjectArrayUntil<T>(T[] obj, Func<T, bool> conditionCheckFunc, Func<T> getLastObjFunc = null, Action<T, int> onPreSerialize = null, string name = null) {
        T[] array = obj;

        return SerializeObjectArray<T>(array, array.Length, onPreSerialize: onPreSerialize, name: name);
    }

    public override Pointer[] SerializePointerArrayUntil(
        Pointer[] obj,
        Func<Pointer, bool> conditionCheckFunc,
        Func<Pointer> getLastObjFunc = null,
        PointerSize size = PointerSize.Pointer32,
        Pointer anchor = null,
        bool allowInvalid = false,
        long? nullValue = null,
        string name = null)
    {
        return SerializePointerArray(obj, obj.Length, size: size, anchor: anchor, allowInvalid: allowInvalid, nullValue: nullValue, name: name);
    }
}