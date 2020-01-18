using System;
using System.IO;

namespace StyleStar
{
    public static class StyleStarLogger
    {
        public static string LogFilename;

        public static void WriteEntry(string entry)
        {
            if (LogFilename == null)
                LogFilename = "StyleStarLog-" + GetDateTimeString() + ".txt";

            using (StreamWriter sw = new StreamWriter(new FileStream(LogFilename, FileMode.Append)))
            {
                sw.WriteLine(GetDateTimeString() + ": " + entry);
            }
        }

        public static string GetDateTimeString()
        {
            return DateTime.Now.ToString("yyyyMMdd HHmm");
        }
    }
}
