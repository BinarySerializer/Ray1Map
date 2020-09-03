using Newtonsoft.Json;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace R1Engine {
    [JsonConverter(typeof(PointerJsonConverter))]
    public class Pointer : IEquatable<Pointer>, IComparable<Pointer> {
        public uint AbsoluteOffset { get; }
        public Context Context { get; }
        public Pointer Anchor { get; private set; }
        public BinaryFile file;
        public Pointer(uint offset, BinaryFile file, Pointer anchor = null) {
            if (anchor != null) {
                this.Anchor = anchor;
                offset = anchor.AbsoluteOffset + offset;
            }
            this.AbsoluteOffset = offset;
            this.file = file;
            this.Context = file.Context;
            if (Context != null && !(file is ProcessMemoryStreamFile)) {
                Context.MemoryMap.AddPointer(this);
            }
        }

        public uint FileOffset {
            get {
                if (file != null) {
                    return (uint)(AbsoluteOffset - file.baseAddress);
                } else return AbsoluteOffset;
            }
        }

        public uint SerializedOffset {
            get {
                uint off = AbsoluteOffset;
                if (Anchor != null) {
                    off -= Anchor.AbsoluteOffset;
                }
                return off;
            }
        }

        public string StringFileOffset {
            get {
                return String.Format("{0:X8}", FileOffset);
            }
        }

        public string StringAbsoluteOffset {
            get {
                return String.Format("{0:X8}", AbsoluteOffset);
            }
        }

        public Pointer SetAnchor(Pointer anchor) {
            Pointer ptr = new Pointer(AbsoluteOffset, file);
            ptr.Anchor = anchor;
            return ptr;
        }

        public override bool Equals(System.Object obj) {
            return obj is Pointer && this == (Pointer)obj;
        }
        public override int GetHashCode() {
            return AbsoluteOffset.GetHashCode() ^ file.GetHashCode(); // ^ (Anchor?.GetHashCode() ?? 0);
        }

        public bool Equals(Pointer other) {
            return this == other;
        }

        public static bool operator ==(Pointer x, Pointer y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.AbsoluteOffset == y.AbsoluteOffset && x.file == y.file; // && x.Anchor == y.Anchor;
        }
        public static bool operator !=(Pointer x, Pointer y) {
            return !(x == y);
        }
        public static Pointer operator +(Pointer x, long y) {
            return new Pointer((uint)(x.AbsoluteOffset + y), x.file) { Anchor = x.Anchor };
        }
        public static Pointer operator -(Pointer x, long y) {
            return new Pointer((uint)(x.AbsoluteOffset - y), x.file) { Anchor = x.Anchor };
        }
        public static ulong operator +(Pointer x, Pointer y) {
            return x.AbsoluteOffset + y.AbsoluteOffset;
        }
        public static long operator -(Pointer x, Pointer y) {
            return x.AbsoluteOffset - y.AbsoluteOffset;
        }
        public override string ToString() {
            if (file != null && file.baseAddress != 0) {
                return file.filePath + "|" + String.Format("0x{0:X8}", AbsoluteOffset) + "[" + String.Format("0x{0:X8}", FileOffset) + "]";
            } else {
                return file.filePath + "|" + String.Format("0x{0:X8}", AbsoluteOffset);
            }
        }

        public int CompareTo(Pointer other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return AbsoluteOffset.CompareTo(other.AbsoluteOffset);
        }
    }

    public class Pointer<T> where T : R1Serializable, new() {
        public Context Context { get; }
        public Pointer pointer;
        public T Value { get; set; }

        public Pointer(Pointer pointer, bool resolve = false, SerializerObject s = null, Action<T> onPreSerialize = null) {
            Context = pointer?.Context;
            this.pointer = pointer;
            if (resolve) {
                Resolve(s, onPreSerialize: onPreSerialize);
            }
        }

        public Pointer(SerializerObject s, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, bool allowInvalid = false) {
            this.pointer = s.SerializePointer(this.pointer, anchor: anchor, allowInvalid: allowInvalid, name: "Pointer");
            if (resolve) {
                Resolve(s, onPreSerialize: onPreSerialize);
            }
        }

        public Pointer(Pointer pointer, T value) {
            this.pointer = pointer;
            this.Value = value;
        }
        public Pointer() {
            this.pointer = null;
            this.Value = null;
        }

        public Pointer<T> Resolve(SerializerObject s, Action<T> onPreSerialize = null) {
            if (pointer != null) {
                Value = pointer.Context.Cache.FromOffset<T>(pointer);
                s.DoAt(pointer, () => {
                    Value = s.SerializeObject<T>(Value, onPreSerialize: onPreSerialize, name: "Value");
                });
            }
            return this;
        }
        public Pointer<T> Resolve(Context c) {
            if (pointer != null) {
                Value = c.Cache.FromOffset<T>(pointer);
            }
            return this;
        }

        public static implicit operator T(Pointer<T> a) {
            return a.Value;
        }
        public static implicit operator Pointer<T>(T t) {
            if (t == null) {
                return new Pointer<T>(null, null);
            } else {
                return new Pointer<T>(t.Offset, t);
            }
        }
    }
}
