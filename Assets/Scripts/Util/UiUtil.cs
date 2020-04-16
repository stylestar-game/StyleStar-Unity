using StyleStar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static partial class Util
{
    public static Color Transparent = new Color(0, 0, 0, 0);

    public static void SetColor(this GameObject obj, Color color)
    {
        if(obj.GetComponent<Image>() != null)
            obj.GetComponent<Image>().color = new Color(color.r, color.g, color.b, color.a);
        else if (obj.GetComponent<SpriteRenderer>() != null)
            obj.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, color.a);
    }

    public static void SetText(this GameObject obj, string text)
    {
        obj.GetComponent<TextMeshProUGUI>().text = text;
    }

    public static Color LerpBlackAlpha(this Color color, float ratio, float alpha)
    {
        var tempColor = Color.Lerp(Color.black, Transparent, alpha);
        return Color.Lerp(color, tempColor, ratio);
    }

    public static Color IfNull(this Color c, Color other)
    {
        if (c == ThemeColors.NullColor)
            return other;
        else
            return c;
    }

    public static void SetActive(this List<CardBase> list, bool active)
    {
        if (list.Count > 0)
        {
            if (list[0].IsActive == active)
                return;
        }

        for (int i = 0; i < list.Count; i++)
            list[i].SetActive(active);
    }

    public static Rect GetScreenRect()
    {
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        return canvas ? canvas.pixelRect : new Rect();
    }
}
