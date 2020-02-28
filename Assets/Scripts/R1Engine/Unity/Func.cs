using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

public static class Func
{
    public static void ApplyTexture(GameObject obj, string file, FilterMode filterMode = FilterMode.Bilinear)
    {
        var f = new FileStream(file, FileMode.Open);
        var bytes = new byte[f.Length];
        f.Read(bytes, 0, bytes.Length);
        f.Close();
        var t = new Texture2D(2, 2);
        t.LoadImage(bytes);
        t.filterMode = filterMode;
        var r = obj.GetComponent<Renderer>();
        r.material.mainTexture = t;
        r.material.shader = Shader.Find("Sprites/Default");
    }

    public static void ApplyTexture(GameObject obj, Texture2D texture, FilterMode filterMode = FilterMode.Bilinear)
    {
        texture.filterMode = filterMode;
        var r = obj.GetComponent<Renderer>();
        r.material.mainTexture = texture;
        r.material.shader = Shader.Find("Sprites/Default");
    }



    public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
            {
                yield return i;
            }
        }
    }
}
