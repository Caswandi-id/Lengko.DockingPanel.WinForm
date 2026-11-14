using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Lengko.DockingPanel.WinForms;

namespace Lengko.DockingPanel.Demo.CSharp;

public class MainForm : Form
{
    private readonly LengkoDockPanel _dockPanel;
    private readonly ListBox _outputList;
    private readonly MenuStrip _menuStrip;
    private readonly StatusStrip _statusStrip;
    private readonly ToolStripStatusLabel _statusLabel;
    private int _docCount = 2;

    public MainForm()
    {
        Text = "Lengko DockingPanel Demo (C#) - Visual Studio Style Workspace";
        Size = new Size(1100, 720);
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(600, 450);

        // Status Strip
        _statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel { Text = "Ready. Ctrl+Tab to switch, drag tab to reorder, drag out to dock, right-click to pin/split." };
        _statusStrip.Items.Add(_statusLabel);

        // Menu Strip
        _menuStrip = new MenuStrip();
        var fileMenu = new ToolStripMenuItem("&File");
        var viewMenu = new ToolStripMenuItem("&View");
        var themeMenu = new ToolStripMenuItem("&Theme");
        var tabStyleMenu = new ToolStripMenuItem("Tab &Style");
        var layoutMenu = new ToolStripMenuItem("&Layout");

        var newDocItem = new ToolStripMenuItem("&New Document", null, (_, _) => CreateNewDocument());
        var exitItem = new ToolStripMenuItem("E&xit", null, (_, _) => Close());
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { newDocItem, new ToolStripSeparator(), exitItem });

        var themeLight = new ToolStripMenuItem("Light Preset", null, (_, _) => SetTheme(DockThemeMode.Light));
        var themeDark = new ToolStripMenuItem("Dark Preset", null, (_, _) => SetTheme(DockThemeMode.Dark));
        var themeBlue = new ToolStripMenuItem("VS Blue Preset", null, (_, _) => SetTheme(DockThemeMode.Blue));
        themeMenu.DropDownItems.AddRange(new ToolStripItem[] { themeLight, themeDark, themeBlue });

        var tabRect = new ToolStripMenuItem("Rectangular", null, (_, _) => SetTabStyle(DockTabStyle.Rectangular));
        var tabRounded = new ToolStripMenuItem("Rounded", null, (_, _) => SetTabStyle(DockTabStyle.Rounded));
        var tabAngled = new ToolStripMenuItem("Angled", null, (_, _) => SetTabStyle(DockTabStyle.Angled));
        var tabUnderline = new ToolStripMenuItem("Underline", null, (_, _) => SetTabStyle(DockTabStyle.Underline));
        tabStyleMenu.DropDownItems.AddRange(new ToolStripItem[] { tabRect, tabRounded, tabAngled, tabUnderline });

        var saveLayout = new ToolStripMenuItem("&Save Layout to XML", null, (_, _) => SaveWorkspaceLayout());
        var loadLayout = new ToolStripMenuItem("&Restore Layout from XML", null, (_, _) => LoadWorkspaceLayout());
        var resetLayout = new ToolStripMenuItem("Re&set Default Layout", null, (_, _) => SetupDefaultLayout());
        layoutMenu.DropDownItems.AddRange(new ToolStripItem[] { saveLayout, loadLayout, new ToolStripSeparator(), resetLayout });

