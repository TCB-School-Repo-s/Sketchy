using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Policy;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.Xml.Linq;

public interface ISchetsTool {
  void MuisVast(SchetsControl s, Point p);
  void MuisDrag(SchetsControl s, Point p);
  void MuisLos(SchetsControl s, Point p);
  void Letter(SchetsControl s, KeyEventArgs e);
}

public abstract class StartpuntTool : ISchetsTool {
  protected Point startpunt;
  protected Brush kwast;

  public virtual void MuisVast(SchetsControl s, Point p) { startpunt = p; }
  public virtual void MuisLos(SchetsControl s, Point p) {
    kwast = new SolidBrush(s.PenKleur);
  }
  public abstract void MuisDrag(SchetsControl s, Point p);
  public abstract void Letter(SchetsControl s, KeyEventArgs e);
}

public class TekstTool : StartpuntTool {
  public override string ToString() { return "tekst"; }

  private string CurrentString = "";
  private Font font = new Font("Arial", 20);
  public override void MuisDrag(SchetsControl s, Point p) {}

  public override void Letter(SchetsControl s, KeyEventArgs e) {
    if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Shift) {
      CurrentString += "\n";
    } else if (e.KeyCode == Keys.Enter && e.Modifiers != Keys.Shift) {
      GraphicsPath path = new GraphicsPath();
      path.AddString(CurrentString, this.font.FontFamily, (int)this.font.Style,
                     this.font.Size + 5, startpunt,
                     StringFormat.GenericDefault);
      s.Schets.sketchElements.AddLast(new SchetsElement(
          new ElementPathData(path.PathTypes, path.PathPoints), s.PenKleur,
          true, false));
      s.Schets.BitmapGraphics.FillRectangle(
          Brushes.White, 0, 0, s.Schets.bitmap.Width, s.Schets.bitmap.Height);
      s.Invalidate();
      CurrentString = "";
    } else if (e.KeyCode == Keys.Back) {
      if (CurrentString.Length > 0) {
        Size size = TextRenderer.MeasureText(CurrentString, font);
        s.CreateGraphics().FillRectangle(Brushes.White, startpunt.X,
                                         startpunt.Y, size.Width, size.Height);
        CurrentString = CurrentString.Substring(0, CurrentString.Length - 1);
      }
    } else if (e.KeyCode == Keys.Space) {
      CurrentString += " ";
    } else {
      if (Char.IsLetter((char)e.KeyValue) ||
          Char.IsSeparator((char)e.KeyValue) ||
          Char.IsPunctuation((char)e.KeyValue)) {
        if (e.Modifiers == Keys.Shift || e.Modifiers == Keys.CapsLock) {
          CurrentString += e.KeyCode.ToString();
        } else {
          CurrentString += e.KeyCode.ToString().ToLower();
        }
      }
    }
    s.CreateGraphics().DrawString(CurrentString, font, kwast, startpunt);
  }
}

public abstract class TweepuntTool : StartpuntTool {
  public static Rectangle Punten2Rechthoek(Point p1, Point p2) {
    return new Rectangle(
        new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y)),
        new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y)));
  }
  public static Pen MaakPen(Brush b, int dikte) {
    Pen pen = new Pen(b, dikte);
    pen.StartCap = LineCap.Round;
    pen.EndCap = LineCap.Round;
    return pen;
  }
  public override void MuisVast(SchetsControl s, Point p) {
    base.MuisVast(s, p);
    kwast = Brushes.Gray;
  }
  public override void MuisDrag(SchetsControl s, Point p) {
    s.Refresh();
    this.Bezig(s.CreateGraphics(), this.startpunt, p, s);
  }
  public override void MuisLos(SchetsControl s, Point p) {
    base.MuisLos(s, p);
    this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p, s);
    s.Invalidate();
  }
  public override void Letter(SchetsControl s, KeyEventArgs e) {}
  public abstract void Bezig(Graphics g, Point p1, Point p2, SchetsControl s);

  public abstract void Compleet(Graphics g, Point p1, Point p2,
                                SchetsControl s);
}

public class RechthoekTool : TweepuntTool {
  public override string ToString() { return "kader"; }

  public override void Bezig(Graphics g, Point p1, Point p2, SchetsControl s) {
    g.DrawRectangle(MaakPen(kwast, 3), Punten2Rechthoek(p1, p2));
  }

  public override void Compleet(Graphics g, Point p1, Point p2,
                                SchetsControl s) {
    GraphicsPath path = new GraphicsPath();
    path.AddRectangle(Punten2Rechthoek(p1, p2));
    s.Schets.AddSketchElement(new SchetsElement(
        new ElementPathData(path.PathTypes, path.PathPoints), s.PenKleur));
  }
}

public class VolRechthoekTool : RechthoekTool {
  public override string ToString() { return "vlak"; }

  public override void Compleet(Graphics g, Point p1, Point p2,
                                SchetsControl s) {
    GraphicsPath path = new GraphicsPath();
    path.AddRectangle(Punten2Rechthoek(p1, p2));
    s.Schets.AddSketchElement(
        new SchetsElement(new ElementPathData(path.PathTypes, path.PathPoints),
                          s.PenKleur, true));
  }
}

public class CirkelTool : TweepuntTool {
  public override string ToString() { return "cirkel"; }

  public override void Bezig(Graphics g, Point p1, Point p2, SchetsControl s) {
    g.DrawEllipse(MaakPen(kwast, 3), Punten2Rechthoek(p1, p2));
  }

