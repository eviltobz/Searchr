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
    );

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
        FileOpener[] FileOpeners,
        DiffOpener[] DiffOpeners,
        LocationOpener[] LocationOpeners,
        ProjectOpener[] ProjectOpeners
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
        string CommandLinePattern);

    public record DiffOpener(
        string Name,
        string Path,
        string CommandLinePattern,
        int MaxFiles);

    public record ProjectOpener(
        string Name,
        string Path,
        string CommandLinePattern,
        string TargetPattern,
        FileOrFolder TargetType);

    public enum FileOrFolder
    {
        File,
        Folder
    }

}

