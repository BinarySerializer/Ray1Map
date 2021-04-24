using System;
using System.Collections.Generic;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public static class ColorExtensions
    {
        public static Color GetColor(this BaseColor c)
        {
            var context = c.Context;

            if (context == null)
                return new Color(c.Red, c.Green, c.Blue, c.Alpha);

            const string key = "CachedColors";

            var cacheDictionary = context.GetStoredObject<Dictionary<Type, Dictionary<int, Color>>>(key) ?? context.StoreObject(key, new Dictionary<Type, Dictionary<int, Color>>());

            var type = c.GetType();

            if (!cacheDictionary.ContainsKey(type))
                cacheDictionary.Add(type, new Dictionary<int, Color>());

            var typeDictionary = cacheDictionary[type];

            var hash = c.GetHashCode();

            if (!typeDictionary.ContainsKey(hash))
                typeDictionary.Add(hash, new Color(c.Red, c.Green, c.Blue, c.Alpha));

            return typeDictionary[hash];
        }

        public static CustomColor GetColor(this Color c)
        {
            return new CustomColor(c.r, c.g, c.b, c.a);
        }
    }
}