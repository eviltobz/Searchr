namespace Searchr.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Searchr.Core;

    public static class Config
    {
        private const string SettingsFile = @"searchr.json";

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
            return serializer.Deserialize<SearchRequest>(File.ReadAllBytes(file));
        }

        private static Settings LoadSettings(string file)
        {
            if (!File.Exists(file))
                CreateDefaultSettingsFile(file);

            var serializer = new JsonSerializer();
            var settings = serializer.Deserialize<Settings>(File.ReadAllBytes(file));
            if (settings is null)
                throw new NullReferenceException("Settings should not be null. File might be corrupted");
            return settings;
        }

        private static void CreateDefaultSettingsFile(string file)
        {
            const string ResourceName = "Searchr.DefaultConfig.json";
            var assembly = Assembly.GetExecutingAssembly();

            using Stream stream = assembly.GetManifestResourceStream(ResourceName)!;
            using StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            File.WriteAllText(file, result);
        }

        public static void SaveSettings()
        {
            var serializer = new JsonSerializer();
            var serialized = serializer.Serialize(Settings);
            File.WriteAllBytes(SettingsFile, serialized);
        }
    }
}
