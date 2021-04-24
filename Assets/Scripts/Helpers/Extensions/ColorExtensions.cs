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

            var cacheDictionary = context.GetStoredObject<Dictionary<Type, Dictionary<uint, Color>>>(key) ?? context.StoreObject(key, new Dictionary<Type, Dictionary<uint, Color>>());

            var type = c.GetType();

            if (!cacheDictionary.ContainsKey(type))
                cacheDictionary.Add(type, new Dictionary<uint, Color>());

            var typeDictionary = cacheDictionary[type];

            if (!typeDictionary.ContainsKey(c.ColorValue))
                typeDictionary.Add(c.ColorValue, new Color(c.Red, c.Green, c.Blue, c.Alpha));

            return typeDictionary[c.ColorValue];
        }

        public static CustomColor GetColor(this Color c)
        {
            return new CustomColor(c.r, c.g, c.b, c.a);
        }
    }
}