        _menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, viewMenu, themeMenu, tabStyleMenu, layoutMenu });

        // DockPanel
        _dockPanel = new LengkoDockPanel
        {
            Dock = DockStyle.Fill,
            ThemeMode = DockThemeMode.Light
        };

        _outputList = new ListBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            Font = new Font("Consolas", 9.0f)
        };

        _dockPanel.ActiveContentChanged += (_, content) =>
        {
            if (content != null)
            {
                Log($"Active Content changed: '{content.TabText}' ({content.DockState})");
                _statusLabel.Text = $"Active: {content.TabText} [{content.DockState}]";
            }
        };

        // Layout controls
        Controls.Add(_dockPanel);
        Controls.Add(_statusStrip);
        Controls.Add(_menuStrip);
        MainMenuStrip = _menuStrip;

        SetupDefaultLayout();
    }

    private void SetupDefaultLayout()
    {
        _dockPanel.SuspendLayout();

        // Clear existing
        for (int i = _dockPanel.Contents.Count - 1; i >= 0; i--)
        {
            _dockPanel.RemoveContent(_dockPanel.Contents[i]);
        }

        // 1. Central Documents
        var codeEditor = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            Font = new Font("Consolas", 10.0f),
            Text = "// Welcome to Lengko.DockingPanel.WinForm\r\n// Visual Studio style docking container for WinForms\r\n\r\nusing System;\r\nusing Lengko.DockingPanel.WinForms;\r\n\r\nnamespace Demo\r\n{\r\n    class App\r\n    {\r\n        static void Main() => Console.WriteLine(\"Hello Docking!\");\r\n    }\r\n}"
        };
        var doc1 = new LengkoDockContent(codeEditor, "Program.cs");
        doc1.Show(_dockPanel, DockState.Document);

        var readmeEditor = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 9.5f),
            Text = "# Lengko.DockingPanel.WinForm\r\n\r\nFeatures:\r\n- Tabbed MDI Documents\r\n- DockLeft, DockRight, DockTop, DockBottom zones\r\n- AutoHide Perimeter Strips\r\n- Floating Tool Windows\r\n- Dock Diamonds and drop indicators\r\n- XML Serialization\r\n- Pure WinForms, zero external dependencies"
        };
        var doc2 = new LengkoDockContent(readmeEditor, "README.md");
        doc2.Show(_dockPanel, DockState.Document);

        // 2. Dock Left: Solution Explorer
        var treeView = new TreeView { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None };
        var rootNode = treeView.Nodes.Add("Solution 'Lengko.DockingPanel' (2 projects)");
        var srcNode = rootNode.Nodes.Add("src");
        srcNode.Nodes.Add("Lengko.DockingPanel.WinForms");
        var samplesNode = rootNode.Nodes.Add("samples");
        samplesNode.Nodes.Add("Lengko.DockingPanel.Demo.CSharp");
        samplesNode.Nodes.Add("Lengko.DockingPanel.Demo.VB");
        samplesNode.Nodes.Add("Lengko.DockingPanel.Demo.FSharp");
        rootNode.ExpandAll();

        var solutionExplorer = new LengkoDockContent(treeView, "Solution Explorer");
        solutionExplorer.Show(_dockPanel, DockState.DockLeft);

        // 3. Dock Right: Properties
        var propGrid = new PropertyGrid { Dock = DockStyle.Fill, SelectedObject = _dockPanel };
        var propContent = new LengkoDockContent(propGrid, "Properties");
        propContent.Show(_dockPanel, DockState.DockRight);

        // 4. Dock Bottom: Output
        var outputContent = new LengkoDockContent(_outputList, "Output");
        outputContent.Show(_dockPanel, DockState.DockBottom);

        // 5. AutoHide Left: Toolbox
        var toolboxList = new ListBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None };
        toolboxList.Items.AddRange(new object[] { "Pointer", "LengkoDockPanel", "LengkoDockContent", "Button", "TextBox", "TreeView", "ListBox" });
        var toolbox = new LengkoDockContent(toolboxList, "Toolbox");
        toolbox.Show(_dockPanel, DockState.AutoHideLeft);

        _dockPanel.ResumeLayout(true);
        Log("Default layout initialized.");
    }

    private void CreateNewDocument()
    {
        _docCount++;
        var tb = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            Font = new Font("Consolas", 10.0f),
            Text = $"// Document {_docCount} created at {DateTime.Now:T}\r\n// Edit this text to see the dirty '*' indicator.\r\n"
        };
        var doc = new LengkoDockContent(tb, $"Document{_docCount}.txt");

        tb.TextChanged += (_, _) => doc.IsModified = true;
        doc.ContentClosing += (_, e) =>
        {
            if (doc.IsModified)
            {
                var res = MessageBox.Show(this, $"Document '{doc.TabText}' has unsaved changes.\r\nSave changes before closing?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel) e.Cancel = true;
                else if (res == DialogResult.Yes) doc.IsModified = false;
            }
        };

        doc.Show(_dockPanel, DockState.Document);
        Log($"Created Document{_docCount}.txt");
    }

    private void SetTheme(DockThemeMode mode)
    {
        _dockPanel.ThemeMode = mode;
        var theme = _dockPanel.Theme;

        if (theme.IsDark)
        {
            BackColor = Color.FromArgb(30, 30, 30);
            ForeColor = Color.FromArgb(240, 240, 240);
            _menuStrip.Renderer = new ToolStripProfessionalRenderer(new DarkColorTable());
            _menuStrip.BackColor = Color.FromArgb(32, 32, 36);
            _menuStrip.ForeColor = Color.FromArgb(240, 240, 240);
            _statusStrip.Renderer = new ToolStripProfessionalRenderer(new DarkColorTable());
            _statusStrip.BackColor = Color.FromArgb(32, 32, 36);
            _statusStrip.ForeColor = Color.FromArgb(200, 200, 200);
            _statusLabel.ForeColor = Color.FromArgb(200, 200, 200);
        }
        else
        {
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
            _menuStrip.Renderer = new ToolStripProfessionalRenderer();
            _menuStrip.BackColor = SystemColors.Control;
            _menuStrip.ForeColor = SystemColors.ControlText;
            _statusStrip.Renderer = new ToolStripProfessionalRenderer();
            _statusStrip.BackColor = SystemColors.Control;
            _statusStrip.ForeColor = SystemColors.ControlText;
            _statusLabel.ForeColor = SystemColors.ControlText;
        }

        ApplyImmersiveDarkMode(theme.IsDark);
        Log($"Theme changed to {mode}");
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    private void ApplyImmersiveDarkMode(bool isDark)
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT) return;
        try
        {
            int useDark = isDark ? 1 : 0;
            if (DwmSetWindowAttribute(Handle, 20, ref useDark, sizeof(int)) != 0)
            {
                DwmSetWindowAttribute(Handle, 19, ref useDark, sizeof(int));
            }
        }
        catch { }
    }

    private void SetTabStyle(DockTabStyle style)
    {
        _dockPanel.TabStyle = style;
        Log($"Tab style changed to {style}");
    }

    private sealed class DarkColorTable : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Color.FromArgb(32, 32, 36);
        public override Color ImageMarginGradientBegin => Color.FromArgb(32, 32, 36);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(32, 32, 36);
        public override Color ImageMarginGradientEnd => Color.FromArgb(32, 32, 36);
        public override Color MenuBorder => Color.FromArgb(50, 50, 56);
        public override Color MenuItemBorder => Color.FromArgb(60, 60, 70);
        public override Color MenuItemSelected => Color.FromArgb(50, 50, 60);
        public override Color MenuStripGradientBegin => Color.FromArgb(32, 32, 36);
        public override Color MenuStripGradientEnd => Color.FromArgb(32, 32, 36);
        public override Color StatusStripGradientBegin => Color.FromArgb(32, 32, 36);
        public override Color StatusStripGradientEnd => Color.FromArgb(32, 32, 36);
    }

    private void SaveWorkspaceLayout()
    {
        try
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dock_layout.xml");
            _dockPanel.SaveLayout(path);
            Log($"Layout saved to {path}");
            MessageBox.Show(this, $"Layout saved to:\r\n{path}", "Save Layout", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            Log($"Error saving layout: {ex.Message}");
        }
    }

    private void LoadWorkspaceLayout()
    {
        try
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dock_layout.xml");
            if (!File.Exists(path))
            {
                MessageBox.Show(this, "No saved layout file found. Please Save Layout first.", "Restore Layout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _dockPanel.LoadLayout(path, (typeName, tabText) => new LengkoDockContent(new TextBox { Multiline = true, Dock = DockStyle.Fill, Text = $"Restored: {tabText}" }, tabText));
            Log($"Layout restored from {path}");
        }
        catch (Exception ex)
        {
            Log($"Error restoring layout: {ex.Message}");
        }
    }

    private void Log(string message)
    {
        _outputList.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        _outputList.TopIndex = _outputList.Items.Count - 1;
    }
}
