Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports Lengko.DockingPanel.WinForms

Namespace Lengko.DockingPanel.Demo.VB
    Public Class MainForm
        Inherits Form

        Private ReadOnly _dockPanel As LengkoDockPanel
        Private ReadOnly _outputList As ListBox

        Public Sub New()
            Text = "Lengko DockingPanel Demo (VB.NET)"
            Size = New Size(1050, 700)
            StartPosition = FormStartPosition.CenterScreen
            MinimumSize = New Size(600, 450)

            _dockPanel = New LengkoDockPanel With {
                .Dock = DockStyle.Fill,
                .ThemeMode = DockThemeMode.Light
            }

            _outputList = New ListBox With {
                .Dock = DockStyle.Fill,
                .BorderStyle = BorderStyle.None,
                .Font = New Font("Consolas", 9.0F)
            }

            Dim menuStrip As New MenuStrip()
            Dim themeMenu As New ToolStripMenuItem("&Theme")
            Dim lightItem As New ToolStripMenuItem("Light", Nothing, Sub(s, e) _dockPanel.ThemeMode = DockThemeMode.Light)
            Dim darkItem As New ToolStripMenuItem("Dark", Nothing, Sub(s, e) _dockPanel.ThemeMode = DockThemeMode.Dark)
            Dim blueItem As New ToolStripMenuItem("VS Blue", Nothing, Sub(s, e) _dockPanel.ThemeMode = DockThemeMode.Blue)
            themeMenu.DropDownItems.AddRange(New ToolStripItem() {lightItem, darkItem, blueItem})
            menuStrip.Items.Add(themeMenu)

            Controls.Add(_dockPanel)
            Controls.Add(menuStrip)
            MainMenuStrip = menuStrip

            SetupWorkspace()
        End Sub

        Private Sub SetupWorkspace()
            _dockPanel.SuspendLayout()

            ' 1. Document
            Dim codeBox As New TextBox With {
                .Multiline = True,
                .Dock = DockStyle.Fill,
                .BorderStyle = BorderStyle.None,
                .Font = New Font("Consolas", 10.0F),
                .Text = "' VB.NET Lengko Docking Sample" & vbCrLf & "Module Main" & vbCrLf & "    Sub Main()" & vbCrLf & "    End Sub" & vbCrLf & "End Module"
            }
            Dim doc As New LengkoDockContent(codeBox, "Module1.vb")
            doc.Show(_dockPanel, DockState.Document)

            ' 2. Left: Solution Explorer
            Dim tree As New TreeView With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None}
            Dim root = tree.Nodes.Add("Solution 'VB.Demo'")
            root.Nodes.Add("Module1.vb")
            root.ExpandAll()
            Dim explorer As New LengkoDockContent(tree, "Solution Explorer")
            explorer.Show(_dockPanel, DockState.DockLeft)

            ' 3. Right: Properties
            Dim grid As New PropertyGrid With {.Dock = DockStyle.Fill, .SelectedObject = _dockPanel}
            Dim prop As New LengkoDockContent(grid, "Properties")
            prop.Show(_dockPanel, DockState.DockRight)

            ' 4. Bottom: Output
            Dim outDoc As New LengkoDockContent(_outputList, "Output")
            outDoc.Show(_dockPanel, DockState.DockBottom)

            _outputList.Items.Add("[Ready] VB.NET Docking Workspace Loaded.")
            _dockPanel.ResumeLayout(True)
        End Sub
    End Class
End Namespace
