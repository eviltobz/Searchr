using Searchr.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Searchr.UI
{
    using System;

    public static class Config
    {
        //private const string SettingsFile = @"My.settings";
        private const string SettingsFile = @"searchr.settings";

        public static Settings Settings { get; set; } = LoadSettings(SettingsFile);

        public static string HistoryDirectory { get; } = FindHistoryDirectory();

        private static string FindHistoryDirectory()
        {
            return FindDirectory("History") ?? (Debugger.IsAttached ? @"..\..\History" : "History");
        }

        private static string? FindDirectory(string dir, int searchDepth = 0)
        {
            if (Directory.Exists(dir))
            {
                return dir;
            }
            else if (searchDepth == 2)
            {
                return null;
            }
            else
            {
                dir = Path.Combine("..", dir);
                return FindDirectory(dir, searchDepth + 1);
            }
        }

        public static List<string> CommonDirs
        {
            get
            {
                return EnumerateHistory()
                        .Select(s => s.Directory)
                        .GroupBy(s => s)
                        .ToDictionary(g => g.Key, g => g.Count())
                        .OrderByDescending(g => g.Value)
                        .Select(g => g.Key)
                        .ToList();
            }
        }

        public static List<string> CommonIncludedExtensions
        {
            get
            {
                return EnumerateHistory()
                        .Where(s => s.IncludeFileWildcards.Count > 0)
                        .Select(s => string.Join(",", s.IncludeFileWildcards.OrderBy(s2 => s2)))
                        .GroupBy(s => s)
                        .ToDictionary(g => g.Key, g => g.Count())
                        .OrderByDescending(g => g.Value)
                        .Select(g => g.Key)
                        .ToList();
            }
        }

        public static List<string> CommonExcludedExtensions
        {
            get
            {
                return EnumerateHistory()
                        .Where(s => s.ExcludeFileWildcards.Count > 0)
                        .Select(s => string.Join(",", s.ExcludeFileWildcards.OrderBy(s2 => s2)))
                        .GroupBy(s => s)
                        .ToDictionary(g => g.Key, g => g.Count())
                        .OrderByDescending(g => g.Value)
                        .Select(g => g.Key)
                        .ToList();
            }
        }

        public static List<string> CommonExcludedDirs
        {
            get
            {
                return EnumerateHistory()
                        .Where(s => s.ExcludeFolderNames.Count > 0)
                        .Select(s => string.Join(",", s.ExcludeFolderNames.OrderBy(s2 => s2)))
                        .GroupBy(s => s)
                        .ToDictionary(g => g.Key, g => g.Count())
                        .OrderByDescending(g => g.Value)
                        .Select(g => g.Key)
                        .ToList();
            }
        }

        public static SearchRequest? LatestSearch()
        {
            return EnumerateHistory().FirstOrDefault();
        }

        private static IEnumerable<SearchRequest> EnumerateHistory()
        {
            var dir = new DirectoryInfo(HistoryDirectory);

            if (!dir.Exists)
            {
                dir.Create();
            }

            return dir.EnumerateFiles().OrderByDescending(f => f.LastWriteTime).Select(fi => LoadSearch(fi.FullName)).Where(x => x is not null)!;
        }

        private static SearchRequest? LoadSearch(string file)
        {
            var serializer = new JsonSerializer();
            var search = serializer.Deserialize<SearchRequest>(File.ReadAllBytes(file));
            return search;
        }

        private static Settings LoadSettings(string file)
        {
            if (File.Exists(file))
            {
                var serializer = new JsonSerializer();
                var settings = serializer.Deserialize<Settings>(File.ReadAllBytes(file));
                if (settings is null)
                    throw new NullReferenceException("Settings should not be null. File might be corrupted");
                return settings;
            }
            else
            {
                //var retval = new Settings(
                //    new Settings.WindowSettings(false, 1024, 768),
                //    new Settings.ResultsPaneSettings(
                //        59, 400, 47, 256, 100, 50,
                //        0, 1, 2, 3, 4, -1),
                //    new Settings.OpenerSettings(
                //        new Settings.Opener[]{
                //            new Settings.Opener("vim", @"C:\tools\vim\vim82\gvim.exe", DoubleClickAction:true),
                //            new Settings.Opener("VsCode", @"C:\Program Files\Microsoft VS Code\Code.exe"),
                //        },
                //        new Settings.MultiOpener[] {
                //            new Settings.MultiOpener("KDiff3", @"C:\Program Files\KDiff3\kdiff3.exe", 3),
                //            new Settings.MultiOpener("Vimdiff", @"C:\tools\vim\vim82\gvim.exe", 4, "-d"),
                //        },
                //        new Settings.Opener[] {
                //            new Settings.Opener("PowerShell", @"powershell.exe", "-NoExit -Command Set-Location -LiteralPath '[folder]'"),
                //            new Settings.Opener("Command Prompt", @"cmd.exe", "/k cd /d \"[folder]\""),
                //            new Settings.Opener("Explorer", @"explorer.exe", "/select, \"[fullpath]\"")
                //        }
                //    )) ;
                //return new Settings()
                //{
                //    Maximised = false,
                //    Width = 1024,
                //    Height = 768,
                //    ColumnWidth0 = 100,
                //    ColumnWidth1 = 100,
                //    ColumnWidth2 = 100,
                //    ColumnWidth3 = 100,
                //    ColumnWidth4 = 100
                //};
                return Settings.Defaults;
            }
        }

        public static void SaveSettings()
        {
            var serializer = new JsonSerializer();
            var serialized = serializer.Serialize(Settings);
            File.WriteAllBytes(SettingsFile, serialized);
        }
    }
}
