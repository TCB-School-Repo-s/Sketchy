using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sketchy
{
    public partial class SchetsForm : Form
    {

        bool vast;
        ISchetsTool[] deTools = { new PenTool()
                                , new LijnTool()
                                , new RechthoekTool()
                                , new VolRechthoekTool()
                                , new CirkelTool()
                                , new VolCirkelTool()
                                , new TekstTool()
                                , new GumTool()
                                };
        ISchetsTool huidigeTool;
        String[] deKleuren = { "Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan" };
        public SchetsForm()
        {
            InitializeComponent();
            this.huidigeTool = deTools[0];
        }

        private void button8_Click(object sender, EventArgs e)
        {
            schetsControl1.Roteer();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            schetsControl1.Schoon();
        }

        private void SchetsForm_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.huidigeTool = deTools[0];
        }

        private void schetsControl1_MouseDown(object sender, MouseEventArgs e)
        {
            vast = true;
            this.huidigeTool.MuisVast(schetsControl1, e.Location);
        }

        private void schetsControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (vast) huidigeTool.MuisDrag(schetsControl1, e.Location);
        }

        private void schetsControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (vast)
                huidigeTool.MuisLos(schetsControl1, e.Location);
            vast = false;
        }

        private void schetsControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            huidigeTool.Letter(schetsControl1, e.KeyChar);
        }
    }
}
