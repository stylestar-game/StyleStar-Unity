using System;
using System.Linq;
using UnityEngine;

namespace StyleStar
{
    public static class Util
    {
        static readonly bool _drawBoundingBox = false;

        ///// This snippit modified from: http://bluelinegamestudios.com/posts/drawstring-to-fit-text-to-a-rectangle-in-xna/

        ///// Draws the given string as large as possible inside the boundaries Rectangle without going
        ///// outside of it.  This is accomplished by scaling the string (since the SpriteFont has a specific
        ///// size).
        ///// 
        ///// If the string is not a perfect match inside of the boundaries (which it would rarely be), then
        ///// the string will be absolutely-centered inside of the boundaries.
        //static public void DrawString(this SpriteBatch spriteBatch, SpriteFont font, string strToDraw, Rectangle boundaries, Color color, Justification just = Justification.Center)
        //{
        //    Vector2 size = MeasureString(font, strToDraw);

        //    float xScale = (boundaries.Width / size.X);
        //    float yScale = (boundaries.Height / size.Y);

        //    // Taking the smaller scaling value will result in the text always fitting in the boundaires.
        //    float scale = Math.Min(xScale, yScale);

        //    // Figure out the location to absolutely-center it in the boundaries rectangle.
        //    int strWidth = (int)Math.Round(size.X * scale);
        //    int strHeight = (int)Math.Round(size.Y * scale);
        //    Vector2 position = new Vector2();
        //    switch (just)
        //    {
        //        case Justification.Left:
        //            position.X = boundaries.X;
        //            position.Y = boundaries.Y;
        //            break;
        //        case Justification.Right:
        //            break;
        //        case Justification.Center:
        //            position.X = (((boundaries.Width - strWidth) / 2) + boundaries.X);
        //            position.Y = (((boundaries.Height - strHeight) / 2) + boundaries.Y);
        //            break;
        //        default:
        //            break;
        //    }

        //    // Draw the string to the sprite batch!
        //    if (_drawBoundingBox)
        //        spriteBatch.Draw(Globals.Textures["BeatMark"], new Rectangle((int)boundaries.X, (int)boundaries.Y, boundaries.Width, boundaries.Height), Color.Red);
        //    spriteBatch.DrawStringAbs(font, strToDraw, position, color, scale);
        //} // end DrawString()

        //public static Vector2 GetStringFromBoundingBox(SpriteFont font, string text, Rectangle boundaries, Justification just, out float scale, out Rectangle boundingBox)
        //{
        //    Vector2 size = MeasureString(font, text);

        //    float xScale = (boundaries.Width / size.X);
        //    float yScale = (boundaries.Height / size.Y);

        //    // Taking the smaller scaling value will result in the text always fitting in the boundaires.
        //    scale = Math.Min(xScale, yScale);

        //    // Figure out the location to absolutely-center it in the boundaries rectangle.
        //    int strWidth = (int)Math.Round(size.X * scale);
        //    int strHeight = (int)Math.Round(size.Y * scale);
        //    boundingBox = new Rectangle(0, 0, strWidth, strHeight);
        //    Vector2 position = new Vector2();
        //    if (just.HasFlag(Justification.Left))
        //    {
        //        position.X = boundaries.X;
        //    }
        //    else if (just.HasFlag(Justification.Center))
        //    {
        //        position.X = (((boundaries.Width - strWidth) / 2) + boundaries.X);
        //    }
        //    else if (just.HasFlag(Justification.Right))
        //    {
        //        position.X = boundaries.X + boundaries.Width - strWidth;
        //    }
        //    if (just.HasFlag(Justification.Top))
        //    {
        //        position.Y = boundaries.Y;
        //    }
        //    else if (just.HasFlag(Justification.Middle))
        //    {
        //        position.Y = (((boundaries.Height - strHeight) / 2) + boundaries.Y);
        //    }
        //    else if (just.HasFlag(Justification.Bottom))
        //    {
        //        position.Y = boundaries.Y + boundaries.Height - strHeight;
        //    }

        //    boundingBox.X = (int)position.X;
        //    boundingBox.Y = (int)position.Y;

        //    int newLines = text.Count(c => c == '\n');
        //    float yDiff = (font.LineSpacing * (newLines + 1)) - size.Y;
        //    position.Y -= yDiff * scale;

        //    return position;
        //}

        //public static float GetTextOffset(SpriteFont font, string text)
        //{
        //    Vector2 size = MeasureString(font, text);
        //    int newLines = text.Count(c => c == '\n');
        //    return (font.LineSpacing * (newLines + 1)) - size.Y;
        //}

        //public static void DrawStringFixedHeight(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color color, float maxFontHeight, Justification justification = Justification.Middle, float strokeWidth = 0.0f, Color? strokeColor = null)
        //{
        //    Vector2 size = MeasureString(font, text);
        //    float yScale = (maxFontHeight / size.Y);
        //    int strWidth = (int)Math.Round(size.X * yScale);
        //    int strHeight = (int)Math.Round(size.Y * yScale);

