namespace Lengko.DockingPanel.Demo.FSharp

open System
open System.Drawing
open System.Windows.Forms
open Lengko.DockingPanel.WinForms

module Program =

    type DockDemoForm() as this =
        inherit Form()

        let dockPanel = new LengkoDockPanel(Dock = DockStyle.Fill, ThemeMode = DockThemeMode.Light)
        let outputList = new ListBox(Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, Font = new Font("Consolas", 9.0f))

        do
            this.Text <- "Lengko DockingPanel Demo (F#)"
            this.Size <- Size(1050, 700)
            this.StartPosition <- FormStartPosition.CenterScreen
            this.MinimumSize <- Size(600, 450)

            let menuStrip = new MenuStrip()
            let themeMenu = new ToolStripMenuItem("&Theme")
            let lightItem = new ToolStripMenuItem("Light", null, fun _ _ -> dockPanel.ThemeMode <- DockThemeMode.Light)
            let darkItem = new ToolStripMenuItem("Dark", null, fun _ _ -> dockPanel.ThemeMode <- DockThemeMode.Dark)
            let blueItem = new ToolStripMenuItem("VS Blue", null, fun _ _ -> dockPanel.ThemeMode <- DockThemeMode.Blue)
            themeMenu.DropDownItems.AddRange([| lightItem :> ToolStripItem; darkItem :> ToolStripItem; blueItem :> ToolStripItem |])
            menuStrip.Items.Add(themeMenu) |> ignore

            this.Controls.Add(dockPanel)
            this.Controls.Add(menuStrip)
            this.MainMenuStrip <- menuStrip

            dockPanel.SuspendLayout()

            // 1. Document
            let codeBox = new TextBox(
                Multiline = true,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 10.0f),
                Text = "// F# Lengko Docking Sample\r\nopen System\r\n\r\nprintfn \"Hello Docking!\""
            )
            let doc = new LengkoDockContent(codeBox, "Program.fs")
            doc.Show(dockPanel, DockState.Document)

            // 2. Left: Solution Explorer
            let tree = new TreeView(Dock = DockStyle.Fill, BorderStyle = BorderStyle.None)
            let root = tree.Nodes.Add("Solution 'FSharp.Demo'")
            root.Nodes.Add("Program.fs") |> ignore
            root.ExpandAll()
            let explorer = new LengkoDockContent(tree, "Solution Explorer")
            explorer.Show(dockPanel, DockState.DockLeft)

            // 3. Right: Properties
            let grid = new PropertyGrid(Dock = DockStyle.Fill, SelectedObject = dockPanel)
            let prop = new LengkoDockContent(grid, "Properties")
            prop.Show(dockPanel, DockState.DockRight)

            // 4. Bottom: Output
            let outDoc = new LengkoDockContent(outputList, "Output")
            outDoc.Show(dockPanel, DockState.DockBottom)

            outputList.Items.Add("[Ready] F# Docking Workspace Loaded.") |> ignore
            dockPanel.ResumeLayout(true)

    [<STAThread>]
    [<EntryPoint>]
    let main _ =
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)
        Application.Run(new DockDemoForm())
        0
