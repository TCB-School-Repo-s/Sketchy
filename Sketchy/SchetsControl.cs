using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketchy
{
    public class SchetsControl : UserControl
    {
        private Schets schets;
        private Color penkleur;

        public Color PenKleur
        {
            get { return penkleur; }
        }
        public Schets Schets
        {
            get { return schets; }
        }
        public SchetsControl()
        {
            this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        private void teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics);
        }
        private void veranderAfmeting(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }
        public Graphics MaakBitmapGraphics()
        {
            Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }
        public void Schoon()
        {
            schets.Schoon();
            this.Invalidate();
        }
        public void Roteer()
        {
            schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            schets.Roteer();
            this.Invalidate();
        }
        public void VeranderKleur(ComboBox box)
        {
            string kleurNaam = (box.Text);
            penkleur = Color.FromName(kleurNaam);
        }
        public void VeranderKleurViaMenu()
        {
            //string kleurNaam = ((ToolStripMenuItem)obj).Text;
            penkleur = Color.FromName("black");
        }
    }
}