        //    float xOffset = 0, yOffset = 0;
        //    if (justification.HasFlag(Justification.Right))
        //    {
        //        xOffset += -strWidth;
        //    }
        //    else if (justification.HasFlag(Justification.Center))
        //    {
        //        xOffset += -strWidth / 2;
        //    }
        //    if (!justification.HasFlag(Justification.Bottom) && !justification.HasFlag(Justification.Middle))
        //    {
        //        //yOffset += 0;
        //    }
        //    else if (justification.HasFlag(Justification.Bottom))
        //    {
        //        yOffset += -strHeight;
        //    }
        //    else if (justification.HasFlag(Justification.Middle))
        //    {
        //        yOffset += -strHeight / 2;
        //    }
        //    Vector2 offset = new Vector2(xOffset, yOffset);

        //    if (_drawBoundingBox)
        //        sb.Draw(Globals.Textures["BeatMark"], new Rectangle((int)(position.X + offset.X), (int)(position.Y + offset.Y), strWidth, strHeight), Color.Red);

        //    if (strokeWidth > 0.0f)
        //    {
        //        if (strokeColor == null)
        //            strokeColor = Color.Black;
        //        DrawStringStroke(sb, font, text, position + offset, strokeWidth, (Color)strokeColor, yScale, StrokeStyle.All);
        //    }
        //    sb.DrawStringAbs(font, text, position + offset, color, yScale);

        //}

        //public static Vector2 GetStringFixedHeight(SpriteFont font, string text, Vector2 position, float maxFontHeight, Justification justification, out float scale, out Rectangle boundingBox)
        //{
        //    Vector2 size = MeasureString(font, text);
        //    scale = (maxFontHeight / size.Y);
        //    int strWidth = (int)Math.Round(size.X * scale);
        //    int strHeight = (int)Math.Round(size.Y * scale);

        //    float xOffset = 0, yOffset = 0;
        //    if (justification.HasFlag(Justification.Right))
        //    {
        //        xOffset += -strWidth;
        //    }
        //    else if (justification.HasFlag(Justification.Center))
        //    {
        //        xOffset += -strWidth / 2;
        //    }
        //    if (!justification.HasFlag(Justification.Bottom) && !justification.HasFlag(Justification.Middle))
        //    {
        //        //yOffset += 0;
        //    }
        //    else if (justification.HasFlag(Justification.Bottom))
        //    {
        //        yOffset += -strHeight;
        //    }
        //    else if (justification.HasFlag(Justification.Middle))
        //    {
        //        yOffset += -strHeight / 2;
        //    }

        //    boundingBox = new Rectangle((int)(position.X + xOffset), (int)(position.Y + yOffset), strWidth, strHeight);

        //    int newLines = text.Count(c => c == '\n');
        //    float yDiff = (font.LineSpacing * (newLines + 1)) - size.Y;
        //    position.Y -= yDiff * scale;

        //    return position + new Vector2(xOffset, yOffset);
        //}

        //public static void DrawStringAbs(this SpriteBatch sb, SpriteFont font, string strToDraw, Vector2 position, Color color)
        //{
        //    Vector2 size = MeasureString(font, strToDraw);
        //    float yDiff = font.LineSpacing - size.Y;
        //    position.Y -= yDiff;

        //    sb.DrawString(font, strToDraw, position, color);
        //}

        //public static void DrawStringAbs(this SpriteBatch sb, SpriteFont font, string strToDraw, Vector2 position, Color color, float scale)
        //{
        //    Vector2 size = MeasureString(font, strToDraw);
        //    int newLines = strToDraw.Count(c => c == '\n');
        //    float yDiff = (font.LineSpacing * (newLines + 1)) - size.Y;
        //    position.Y -= yDiff * scale;

        //    try
        //    {
        //        sb.DrawString(font, strToDraw, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.WriteEntry("Error trying to draw string: " + e.Message + " @ " + e.StackTrace);
        //    }
        //}

        //public static Vector2 MeasureString(SpriteFont font, string text)
        //{
        //    if (text.Length == 0)
        //        return Vector2.Zero;

        //    var width = 0.0f;
        //    var finalLineHeight = 0.0f;

        //    var offset = Vector2.Zero;
        //    bool firstGlyphOfLine = true;

        //    var glyphs = font.GetGlyphs();

        //    for (int i = 0; i < text.Length; i++)
        //    {
        //        var c = text[i];

        //        if (c == '\r')
        //            continue;

        //        if (c == '\n')
        //        {
        //            offset.X = 0;
        //            offset.Y += font.LineSpacing;
        //            firstGlyphOfLine = true;
        //            continue;
        //        }

        //        var currentGlyph = glyphs.ContainsKey(c) ? glyphs[c] : new SpriteFont.Glyph();
        //        if (currentGlyph.WidthIncludingBearings == 0.0f)
        //            continue;

        //        if (firstGlyphOfLine)
        //        {
        //            offset.X = Math.Max(currentGlyph.LeftSideBearing, 0);
        //            firstGlyphOfLine = false;
        //        }
        //        else
        //        {
        //            offset.X += font.Spacing + currentGlyph.LeftSideBearing;
        //        }

