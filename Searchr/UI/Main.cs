using System;
using System.Linq;
using System.Windows.Forms;

namespace Searchr.UI
{
    using Searchr.Core;

    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            AddResultsTab();
        }

        private void frmMain_Click(object sender, EventArgs e)
        {
            txtHidden.Focus();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings();
        }

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Maximized)
            {
                Config.Settings.Window.Width = Width;
                Config.Settings.Window.Height = Height;
            }
        }

        private void frmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                if (resultsTabs.SelectedTab.Controls[0] is ucSearchPanel panel)
                {
                    panel.CurrentSearch?.Abort();
                }
            }
        }

        private void resultsTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resultsTabs.SelectedIndex == resultsTabs.TabCount - 1)
            {
                AddResultsTab();
            }
        }

        private void UpdateTabTitle(TabPage tab, SearchRequest search)
        {
            if (search is null)
            {
                tab.Text = "Searchr";
                tab.ToolTipText = "No searches executed yet...";
            }
            else
            {
                tab.Text = search.SearchTerm?.Truncate(10);
                tab.ToolTipText = BuildSearchDescription(search).text;
            }
        }

        private (string text, int indent) BuildSearchDescription(SearchRequest search, int indent = 0)
        {
            var text = "";
            var pad = "";
            int totalIndent;
            if (search.ParentSearch != null)
            {
                var (val, indentX) = BuildSearchDescription(search.ParentSearch, indent + 1);
                totalIndent = indentX;
                pad = "".PadLeft((totalIndent - indent) * 2);
                text += val + $"\n\n{pad}Filter: ";
            }
            else
            {
                text += "Search: ";
                totalIndent = indent;
            }

            text += search.SearchTerm + $"\n{pad}" + search.Directory;
            return (text, totalIndent);
        }

        private void resultsTabs_MouseClick(object sender, MouseEventArgs e)
        {
            var tabControl = sender as TabControl;
            if (tabControl is null)
                return;

            var tabs = tabControl.TabPages;

            if (e.Button == MouseButtons.Middle)
            {
                var tabToRemove = tabs.OfType<TabPage>()
                                      .Where((t, i) => tabControl.GetTabRect(i).Contains(e.Location) && i < tabs.Count - 1)
                                      .FirstOrDefault();

                if (tabToRemove != null) tabs.Remove(tabToRemove);
            }
        }

        private void SaveSettings()
        {
            var currentResults = CurrentResults();

            if (currentResults != null)
            {
                Config.Settings.ResultsPane.ColWidthLines = currentResults.Columns[0].Width;
                Config.Settings.ResultsPane.ColWidthName = currentResults.Columns[1].Width;
                Config.Settings.ResultsPane.ColWidthExtension = currentResults.Columns[2].Width;
                Config.Settings.ResultsPane.ColWidthDirectory = currentResults.Columns[3].Width;
                Config.Settings.ResultsPane.ColWidthSize = currentResults.Columns[4].Width;
                //Config.Settings.ColumnWidth0 = currentResults.Columns[0].Width;
                //Config.Settings.ColumnWidth1 = currentResults.Columns[1].Width;
                //Config.Settings.ColumnWidth2 = currentResults.Columns[2].Width;
                //Config.Settings.ColumnWidth3 = currentResults.Columns[3].Width;
                //Config.Settings.ColumnWidth4 = currentResults.Columns[4].Width;


                Config.Settings.ResultsPane.ColIndexLines = currentResults.Columns[0].DisplayIndex;
                Config.Settings.ResultsPane.ColIndexName = currentResults.Columns[1].DisplayIndex;
                Config.Settings.ResultsPane.ColIndexExtension = currentResults.Columns[2].DisplayIndex;
                Config.Settings.ResultsPane.ColIndexDirectory = currentResults.Columns[3].DisplayIndex;
                Config.Settings.ResultsPane.ColIndexSize = currentResults.Columns[4].DisplayIndex;
                //Config.Settings.ColumnDisplayIndex0 = currentResults.Columns[0].DisplayIndex;
                //Config.Settings.ColumnDisplayIndex1 = currentResults.Columns[1].DisplayIndex;
                //Config.Settings.ColumnDisplayIndex2 = currentResults.Columns[2].DisplayIndex;
                //Config.Settings.ColumnDisplayIndex3 = currentResults.Columns[3].DisplayIndex;
                //Config.Settings.ColumnDisplayIndex4 = currentResults.Columns[4].DisplayIndex;
            }

            Config.Settings.Window.Maximised = WindowState == FormWindowState.Maximized;
            Config.SaveSettings();
        }

        private DataGridView? CurrentResults() => CurrentSearchPanel?.Results();

        private ucSearchPanel? CurrentSearchPanel => resultsTabs.SelectedTab.Controls.OfType<ucSearchPanel>().FirstOrDefault();

        private void AddResultsTab()
        {
            var newTab = CreateTabPage();
            resultsTabs.TabPages.Insert(resultsTabs.TabCount - 1, newTab);
            resultsTabs.SelectedTab = newTab;

            if (newTab.Controls[0] is ucSearchPanel p)
            {
                p.Setup();
                p.SearchTerm.Focus();
            }
        }

        private TabPage CreateTabPage()
        {
            var newTab = new TabPage
            {
                Padding = new Padding(7),
                TabIndex = 0,
                Text = "Searchr",
                UseVisualStyleBackColor = true
            };

            var panel = new ucSearchPanel(x => UpdateTabTitle(newTab, x))
            {
                Dock = DockStyle.Fill
            };

            newTab.Controls.Add(panel);

            return newTab;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                CurrentSearchPanel?.SearchTerm.Focus();
                CurrentSearchPanel?.SearchTerm.SelectAll();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.T))
            {
                AddResultsTab();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.W))
            {
                if (resultsTabs.TabPages.Count == 2)
                {
                    CurrentSearchPanel?.Clear();
                    resultsTabs.SelectedTab.Text = "Searchr";
                }
                else
                {
                    var tabToRemove = resultsTabs.SelectedTab;
                    var currentIndex = resultsTabs.SelectedIndex;
                    if (currentIndex == resultsTabs.TabCount - 2) currentIndex--;
                    resultsTabs.TabPages.Remove(tabToRemove);
                    resultsTabs.SelectedIndex = currentIndex;
                }
                return true;
            }
            else if (keyData == (Keys.Shift | Keys.Control | Keys.Tab))
            {
                if (resultsTabs.TabPages.Count > 1)
                {
                    var newIndex = resultsTabs.SelectedIndex - 1;
                    if (newIndex == -1) newIndex = resultsTabs.TabPages.Count - 2;
                    resultsTabs.SelectedIndex = newIndex;
                }
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Tab))
            {
                if (resultsTabs.TabPages.Count > 1)
                {
                    var newIndex = resultsTabs.SelectedIndex + 1;
                    if (newIndex >= resultsTabs.TabPages.Count - 1) newIndex = 0;
                    resultsTabs.SelectedIndex = newIndex;
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
