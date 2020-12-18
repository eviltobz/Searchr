namespace Searchr.UI
{
    using System.Windows.Forms;

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

    //public record ActiveDiffOpener
    //{
    //    public ActiveDiffOpener(DiffOpener opener, ToolStripMenuItem menuItem)
    //    {
    //        Name = opener.Name;
    //        Path = opener.Path;
    //        MaxFiles = opener.MaxFiles;
    //        CommandLinePattern = opener.CommandLinePattern;
    //        MenuItem = menuItem;
    //    }

    //    public string Name { get; }
    //    public string Path { get; }
    //    public int MaxFiles { get; }
    //    public string CommandLinePattern { get; }
    //    public ToolStripMenuItem MenuItem { get; }
    //}

    //public record ActiveFileOpener
    //{
    //    public ActiveFileOpener(FileOpener opener, ToolStripMenuItem menuItem)
    //    {
    //        Name = opener.Name;
    //        Path = opener.Path;
    //        SingleFile = opener.SingleFile;
    //        MultiFile = opener.MultiFile;
    //        MenuItem = menuItem;
    //    }

    //    public string Name { get; }
    //    public string Path { get; }
    //    public bool SingleFile { get; }
    //    public bool MultiFile { get; }
    //    public ToolStripMenuItem MenuItem { get; }
    //}

    public record ActiveOpener<T>(T Opener, ToolStripMenuItem MenuItem);

}
