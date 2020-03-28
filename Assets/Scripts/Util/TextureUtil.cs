using System.IO;
using UnityEngine;

public static class TextureUtil
{
    public static Texture2D LoadPngAsTexture(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    public static Sprite LoadPngAsSprite(string filePath)
    {
        var tex = LoadPngAsTexture(filePath);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}
