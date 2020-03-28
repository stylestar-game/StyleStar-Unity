using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;


namespace StyleStar
{
    public class SongMetadata
    {
        public string FilePath { get; set; }
        public string ChartFullPath { get; set; }
        public bool IsMetadataFile { get; set; }
        public List<SongMetadata> ChildMetadata { get; private set; } = new List<SongMetadata>();
        public string SongFilename { get; set; }
        public Dictionary<int, double> BpmIndex = new Dictionary<int, double>();
        public List<BpmChangeEvent> BpmEvents = new List<BpmChangeEvent>();
        public double PlaybackOffset { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Designer { get; set; }
        public Sprite AlbumImage { get; set; }
        public string Jacket { get; set; }
        public Sprite TitleImage { get; set; }
        public string TitleCard { get; set; }
        public Sprite ArtistImage { get; set; }
        public string ArtistCard { get; set; }
        public Difficulty Difficulty { get; set; }
        public int Level { get; set; }
        public Color ColorFore { get; set; } = ThemeColors.NullColor;
        public Color ColorBack { get; set; } = ThemeColors.NullColor;
        public Color ColorAccent { get; set; } = ThemeColors.NullColor;
        public Version Version { get; set; } = new Version("0.9");

        public string SongID { get; set; }  // This is used internally for tracking the song

        public SongMetadata() { }

        public SongMetadata(string fileName)
        {
            Parse(fileName);
        }

        public SongMetadata(SongMetadata parent, string fileName)
        {
            SongFilename = parent.SongFilename;
            PlaybackOffset = parent.PlaybackOffset;
            Title = parent.Title;
            Artist = parent.Artist;
            Designer = parent.Designer;
            ColorFore = parent.ColorFore;
            ColorBack = parent.ColorBack;
            ColorAccent = parent.ColorAccent;
            BpmEvents.AddRange(parent.BpmEvents);

            Parse(fileName);
        }

        private void Parse(string fileName)
        {
            try
            {
                ChartFullPath = Path.GetFullPath(fileName);
                FilePath = Path.GetDirectoryName(fileName) + @"\";
                List<string> children = new List<string>();
                using (StreamReader sr = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        string parse;
                        if (StringExtensions.TrySearchTag(line, "VERSION", out parse))
                            Version = Version.Parse(parse);
                        if (StringExtensions.TrySearchTag(line, "WAVE", out parse))
                            SongFilename = parse;
                        if (StringExtensions.TrySearchTag(line, "WAVEOFFSET", out parse))
                            PlaybackOffset = Convert.ToDouble(parse);
                        if (StringExtensions.TrySearchTag(line, "TITLE", out parse))
                            Title = parse;
                        if (StringExtensions.TrySearchTag(line, "ARTIST", out parse))
                            Artist = parse;
                        if (StringExtensions.TrySearchTag(line, "DESIGNER", out parse))
                            Designer = parse;
                        if (StringExtensions.TrySearchTag(line, "DIFFICULTY", out parse))
                            Difficulty = (Difficulty)Convert.ToInt32(parse);
                        if (StringExtensions.TrySearchTag(line, "PLAYLEVEL", out parse))
                            Level = Convert.ToInt32(parse);
                        if (StringExtensions.TrySearchTag(line, "JACKET", out parse))
                            Jacket = parse;
                        if (StringExtensions.TrySearchTag(line, "TITLECARD", out parse))
                            TitleCard = parse;
                        if (StringExtensions.TrySearchTag(line, "ARTISTCARD", out parse))
                            ArtistCard = parse;
                        if (StringExtensions.TrySearchTag(line, "COLORFORE", out parse))
                            ColorFore = Util.ParseFromHex(parse);
                        if (StringExtensions.TrySearchTag(line, "COLORBACK", out parse))
                            ColorBack = Util.ParseFromHex(parse);
                        if (StringExtensions.TrySearchTag(line, "COLORACCENT", out parse))
                            ColorAccent = Util.ParseFromHex(parse);
                        if (StringExtensions.TrySearchTag(line, "SONGID", out parse))
                            SongID = parse;

                        if (Regex.IsMatch(line, "(#BPM)"))
                        {
                            string[] bpmParse = line.Split(new string[] { "#BPM", ": " }, StringSplitOptions.RemoveEmptyEntries);
                            if (!BpmIndex.ContainsKey(bpmParse[0].ParseBase36()))
                                BpmIndex.Add(bpmParse[0].ParseBase36(), Convert.ToDouble(bpmParse[1]));
                            else
                                // Print an error here in some log somewhere
                                Console.WriteLine("When parsing " + fileName + ", multiple BPM definitions were found with the same ID number.");
                        }

                        if (StringExtensions.TrySearchTag(line, "CHART", out parse))
                        {
                            IsMetadataFile = true;
                            if (!parse.EndsWith(Defines.ChartExtension))
                                parse += Defines.ChartExtension;
                            children.Add(FilePath + parse);
                        }
                    }
                }
                foreach (var child in children)
                    ChildMetadata.Add(new SongMetadata(this, child));

                if (String.IsNullOrEmpty(Jacket))
                //AlbumImage = Globals.Textures["FallbackJacket"]; // FIXME
                { }
                else
                {
                    AlbumImage = TextureUtil.LoadPngAsSprite(FilePath + Jacket);

                    //using (FileStream fs = new FileStream(FilePath + Jacket, FileMode.Open))
                    //{
                    //    AlbumImage = Texture2D.FromStream(Globals.GraphicsManager.GraphicsDevice, fs);
                    //}
                }
                if (!String.IsNullOrEmpty(TitleCard))
                {
                    if (File.Exists(FilePath + TitleCard))
                    {
                        TitleImage = TextureUtil.LoadPngAsSprite(FilePath + TitleCard);

                        //using (FileStream fs = new FileStream(FilePath + TitleCard, FileMode.Open))
                        //{
                        //    TitleImage = Texture2D.FromStream(Globals.GraphicsManager.GraphicsDevice, fs);
                        //}
                    }
                }
                if (!String.IsNullOrEmpty(ArtistCard))
                {
                    if (File.Exists(FilePath + ArtistCard))
                    {
                        ArtistImage = TextureUtil.LoadPngAsSprite(FilePath + ArtistCard);

                        //using (FileStream fs = new FileStream(FilePath + ArtistCard, FileMode.Open))
                        //{
                        //    ArtistImage = Texture2D.FromStream(Globals.GraphicsManager.GraphicsDevice, fs);
                        //}
                    }
                }
                if (String.IsNullOrEmpty(SongID))
                    SongID = Title + "_" + Artist + "_" + Level.ToString();
            }
            catch (Exception e)
            {
                StyleStarLogger.WriteEntry("Exception in SongMetadata.Parse() => Input: " + fileName + ", Exception: " + e.Message + ", Stack Trace: " + e.StackTrace + (e.InnerException != null ? ", Inner Exception: " + e.InnerException.Message : ""));
            }
        }

        public int GetLevel(int difficulty)
        {
            if (ChildMetadata.Count > 0)
            {
                var meta = ChildMetadata.Find(x => (int)x.Difficulty == difficulty);
                if (meta == null)
                    return 0;
                else
                    return meta.Level;
            }
            else
            {
                if ((int)Difficulty == difficulty)
                    return Level;
                else
                    return 0;
            }
        }

        public string GetPropertyFromChild(string propertyName, int index)
        {
            string ret = "";
            Type t = this.GetType();
            PropertyInfo pi = t.GetProperty(propertyName);

            if (ChildMetadata.Count > 0 && index < ChildMetadata.Count)
            {
                try
                {
                    ret = (string)pi.GetValue(ChildMetadata[index]);
                    return ret;
                }
                catch
                { }
            }

            try
            {
                ret = (string)pi.GetValue(this);
                return ret;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public bool IsAnyColorNull()
        {
            if (ColorFore == ThemeColors.NullColor ||
                ColorBack == ThemeColors.NullColor ||
                ColorAccent == ThemeColors.NullColor)
                return true;
            else
                return false;
        }
    }
}
