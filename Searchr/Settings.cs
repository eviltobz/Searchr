using System.Collections.Generic;

namespace Searchr.UI
{
    //public class Settings
    //{
    //    public bool Maximised;
    //    public int Width;
    //    public int Height;
    //    // indexy names suck - change to what they represent
    //    public int ColumnWidth0;
    //    public int ColumnWidth1;
    //    public int ColumnWidth2;
    //    public int ColumnWidth3;
    //    public int ColumnWidth4;
    //    //public int ColumnWidth5;
    //    public int ColumnDisplayIndex0;
    //    public int ColumnDisplayIndex1;
    //    public int ColumnDisplayIndex2;
    //    public int ColumnDisplayIndex3;
    //    public int ColumnDisplayIndex4;
    //    //public int ColumnDisplayIndex5;
    //}

    public record Settings(
        //Settings.WindowSettings Window,
        //Settings.ResultsPaneSettings ResultsPane,
        //Settings.OpenerSettings Openers
        WindowSettings Window,
        ResultsPaneSettings ResultsPane,
        OpenerSettings Openers
    )
    {

        public static readonly Settings Defaults =
                     new Settings(
                        new WindowSettings(false, 1024, 768),
                        new ResultsPaneSettings(
                            59, 400, 47, 256, 100, 50,
                            0, 1, 2, 3, 4, -1),
                        new OpenerSettings(
                            new FileOpener[]{
                            new FileOpener("vim", @"C:\tools\vim\vim82\gvim.exe",true, false, DoubleClickAction: true),
                            new FileOpener("vim (single instance)", @"C:\tools\vim\vim82\gvim.exe", false, true),
                            new FileOpener("VsCode", @"C:\Program Files\Microsoft VS Code\Code.exe",true, true),
                            },
                            new DiffOpener[] {
                            new DiffOpener("KDiff3", @"C:\Program Files\KDiff3\kdiff3.exe", 3, ""),
                            new DiffOpener("Vimdiff", @"C:\tools\vim\vim82\gvim.exe", 4, "-d"),
                            },
                            new LocationOpener[] {
                            new LocationOpener("PowerShell", @"powershell.exe", "-NoExit -Command Set-Location -LiteralPath '[folder]'"),
                            new LocationOpener("Command Prompt", @"cmd.exe", "/k cd /d [folder]"),
                            new LocationOpener("Explorer", @"explorer.exe", "/select, [fullpath]")
                            }
                        )
                    );
    }

    public record WindowSettings(
        bool Maximised,
        int Width,
        int Height)
    {
        public bool Maximised { get; set; } = Maximised;
        public int Width { get; set; } = Width;
        public int Height { get; set; } = Height;
    }

    public record ResultsPaneSettings(
        int ColWidthLines,
        int ColWidthName,
        int ColWidthExtension,
        int ColWidthDirectory,
        int ColWidthSize,
        int ColWidthIcon,
        int ColIndexLines,
        int ColIndexName,
        int ColIndexExtension,
        int ColIndexDirectory,
        int ColIndexSize,
        int ColIndexIcon)
    {
        public int ColWidthLines { get; set; } = ColWidthLines;
        public int ColWidthName { get; set; } = ColWidthName;
        public int ColWidthExtension { get; set; } = ColWidthExtension;
        public int ColWidthDirectory { get; set; } = ColWidthDirectory;
        public int ColWidthSize { get; set; } = ColWidthSize;
        public int ColWidthIcon { get; set; } = ColWidthIcon;
        public int ColIndexLines { get; set; } = ColIndexLines;
        public int ColIndexName { get; set; } = ColIndexName;
        public int ColIndexExtension { get; set; } = ColIndexExtension;
        public int ColIndexDirectory { get; set; } = ColIndexDirectory;
        public int ColIndexSize { get; set; } = ColIndexSize;
        public int ColIndexIcon { get; set; } = ColIndexIcon;
    };

    public record OpenerSettings(
        FileOpener[] FileOpeners, // = new Opener[0],
        DiffOpener[] DiffOpeners, // = new MultiOpener[0],
        LocationOpener[] LocationOpeners // = new Opener[0]
    );

    public record FileOpener(
        string Name,
        string Path,
        bool SingleFile,
        bool MultiFile,
        bool DoubleClickAction = false);

    public record LocationOpener(
        string Name,
        string Path,
        string CommandLinePattern); // = "\"[fullpath]\"");//,
        //bool DoubleClickAction = false);

    //public record MultiOpener(
    //    string Name,
    //    string Path,
    //    int MaxFiles,
    //    string CommandLinePattern = "");
    //public record FileOpener
    //{
    //    public FileOpener(string name, string path, bool multiOpen)// string commandLinePattern = "\"[fullpath]\"", bool doubleClickAction = false)
    //    {
    //        Name = name;
    //        Path = path;
    //        MultiOpen = multiOpen;
    //        //CommandLinePattern = commandLinePattern;
    //        //DoubleClickAction = doubleClickAction;
    //    }

    //    public string Name { get; }
    //    public string Path { get; }
    //    public bool MultiOpen { get; }
    //    //public string commandlinepattern { get; }
    //    //public bool doubleclickaction { get; }
    //}

    //public record LocationOpener
    //{
    //    public LocationOpener(string name, string path, string commandLinePattern)
    //    {
    //        Name = name;
    //        Path = path;
    //        CommandLinePattern = commandLinePattern;
    //    }

    //    public string Name { get; }
    //    public string Path { get; }
    //    public string CommandLinePattern { get; }
    //}

    public record DiffOpener(
        string Name,
        string Path,
        int MaxFiles,
        string CommandLinePattern);

    //{
    //    public DiffOpener(string name, string path, int maxFiles, string commandLinePattern = "")
    //    {
    //        Name = name;
    //        Path = path;
    //        MaxFiles = maxFiles;
    //        CommandLinePattern = commandLinePattern;
    //    }

    //    public string Name { get; }
    //    public string Path { get; }
    //    public int MaxFiles { get; }
    //    public string CommandLinePattern { get; }
    //    public ToolStripMenuItem? MenuItem { get; set; }
    //}


}

//}
