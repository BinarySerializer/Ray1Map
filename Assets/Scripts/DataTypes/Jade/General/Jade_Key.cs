using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_Key : BinarySerializable, IEquatable<Jade_Key> {
		public uint Key { get; set; }
		public bool IsNull => Key == 0 || Key == 0xFFFFFFFF;
		public override bool IsShortLog => true;
		public override string ShortLog => ToString();

		public override void SerializeImpl(SerializerObject s) {
			Key = s.Serialize<uint>(Key, name: nameof(Key));
		}

		public Jade_Key() { }
		public Jade_Key(Context context, uint k) {
			Context = context;
			Key = k;
		}
		public static implicit operator uint(Jade_Key k) => k.Key;
		public override string ToString() {
			return Key.ToString("X8");
		}

		public bool Equals(Jade_Key other) => this == (Jade_Key)other;

		public override int GetHashCode() {
			return Key.GetHashCode();
		}
		public override bool Equals(object obj) {
			return obj is Jade_Key && this == (Jade_Key)obj;
		}
		public static bool operator ==(Jade_Key x, Jade_Key y) {
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			return x.Key == y.Key;
		}
		public static bool operator !=(Jade_Key x, Jade_Key y) {
			return !(x == y);
		}

		public const uint KeyTypeMap = 0xFF000000;
		public const uint KeyTypeSounds = 0xFF400000;
		public const uint KeyTypeTextures = 0xFF800000;
		public const uint KeyTypeTextNoSound = 0xFD000000;
		public const uint KeyTypeTextSound = 0xFE000000;
		public static uint WorldKey(Context context, uint key) {
			if (context.GetR1Settings().Jade_Version >= Jade_Version.Montreal) {
				return (uint)BitHelpers.ExtractBits((int)key, 20, 0);
			} else {
				return (uint)BitHelpers.ExtractBits((int)key, 19, 0);
			}
		}
		public static uint ComposeMontrealKey(uint key) {
			uint newKey = (uint)BitHelpers.ExtractBits((int)key, 16, 0);
			uint topPart = (uint)BitHelpers.ExtractBits((int)key, 8, 24);
			newKey = (uint)BitHelpers.SetBits((int)newKey, (int)topPart, 8, 16);
			return newKey;
		}
		public static uint UncomposeBinKey(Context context, uint key) {
			uint newKey = WorldKey(context, key);
			if (context.GetR1Settings().Jade_Version >= Jade_Version.Montreal) {
				uint bottom = (uint)BitHelpers.ExtractBits((int)newKey, 16, 0);
				uint top = (uint)BitHelpers.ExtractBits((int)newKey, 8, 16);
				newKey = (uint)BitHelpers.SetBits((int)bottom, (int)top, 8, 24);
			}
			return newKey;
		}
		public static Jade_Key GetBinaryForKey(Context context, uint worldKey, KeyType type, int languageID = 0) {
			uint newKey = worldKey;
			if (context.GetR1Settings().Jade_Version >= Jade_Version.Montreal) {
				newKey = ComposeMontrealKey(newKey);
			}
			newKey = WorldKey(context, newKey);
			switch (type) {
				case KeyType.Map: newKey |= KeyTypeMap; break;
				case KeyType.Sounds: newKey |= KeyTypeSounds; break;
				case KeyType.Textures: newKey |= KeyTypeTextures; break;
				case KeyType.TextNoSound: newKey |= KeyTypeTextNoSound; break;
				case KeyType.TextSound: newKey |= KeyTypeTextSound; break;
			}
			if (type == KeyType.TextSound || type == KeyType.TextNoSound) {
				if (context.GetR1Settings().Jade_Version >= Jade_Version.Montreal) {
					newKey = (uint)BitHelpers.SetBits((int)newKey, languageID + 1, 4, 20);
				} else {
					newKey = (uint)BitHelpers.SetBits((int)newKey, languageID + 1, 5, 19);
				}
			}
			return new Jade_Key(context, newKey);
		}
		public Jade_Key GetBinary(KeyType type, int languageID = 0) {
			return GetBinaryForKey(Context, this, type, languageID: languageID);
		}
		public KeyType Type {
			get {
				switch (Key & 0xFF000000) {
					case KeyTypeMap:
						uint mask = 0xFFF80000;
						if(Context.GetR1Settings().Jade_Version >= Jade_Version.Montreal) mask = 0xFFF00000;
						switch (Key & mask) {
							case KeyTypeTextures: return KeyType.Textures;
							case KeyTypeSounds: return KeyType.Sounds;
							case KeyTypeMap: return KeyType.Map;
							default: return KeyType.Unknown;
						}
					case KeyTypeTextNoSound: return KeyType.TextNoSound;
					case KeyTypeTextSound: return KeyType.TextSound;
					default: return KeyType.Unknown;
				}
			}
		}
		public bool IsCompressed {
			get {
				switch (Type) {
					case KeyType.Map:
					case KeyType.Textures:
					case KeyType.TextNoSound:
					case KeyType.TextSound:
						return true;
					default:
						return false;
				}
			}
		}

		public enum KeyType {
			Unknown,
			Map,
			Textures,
			Sounds,
			TextNoSound,
			TextSound
		}
	}
}
