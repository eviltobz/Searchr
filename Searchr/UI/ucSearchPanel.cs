namespace Searchr.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Searchr.Core;

    public partial class ucSearchPanel : UserControl
    {
        private readonly Action<SearchRequest> setDetail;
        public SearchRequest? CurrentSearch;
        private Opener defaultOpener;

        public ucSearchPanel(Action<SearchRequest> setDetail)
        {
            this.setDetail = setDetail;
            InitializeComponent();

            btnFilter.Enabled = false;
            btnSearch.Enabled = false;
            btnStop.Enabled = false;
        }

        public void Setup()
        {
            if (Config.Settings.ColumnDisplayIndex0 +
                Config.Settings.ColumnDisplayIndex1 +
                Config.Settings.ColumnDisplayIndex2 +
                Config.Settings.ColumnDisplayIndex3 +
                Config.Settings.ColumnDisplayIndex4 > 0)
            {
                dgResults.Columns[4].DisplayIndex = Config.Settings.ColumnDisplayIndex4;
                dgResults.Columns[3].DisplayIndex = Config.Settings.ColumnDisplayIndex3;
                dgResults.Columns[2].DisplayIndex = Config.Settings.ColumnDisplayIndex2;
                dgResults.Columns[1].DisplayIndex = Config.Settings.ColumnDisplayIndex1;
                dgResults.Columns[0].DisplayIndex = Config.Settings.ColumnDisplayIndex0;
            }

            dgResults.Columns[0].Width = Config.Settings.ColumnWidth0;
            dgResults.Columns[1].Width = Config.Settings.ColumnWidth1;
            dgResults.Columns[2].Width = Config.Settings.ColumnWidth2;
            dgResults.Columns[3].Width = Config.Settings.ColumnWidth3;
            dgResults.Columns[4].Width = Config.Settings.ColumnWidth4;

            dgResults.Columns[4].DefaultCellStyle.Format = "#,# KB";

            cmbIncludeFilePatterns.Items.Clear();
            cmbIncludeFilePatterns.Items.AddRange(Config.CommonIncludedExtensions.ToArray());

            cmbExcludeFilePatterns.Items.Clear();
            cmbExcludeFilePatterns.Items.AddRange(Config.CommonExcludedExtensions.ToArray());

            ucDirectory1.Directory.Items.Clear();
            ucDirectory1.Directory.Items.AddRange(Config.CommonDirs.ToArray());

            cmbExcludeFolderNames.Items.Clear();
            cmbExcludeFolderNames.Items.AddRange(Config.CommonExcludedDirs.ToArray());

            btnStop.Enabled = false;
            btnFilter.Enabled = false;
            btnSearch.Enabled = true;

            btnStop.Height = 27;
            btnFilter.Height = 27;
            btnSearch.Height = 27;

            ButtonColorSet();

            var latest = Config.LatestSearch();

            if (latest != null)
            {
                txtSearchTerm.Text = latest.SearchTerm;
                ucDirectory1.Directory.Text = latest.Directory;
                cmbIncludeFilePatterns.Text = string.Join(",", latest.IncludeFileWildcards);
                cmbExcludeFilePatterns.Text = string.Join(",", latest.ExcludeFileWildcards);
                cmbExcludeFolderNames.Text = string.Join(",", latest.ExcludeFolderNames);
                chkRecursive.Checked = latest.DirectoryOption == SearchOption.AllDirectories;
                chkRegex.Checked = latest.SearchMethod == SearchMethod.SingleLineRegex;
                chkMatchCase.Checked = latest.MatchCase;
                chkIncludeHidden.Checked = !latest.ExcludeHidden;
                chkIncludeSystem.Checked = !latest.ExcludeSystem;
                chkIncludeBinaryFiles.Checked = !latest.ExcludeBinaryFiles;
                chkSearchFileContents.Checked = latest.SearchFileContents;
                chkSearchFileName.Checked = latest.SearchFileName;
                chkSearchFilePath.Checked = latest.SearchFilePath;
            }

            SetupCheckBox(chkRecursive);
            SetupCheckBox(chkRegex);
            SetupCheckBox(chkMatchCase);
            SetupCheckBox(chkIncludeHidden);
            SetupCheckBox(chkIncludeSystem);
            SetupCheckBox(chkIncludeBinaryFiles);
            SetupCheckBox(chkSearchFileContents);
            SetupCheckBox(chkSearchFileName);
            SetupCheckBox(chkSearchFilePath);

            txtSearchTerm.Select(txtSearchTerm.Text!.Length, 0);
            ucDirectory1.Directory.Select(ucDirectory1.Directory!.Text!.Length, 0);
            cmbIncludeFilePatterns.Select(cmbIncludeFilePatterns.Text.Length, 0);
            cmbExcludeFilePatterns.Select(cmbExcludeFilePatterns.Text.Length, 0);
            cmbExcludeFolderNames.Select(cmbExcludeFolderNames.Text.Length, 0);

            SetUpOpeners();
            UpdateDetails();

            statusError.Click += (sender, args) => MessageBox.Show(errorDetail);
        }

        private void UpdateDetails()
        {
            setDetail(CurrentSearch!);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchNow(false);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            SearchNow(true);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            CurrentSearch!.Abort();
        }

        private void txtSearchTerm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchNow(false);
                e.Handled = true;
            }
        }

        public void Clear()
        {
            dgResults.Rows.Clear();
            statusText.Text = "";
        }

        private void SearchNow(bool filter)
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            CurrentSearch = GetSearchRequest(filter);
            UpdateDetails();

            if (string.IsNullOrEmpty(CurrentSearch.SearchTerm))
            {
                MessageBox.Show("No search term");
                return;
            }

            if (System.IO.Directory.Exists(CurrentSearch.Directory) == false)
            {
                MessageBox.Show("Invalid directory");
                return;
            }

            SaveCurrentSearch();

            btnFilter.Enabled = false;
            btnSearch.Enabled = false;
            btnStop.Enabled = true;
            btnStop.Focus();

            ButtonColorSet();

            var paths = dgResults.Rows.OfType<DataGridViewRow>().Select(r => r.FullPath()).ToList();

            statusText.Text = "Searching...";

            dgResults.Rows.Clear();

            var response = filter
                               ? Search.PerformFilter(CurrentSearch, paths)
                               : Search.PerformSearch(CurrentSearch);

            int totalFiles = 0;
            int totalHits = 0;

            Task.Run(() =>
            {
                try
                {
                    statusError.Visible = false;
                    foreach (var result in response.Results.GetConsumingEnumerable(CurrentSearch.CancellationToken))
                    {
                        if (result == SearchResult.Error)
                        {
                            var totalErrors = response.Errors.Count;
                            statusError.Text = $"{totalErrors} error(s) encountered.";
                            var err = string.Join("\n\n", response.Errors.Select(e => e.Message));
                            errorDetail = err;
                            statusError.Visible = true;
                            continue;
                        }

                        totalFiles++;
                        totalHits += result.TotalCount;

                        var row = new DataGridViewRow()
                        {
                            Height = 22,
                            Tag = result
                        };

                        var matchDetail = BuildToolTip(result);


                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = result.TotalCount
                                ,
                            ToolTipText = matchDetail
                        });

                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = result.FileName
                        });

                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = result.FileType
                        });

                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = result.RelativeFolder
                            ,
                            ToolTipText = result.FullFolder
                        });

                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = result.FileSize
                        });

                        dgResults.InvokeAction(dg =>
                        {
                            statusText.Text = $"Searching... Found {totalHits:n0} lines in {totalFiles:n0} files";
                            dg.Rows.Add(row);
                        });
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Search failed: {ex.Message}");
                }

                this.InvokeAction(_ =>
                {
                    timer.Stop();
                    statusText.Text = $"Found {totalHits:n0} lines in {totalFiles:n0} files in {timer.Elapsed.TotalSeconds:N3} seconds";

                    if (response.Error is null)
                        statusError.Visible = false;
                    else
                    {
                        var totalErrors = response.Errors.Count;
                        statusError.Text = $"{totalErrors} error(s) encountered.";
                        var err = string.Join("\n\n", response.Errors.Select(e => e.Message));
                        errorDetail = err;
                        statusError.Visible = true;
                    }

                    btnStop.Enabled = false;
                    btnFilter.Enabled = totalFiles > 0;
                    btnSearch.Enabled = true;
                    btnSearch.Focus();

                    ButtonColorSet();
                });
            });
        }

        private string errorDetail = "";

        private string BuildToolTip(SearchResult result)
        {
            const int PreviewLength = 120;
            var preview = result.TotalCount > 0 ? "Matches:" : "";
            foreach ((int num, string content) in result.Matches)
            {
                preview += $"\n#{num}: {content.Truncate(PreviewLength)}";
            }

            return preview;
        }

        private SearchRequest GetSearchRequest(bool filter)
        {
            var directory = ucDirectory1.Directory.Text;

            if (directory.EndsWith("\\") && !directory.EndsWith(":\\"))
            {
                directory = directory.Substring(0, directory.Length - 1);
                ucDirectory1.Directory.Text = directory;
            }

            var request = new SearchRequest(
                ucDirectory1.Directory.Text,
                chkRecursive.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly,
                txtSearchTerm.Text,
                chkRegex.Checked ? SearchMethod.SingleLineRegex : SearchMethod.SingleLine,
                chkMatchCase.Checked,
                4,
                GetExtensions(cmbExcludeFilePatterns.Text),
                GetExtensions(cmbIncludeFilePatterns.Text),
                GetFolders(cmbExcludeFolderNames.Text),
                !chkIncludeHidden.Checked,
                !chkIncludeSystem.Checked,
                !chkIncludeBinaryFiles.Checked,
                chkSearchFileContents.Checked,
                chkSearchFileName.Checked,
                chkSearchFilePath.Checked,
                filter ? CurrentSearch : null);

            return request;
        }

        private List<string> GetExtensions(string text)
        {
            return text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Distinct().ToList();
        }

        private List<string> GetFolders(string text)
        {
            return text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Distinct().ToList();
        }

        private void SaveCurrentSearch()
        {
            using (var sha = new SHA256Managed())
            {
                var serializer = new JsonSerializer();
                var serialized = serializer.Serialize(CurrentSearch);
                var hashCode = Hex.ToString(sha.ComputeHash(serialized));
                var historyFile = Path.Combine(Config.HistoryDirectory, string.Format("{0}.search", hashCode));
                if (File.Exists(historyFile) == false)
                {
                    File.WriteAllBytes(historyFile, serialized);
                }
                else
                {
                    new FileInfo(historyFile).LastWriteTime = DateTime.Now;
                }
            }
        }

        public DataGridView Results()
        {
            return dgResults;
        }

        public TextBox SearchTerm => txtSearchTerm;

        private void SetupCheckBox(CheckBox checkbox)
        {
            checkbox.CheckedChanged += Checkbox_CheckedChanged;

            Checkbox_CheckedChanged(checkbox, null!);

            checkbox.Height = 27;
        }

        private void Checkbox_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox checkbox)
            {
                Colorise(checkbox, blue, SystemColors.Control);
            }
        }

        readonly Color blue = Color.FromArgb(114, 200, 255);
        readonly Color red = Color.FromArgb(255, 161, 175);
        readonly Color green = Color.FromArgb(175, 255, 161);

        private void ButtonColorSet()
        {
            Colorise(btnStop, red, SystemColors.Control);
            Colorise(btnFilter, green, SystemColors.Control);
            Colorise(btnSearch, green, SystemColors.Control);
        }

        private void Colorise(Button btn, Color ifEnabled, Color ifDisabled)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = btn.Enabled ? ifEnabled : ifDisabled;
            btn.FlatAppearance.BorderColor = btn.Enabled ? Darker(ifEnabled) : Darker(ifDisabled, 0.95);
            btn.FlatAppearance.BorderSize = 1;
        }

        private void Colorise(CheckBox chk, Color ifEnabled, Color ifDisabled)
        {
            chk.FlatStyle = FlatStyle.Flat;
            chk.BackColor = chk.Checked ? ifEnabled : ifDisabled;
            chk.FlatAppearance.BorderColor = chk.Checked ? Darker(ifEnabled, 0.9) : Darker(ifDisabled, 0.95);
            chk.FlatAppearance.BorderSize = 1;
        }

        private Color Darker(Color src, double ratio = 0.85)
        {
            return Color.FromArgb((int)Math.Max(0, src.R * ratio),
                                  (int)Math.Max(0, src.G * ratio),
                                  (int)Math.Max(0, src.B * ratio));
        }

        private void OpenFile(Opener opener)
        {
            foreach (var row in dgResults.SelectedRows.OfType<DataGridViewRow>())
            {
                var commandLine = opener.CommandLinePattern
                    .Replace("[folder]", row.Folder())
                    .Replace("[filename]", row.FileName())
                    .Replace("[fullpath]", row.FullPath());

                Process.Start(opener.Path, commandLine);
            }
        }
        private class RowCompare : IEqualityComparer<DataGridViewRow>
        {
            public bool Equals(DataGridViewRow? x, DataGridViewRow? y) =>
                x?.Folder() == y?.Folder();

            public int GetHashCode(DataGridViewRow obj) =>
                obj.Folder().GetHashCode();
        }

        private void OpenLocation(Opener opener)
        {
            var folders = dgResults.SelectedRows.OfType<DataGridViewRow>().Distinct(new RowCompare());
            foreach (var row in folders)
            {
                var commandLine = opener.CommandLinePattern
                    .Replace("[folder]", row.Folder())
                    .Replace("[filename]", row.FileName())
                    .Replace("[fullpath]", row.FullPath());

                Process.Start(opener.Path, commandLine);
            }
        }

        private void MultiOpenLocation(MultiOpener opener)
        {
            var files = dgResults.SelectedRows.OfType<DataGridViewRow>().Select(x => $"\"{x.FullPath()}\"");
            var fileString = string.Join(" ", files);
            var commandLine = opener.CommandLinePattern + $" {fileString}";
            Process.Start(opener.Path, commandLine);
        }

        private void SetUpOpeners()
        {
            foreach (var editor in FileOpeners)
                AddMenuItem("Open file in " + editor.Name, (s, e) => OpenFile(editor));
            ResultsContextMenu.Items.Add(new ToolStripSeparator());

            foreach (var opener in LocationOpeners)
                AddMenuItem(opener.Name + " Here", (s, e) => OpenLocation(opener));
            ResultsContextMenu.Items.Add(new ToolStripSeparator());

            foreach (var opener in MultiOpeners)
            {
                opener.MenuItem = AddMenuItem("Open with " + opener.Name, (s, e) => MultiOpenLocation(opener));
            }

            ResultsContextMenu.Items.Add(new ToolStripSeparator());

            AddMenuItem("Copy folder to clipboard", (s, e) => Clipboard.SetText(activeRow?.Folder()));
            AddMenuItem("Copy filename to clipboard", (s, e) => Clipboard.SetText(activeRow?.FileName()));
            AddMenuItem("Copy full path to clipboard", (s, e) => Clipboard.SetText(activeRow?.FullPath()));
            ResultsContextMenu.Items.Add(new ToolStripSeparator());

            AddMenuItem("Clear results", (s, e) => Clear());

            defaultOpener = FileOpeners.Union(LocationOpeners).First(x => x.DoubleClickAction);
            ResultsContextMenu.Opening += ResultsContextMenu_Opening;
        }

        private void ResultsContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var opener in MultiOpeners)
            {
                if (dgResults.SelectedRows.Count > 1 && dgResults.SelectedRows.Count <= opener.MaxFiles)
                    opener!.MenuItem!.Enabled = true;
                else
                    opener!.MenuItem!.Enabled = false;
            }
        }

        private ToolStripMenuItem AddMenuItem(string name, EventHandler handler)
        {
            var item = new ToolStripMenuItem(name);
            item.Click += handler;
            ResultsContextMenu.Items.Add(item);
            return item;
        }

        private readonly struct Opener
        {
            public Opener(string name, string path, string commandLinePattern = "\"[fullpath]\"", bool doubleClickAction = false)
            {
                Name = name;
                Path = path;
                CommandLinePattern = commandLinePattern;
                DoubleClickAction = doubleClickAction;
            }

            public string Name { get; }
            public string Path { get; }
            public string CommandLinePattern { get; }
            public bool DoubleClickAction { get; }
        }

        private class MultiOpener
        {
            public MultiOpener(string name, string path, int maxFiles, string commandLinePattern = "")
            {
                Name = name;
                Path = path;
                MaxFiles = maxFiles;
                CommandLinePattern = commandLinePattern;
            }

            public string Name { get; }
            public string Path { get; }
            public int MaxFiles { get; }
            public string CommandLinePattern { get; }
            public ToolStripMenuItem? MenuItem { get; set; }
        }

        private delegate string CommandLineBuilder(string folder, string filename, string fullpath);

        private static readonly Opener[] FileOpeners = {
            new Opener("vim", @"C:\tools\vim\vim82\gvim.exe", doubleClickAction:true),
            new Opener("VsCode", @"C:\Program Files\Microsoft VS Code\Code.exe"),
        };

        private static readonly MultiOpener[] MultiOpeners =
        {
            new MultiOpener("KDiff3", @"C:\Program Files\KDiff3\kdiff3.exe", 3),
            new MultiOpener("Vimdiff", @"C:\tools\vim\vim82\gvim.exe", 4, "-d"),
        };

        private static readonly Opener[] LocationOpeners = {
            new Opener("PowerShell", @"powershell.exe", "-NoExit -Command Set-Location -LiteralPath '[folder]'"),
            new Opener("Command Prompt", @"cmd.exe", "/k cd /d \"[folder]\""),
            new Opener("Explorer", @"explorer.exe", "/select, \"[fullpath]\"")
        };

        private void dgResults_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Button == MouseButtons.Right)
            {
                activeRow = dgResults.Rows[e.RowIndex];
                if (!activeRow.Selected)
                {
                    dgResults.ClearSelection();
                    activeRow.Selected = true;
                }
                Rectangle r = dgResults.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                ResultsContextMenu.Show((Control)sender, r.Left + e.X, r.Top + e.Y);
            }
        }

        private void dgResults_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgResults.ClearSelection();
                activeRow = dgResults.Rows[e.RowIndex];
                activeRow.Selected = true;
                OpenFile(defaultOpener);
            }
        }

        private DataGridViewRow? activeRow = null;

        private void dgResults_SelectionChanged(object sender, EventArgs e)
        {
            if (dgResults.SelectedRows.Count == 1)
                activeRow = dgResults.SelectedRows[0];
        }

        private void dgResults_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.Print($"DOWN - Code:{e.KeyCode}");
            if (e.KeyCode == Keys.Return)
                e.Handled = true;
        }

        private void dgResults_KeyUp(object sender, KeyEventArgs e)
        {
            //Debug.Print($"UP   - Code:{e.KeyCode}");
            if (e.KeyCode == Keys.Return)
            {
                e.Handled = true;
            }
        }

        private void dgResults_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Debug.Print($"PRES - Char:{e.KeyChar}");
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                if (activeRow != null && dgResults.SelectedRows.Count == 1)
                    OpenFile(defaultOpener);
            }
        }
    }
}
