using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

public class SchetsWin : Form
{   
    MenuStrip menuStrip;
    SchetsControl schetscontrol;
    ISchetsTool huidigeTool;
    ColorDialog colorDialog = new ColorDialog();
    Panel paneel;
    bool vast;
    public bool changes;
    
    private void veranderAfmeting(object o, EventArgs ea)
    {
        schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                      , this.ClientSize.Height - 50);
        paneel.Location = new Point(64, this.ClientSize.Height - 30);
    }

    private void klikToolMenu(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
    }

    private void klikToolButton(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
    }

    private void afsluiten(object obj, EventArgs ea)
    {
        
        this.Close();
    }
    
    private void onFormClose(object obj, FormClosingEventArgs e)
    {
        if (this.schetscontrol.Schets.sketchChanged)
        {
            DialogResult stillclose = MessageBox.Show("Are you sure you want to close without saving?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (stillclose != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }

    public SchetsWin()
    {
        ISchetsTool[] deTools = { new PenTool()         
                                , new LijnTool()
                                , new RechthoekTool()
                                , new VolRechthoekTool()
                                , new CirkelTool()
                                , new VolCirkelTool()
                                , new TekstTool()
                                , new GumTool()
                                };
        String[] deKleuren = { "Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan" };

        this.ClientSize = new Size(700, 500);
        huidigeTool = deTools[0];

        schetscontrol = new SchetsControl();
        schetscontrol.Location = new Point(64, 10);
        schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                    {
                                        vast = true; this.schetscontrol.Schets.sketchChanged = true;
                                        huidigeTool.MuisVast(schetscontrol, mea.Location); 
                                    };
        schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                    {   if (vast)
                                        huidigeTool.MuisDrag(schetscontrol, mea.Location); 
                                    };
        schetscontrol.MouseUp   += (object o, MouseEventArgs mea) =>
                                    {   if (vast)
                                        huidigeTool.MuisLos(schetscontrol, mea.Location);
                                        vast = false; 
                                    };
        schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) => 
                                    {   huidigeTool.Letter  (schetscontrol, kpea.KeyChar); 
                                    };
        this.Controls.Add(schetscontrol);

        menuStrip = new MenuStrip();
        menuStrip.Visible = false;
        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakToolMenu(deTools);
        this.maakActieMenu(deKleuren);
        this.maakToolButtons(deTools);
        this.maakActieButtons(deKleuren);
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
        this.FormClosing += this.onFormClose;
    }

    private void maakFileMenu()
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("File");
        menu.MergeAction = MergeAction.MatchOnly;
        menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
        menuStrip.Items.Add(menu);
    }

    private void maakToolMenu(ICollection<ISchetsTool> tools)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
        foreach (ISchetsTool tool in tools)
        {   ToolStripItem item = new ToolStripMenuItem();
            item.Tag = tool;
            item.Text = tool.ToString();
            item.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            item.Click += this.klikToolMenu;
            menu.DropDownItems.Add(item);
        }
        menuStrip.Items.Add(menu);
    }

    private void maakActieMenu(String[] kleuren)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Actie");
        menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
        menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer);
        menuStrip.Items.Add(menu);
        menu.DropDownItems.Add("Save project", null, schetscontrol.Schets.SaveProject);
        menu.DropDownItems.Add("Open project", null, schetscontrol.OpenProject);
    }

    private void maakToolButtons(ICollection<ISchetsTool> tools)
    {
        int t = 0;
        foreach (ISchetsTool tool in tools)
        {
            RadioButton b = new RadioButton();
            b.Appearance = Appearance.Button;
            b.Size = new Size(45, 62);
            b.Location = new Point(10, 10 + t * 62);
            b.Tag = tool;
            b.Text = tool.ToString();
            b.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            b.TextAlign = ContentAlignment.TopCenter;
            b.ImageAlign = ContentAlignment.BottomCenter;
            b.Click += this.klikToolButton;
            this.Controls.Add(b);
            if (t == 0) b.Select();
            t++;
        }
    }

    private void maakActieButtons(String[] kleuren)
    {   
        paneel = new Panel(); this.Controls.Add(paneel);
        paneel.Size = new Size(600, 24);
            
        Button clear = new Button(); paneel.Controls.Add(clear);
        clear.Text = "Clear";  
        clear.Location = new Point(  0, 0); 
        clear.Click += schetscontrol.Schoon;

        Button undo = new Button(); paneel.Controls.Add(undo);
        undo.Text = "Undo";
        undo.Location = new Point(80, 0);
        undo.Click += schetscontrol.undo;
        Button rotate = new Button(); paneel.Controls.Add(rotate);
        rotate.Text = "Rotate"; 
        rotate.Location = new Point( 160, 0); 
        rotate.Click += schetscontrol.Roteer;

        Button saveimg = new Button(); paneel.Controls.Add(saveimg);
        saveimg.Text = "Save img";
        saveimg.Location = new Point(240, 0);
        saveimg.Click += schetscontrol.Schets.SaveBitmap;

        Button openimg = new Button(); paneel.Controls.Add(openimg);
        openimg.Text = "Open img";
        openimg.Location = new Point(320, 0);
        openimg.Click += schetscontrol.OpenBitmap;

        Button kleurBttn = new Button(); paneel.Controls.Add(kleurBttn);
        kleurBttn.Text = "Kies kleur";
        kleurBttn.Location = new Point(420, 0);
        kleurBttn.Click += KleurBttn_Click;
        
        /*ComboBox cbb = new ComboBox(); paneel.Controls.Add(cbb);
        cbb.Location = new Point(400, 0); 
        cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
        cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
        foreach (string k in kleuren)
            cbb.Items.Add(k);
        cbb.SelectedIndex = 0;*/
    }

    private void KleurBttn_Click(object sender, EventArgs e)
    {
        this.schetscontrol.Schets.OpenColorDialog(this.schetscontrol);
    }
}