  public override void Compleet(Graphics g, Point p1, Point p2,
                                SchetsControl s) {
    GraphicsPath path = new GraphicsPath();
    path.AddEllipse(Punten2Rechthoek(p1, p2));
    s.Schets.AddSketchElement(new SchetsElement(
        new ElementPathData(path.PathTypes, path.PathPoints), s.PenKleur));
  }
}

public class VolCirkelTool : CirkelTool {
  public override string ToString() { return "bol"; }

  public override void Compleet(Graphics g, Point p1, Point p2,
                                SchetsControl s) {
    GraphicsPath path = new GraphicsPath();
    path.AddEllipse(Punten2Rechthoek(p1, p2));
    s.Schets.AddSketchElement(
        new SchetsElement(new ElementPathData(path.PathTypes, path.PathPoints),
                          s.PenKleur, true));
  }
}

public class LijnTool : TweepuntTool {
  public override string ToString() { return "lijn"; }

  public override void Bezig(Graphics g, Point p1, Point p2, SchetsControl s) {
    g.DrawLine(MaakPen(this.kwast, 3), p1, p2);
  }

  public override void Compleet(Graphics g, Point p1, Point p2,
                                SchetsControl s) {
    GraphicsPath path = new GraphicsPath();
    path.AddLine(p1, p2);
    s.Schets.AddSketchElement(new SchetsElement(
        new ElementPathData(path.PathTypes, path.PathPoints), s.PenKleur));
  }
}

public class PenTool : ISchetsTool {
  public override string ToString() { return "pen"; }

  private Point startpunt;
  private GraphicsPath path = new GraphicsPath();
  public void Letter(SchetsControl s, KeyEventArgs e) {}

  public void MuisDrag(SchetsControl s, Point p) {
    this.Bezig(s, p, s.CreateGraphics());
    startpunt = p;
  }

  public void MuisLos(SchetsControl s, Point p) { this.Compleet(s, p); }

  public void MuisVast(SchetsControl s, Point p) { startpunt = p; }

  public void Bezig(SchetsControl s, Point p, Graphics gr) {
    gr.DrawLine(new Pen(s.PenKleur, 3), startpunt, p);
    path.AddLine(startpunt, p);
  }

  public void Compleet(SchetsControl s, Point p) {
    path.Flatten();
    s.Schets.sketchElements.AddLast(
        new SchetsElement(new ElementPathData(path.PathTypes, path.PathPoints),
                          s.PenKleur, false, true));
    s.Schets.BitmapGraphics.FillRectangle(
        Brushes.White, 0, 0, s.Schets.bitmap.Width, s.Schets.bitmap.Height);
    s.Invalidate();
    path.Reset();
  }
}

public class GumTool : ISchetsTool {
  public override string ToString() { return "gum"; }
  public void Letter(SchetsControl s, KeyEventArgs e) {}

  public void MuisDrag(SchetsControl s, Point p) {
    for (LinkedListNode<SchetsElement> node = s.Schets.sketchElements.Last;
         node != null; node = node.Previous) {
      SchetsElement el = node.Value;
      if (el.CheckInBounds(p)) {
        s.Schets.sketchElements.Remove(node);
        s.Schets.BitmapGraphics.FillRectangle(
            Brushes.White, 0, 0, s.Schets.bitmap.Width, s.Schets.bitmap.Height);
        s.Invalidate();
        break;
      }
    }
  }

  public void MuisLos(SchetsControl s, Point p) {
    for (LinkedListNode<SchetsElement> node = s.Schets.sketchElements.Last;
         node != null; node = node.Previous) {
      SchetsElement el = node.Value;
      if (el.CheckInBounds(p)) {
        s.Schets.sketchElements.Remove(node);
        s.Schets.BitmapGraphics.FillRectangle(
            Brushes.White, 0, 0, s.Schets.bitmap.Width, s.Schets.bitmap.Height);
        s.Invalidate();
        break;
      }
    }
  }

  public void MuisVast(SchetsControl s, Point p) {}
}

public class SchetsElement {
  public ElementPathData data { get; set; }
  public bool isFilled { get; set; }
  public bool isPenTool { get; set; }
  public Color kleur { get; set; }

  public SchetsElement(ElementPathData data, Color kleur, bool isFilled = false,
                       bool isPenTool = false) {
    this.data = data;
    this.isFilled = isFilled;
    this.kleur = kleur;
    this.isPenTool = isPenTool;
  }

  public bool CheckInBounds(Point p) {
    GraphicsPath path = new GraphicsPath(data.points, data.type);
    RectangleF bounds = path.GetBounds();

    if (isPenTool) {
      if (path.PathPoints.Contains(p))
        return true;
    } else {
      if (p.X >= bounds.Left && p.X <= bounds.Right && p.Y >= bounds.Top &&
          p.Y <= bounds.Bottom) {
        return true;
      }
    }
    return false;
  }

  public void Draw(Graphics gr) {
    GraphicsPath path = new GraphicsPath(data.points, data.type);
    if (isFilled) {
      path.Flatten();
      gr.FillPath(new SolidBrush(kleur), path);
    } else {
      path.Flatten();
      gr.DrawPath(new Pen(kleur, 3), path);
    }
  }
}

public class ElementPathData {
  public byte[] type { get; set; }
  public PointF[] points { get; set; }

  public ElementPathData(byte[] type, PointF[] points) {
    this.type = type;
    this.points = points;
  }
}
