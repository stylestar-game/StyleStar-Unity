using StyleStar;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class SongSelection
{
    public static List<SongMetadata> Songlist { get; set; } = new List<SongMetadata>();
    public static List<FolderParams> FolderParams { get; set; } = new List<FolderParams>();

    public static void ImportSongs(string songsFolder)
    {
        if (!Directory.Exists(songsFolder))
            return;
        //DirectoryInfo di = new DirectoryInfo(songsFolder);
        //var folders = di.EnumerateDirectories();
        var folders = Directory.EnumerateDirectories(songsFolder);
        foreach (var folder in folders)
        {
            // If a folder contains an *.ssi file, use that to load charts
            // Otherwise, load each chart individually
            //var files = folder.EnumerateFiles();
            var files = Directory.EnumerateFiles(folder);
            var info = files.Where(f => f.EndsWith(Defines.InfoExtension));
            if (info != null && info.Count() > 0)
            {
                foreach (var file in info)
                {
                    Songlist.Add(new SongMetadata(file));
                }
            }
            else
            {
                var charts = files.Where(f => f.EndsWith(Defines.ChartExtension));
                if (charts != null && charts.Count() > 0)
                {
                    foreach (var chart in charts)
                        Songlist.Add(new SongMetadata(chart));
                }
            }
        }
        FolderParams.Add(new FolderParams() { Type = SortType.Title, Name = "SORT BY\nTITLE" });
        FolderParams.Add(new FolderParams() { Type = SortType.Artist, Name = "SORT BY\nARTIST" });
        FolderParams.Add(new FolderParams() { Type = SortType.Level, Name = "SORT BY\nLEVEL" });
    }
}

public class FolderParams
{
    public SortType Type;
    public int Value;
    public string Category;
    public string Name;
}

public enum SortType
{
    Title,
    Artist,
    Level,
    Genre
}