        //        offset.X += currentGlyph.Width;

        //        var proposedWidth = offset.X + Math.Max(currentGlyph.RightSideBearing, 0);
        //        if (proposedWidth > width)
        //            width = proposedWidth;

        //        offset.X += currentGlyph.RightSideBearing;

        //        if (currentGlyph.Cropping.Height > finalLineHeight)
        //            finalLineHeight = currentGlyph.Cropping.Height;
        //    }

        //    return new Vector2(width, offset.Y + finalLineHeight);
        //}

        //public static Rectangle Shift(this Rectangle rect, int x, int y)
        //{
        //    return new Rectangle(rect.X + x, rect.Y + y, rect.Width, rect.Height);
        //}

        //public static void DrawStringJustify(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color color, float scale, Justification justification)
        //{
        //    DrawStringJustify(sb, font, text, position, color, scale, justification, 0, new Color());
        //}

        //public static void DrawStringJustify(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color color, float scale, Justification justification, float strokeSize, Color strokeColor)
        //{
        //    //Vector2 size = font.MeasureString(text);
        //    Vector2 size = MeasureString(font, text);
        //    float trueY = size.Y * scale;
        //    float xOffset = 0, yOffset = 0;
        //    if (justification.HasFlag(Justification.Right))
        //    {
        //        xOffset += -size.X * scale;
        //    }
        //    else if (justification.HasFlag(Justification.Center))
        //    {
        //        xOffset += -size.X * scale / 2;
        //    }
        //    if (!justification.HasFlag(Justification.Bottom) && !justification.HasFlag(Justification.Middle))
        //    {
        //        yOffset += -trueY;
        //    }
        //    else if (justification.HasFlag(Justification.Bottom))
        //    {
        //        yOffset += -size.Y * scale + trueY;
        //    }
        //    else if (justification.HasFlag(Justification.Middle))
        //    {
        //        yOffset += -trueY / 2;
        //    }
        //    Vector2 offset = new Vector2(xOffset, yOffset);
        //    if (strokeSize > 0)
        //    {
        //        DrawStringStroke(sb, font, text, position + offset, strokeSize, strokeColor, scale, StrokeStyle.All);
        //    }
        //    sb.DrawStringAbs(font, text, position + offset, color, scale);
        //}

        //public static void DrawStringStroke(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, float strokeSize, Color color, float scale)
        //{
        //    DrawStringStroke(sb, font, text, position, strokeSize, color, scale, StrokeStyle.Corners);
        //}

        //public static void DrawStringStroke(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, float strokeSize, Color color, float scale, StrokeStyle style)
        //{
        //    sb.DrawStringAbs(font, text, new Vector2(position.X - strokeSize, position.Y - strokeSize), color, scale);
        //    sb.DrawStringAbs(font, text, new Vector2(position.X + strokeSize, position.Y - strokeSize), color, scale);
        //    sb.DrawStringAbs(font, text, new Vector2(position.X - strokeSize, position.Y + strokeSize), color, scale);
        //    sb.DrawStringAbs(font, text, new Vector2(position.X + strokeSize, position.Y + strokeSize), color, scale);
        //    if (style == StrokeStyle.All)
        //    {
        //        sb.DrawStringAbs(font, text, new Vector2(position.X - strokeSize, position.Y), color, scale);
        //        sb.DrawStringAbs(font, text, new Vector2(position.X + strokeSize, position.Y), color, scale);
        //        sb.DrawStringAbs(font, text, new Vector2(position.X, position.Y + strokeSize), color, scale);
        //        sb.DrawStringAbs(font, text, new Vector2(position.X, position.Y - strokeSize), color, scale);
        //    }
        //}

        //public static Color LerpBlackAlpha(this Color color, float ratio, float alpha)
        //{
        //    var tempColor = Color.Lerp(Color.Black, Color.Transparent, alpha);
        //    return Color.Lerp(color, tempColor, ratio);
        //}

        public static Color ParseFromHex(string input)
        {
            int r, g, b;
            if (input.StartsWith("#"))
                input = input.Remove(0, 1);
            r = int.Parse(input.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            g = int.Parse(input.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            b = int.Parse(input.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color32((byte)r, (byte)g, (byte)b, 255);
        }

        //public static Color IfNull(this Color c, Color other)
        //{
        //    if (c == ThemeColors.NullColor)
        //        return other;
        //    else
        //        return c;
        //}

        // Parsing Functions

        private const string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";

        public static int ParseBase36(this string s)
        {
            int result = 0;
            int pos = 0;
            var reversed = s.ToLower().Reverse();
            foreach (var c in reversed)
            {
                result += CharList.IndexOf(c) * (int)Math.Pow(36, pos);
                pos++;
            }
            return result;
        }

    }

    [Flags]
    public enum Justification
    {
        Left = 0x01,
        Right = 0x02,
        Center = 0x04,
        Top = 0x08,
        Bottom = 0x10,
        Middle = 0x20
    }

    public enum StrokeStyle
    {
        Corners,
        All
    }
}
