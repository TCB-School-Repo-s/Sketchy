using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class SchetsControl : UserControl {
  private Schets schets;
  private Color penkleur = Color.Black;

  public Color PenKleur {
    get { return penkleur; }
    set { penkleur = value; }
  }
  public Schets Schets {
    get { return schets; }
  }
  public SchetsControl() {
    this.BorderStyle = BorderStyle.Fixed3D;
    this.schets = new Schets();
    this.Paint += this.teken;
    this.Resize += this.veranderAfmeting;
    this.veranderAfmeting(null, null);
  }
  protected override void OnPaintBackground(PaintEventArgs e) {}
  private void teken(object o, PaintEventArgs pea) {
    schets.Teken(pea.Graphics);
  }
  private void veranderAfmeting(object o, EventArgs ea) {
    schets.VeranderAfmeting(this.ClientSize);
    this.Invalidate();
  }
  public Graphics MaakBitmapGraphics() {
    Graphics g = schets.BitmapGraphics;
    g.SmoothingMode = SmoothingMode.AntiAlias;
    return g;
  }
  public void Schoon(object o, EventArgs ea) {
    schets.Schoon();
    schets.sketchChanged = false;
    this.Invalidate();
  }
  public void Roteer(object o, EventArgs ea) {
    schets.VeranderAfmeting(
        new Size(this.ClientSize.Height, this.ClientSize.Width));
    schets.Roteer();
    this.Invalidate();
  }

  public void OpenProject(object o, EventArgs e) {
    this.schets.OpenProject();
    this.Invalidate();
  }
  public void OpenBitmap(object o, EventArgs e) {
    using (OpenFileDialog dlg = new OpenFileDialog()) {
      dlg.Title = "Open Image";
      dlg.Filter =
          "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";

      if (dlg.ShowDialog() == DialogResult.OK) {
        this.Schets.ImportBitmap(new Bitmap(dlg.FileName));
      }
    }
    this.Invalidate();
  }
}
