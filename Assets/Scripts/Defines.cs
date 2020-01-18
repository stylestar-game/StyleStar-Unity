namespace StyleStar
{
    public static class Defines
    {
        public static readonly string ChartExtension = ".ssf";
        public static readonly string InfoExtension = ".ssi";
        public static readonly string ZipExtension = ".ssz";

        public static readonly string ConfigFile = "config.toml";
        public static readonly string KeyConfig = "KeyConfig";
        public static readonly string TouchConfig = "TouchConfig";
        public static readonly string GameConfig = "GameConfig";

        public static readonly string UserFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\StyleStar\\";
        public static readonly string SongFolder = UserFolder + "Songs";
    }
}
