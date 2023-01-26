using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Policy;
using System.Text.Json.Serialization;
using System.Xml.Linq;

public interface ISchetsTool {
  void MuisVast(SchetsControl s, Point p);
  void MuisDrag(SchetsControl s, Point p);
  void MuisLos(SchetsControl s, Point p);
  void Letter(SchetsControl s, char c);
}

public abstract class StartpuntTool : ISchetsTool {
  protected Point startpunt;
  protected Brush kwast;

  public virtual void MuisVast(SchetsControl s, Point p) { startpunt = p; }
  public virtual void MuisLos(SchetsControl s, Point p) {
    kwast = new SolidBrush(s.PenKleur);
  }
  public abstract void MuisDrag(SchetsControl s, Point p);
  public abstract void Letter(SchetsControl s, char c);
}

public class TekstTool : StartpuntTool {
  public override string ToString() { return "tekst"; }

  public override void MuisDrag(SchetsControl s, Point p) {}

  public override void Letter(SchetsControl s, char c) {
    if (c >= 32) {
      Graphics gr = s.MaakBitmapGraphics();
      Font font = new Font("Tahoma", 20);
      string tekst = c.ToString();
      SizeF sz = gr.MeasureString(tekst, font, this.startpunt,
                                  StringFormat.GenericTypographic);
      gr.DrawString(tekst, font, kwast, this.startpunt,
                    StringFormat.GenericTypographic);
      // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width,
      // sz.Height);
      startpunt.X += (int)sz.Width;
      s.Invalidate();
    }
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
  public override void Letter(SchetsControl s, char c) {}
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
    s.Schets.AddSketchElement(new SchetsElement(path, s.PenKleur));
  }
}

public class VolRechthoekTool : RechthoekTool {
  public override string ToString() { return "vlak"; }

  public override void Compleet(Graphics g, Point p1, Point p2,
                                SchetsControl s) {
    GraphicsPath path = new GraphicsPath();
    path.AddRectangle(Punten2Rechthoek(p1, p2));
    s.Schets.AddSketchElement(new SchetsElement(path, s.PenKleur, null, true));
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
    s.Schets.AddSketchElement(new SchetsElement(path, s.PenKleur));
  }
}

public class VolCirkelTool : CirkelTool {
  public override string ToString() { return "bol"; }

  public override void Compleet(Graphics g, Point p1, Point p2,
                                SchetsControl s) {
    GraphicsPath path = new GraphicsPath();
    path.AddEllipse(Punten2Rechthoek(p1, p2));
    s.Schets.AddSketchElement(new SchetsElement(path, s.PenKleur, null, true));
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
    s.Schets.AddSketchElement(new SchetsElement(path, s.PenKleur));
  }
}

public class PenTool : ISchetsTool {
  public override string ToString() { return "pen"; }

  private Point startpunt;
  private GraphicsPath path = new GraphicsPath();
  public void Letter(SchetsControl s, char c) {}

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
    s.Schets.sketchElements.AddLast(new SchetsElement(
        (GraphicsPath)path.Clone(), s.PenKleur, null, false, true));
    s.Schets.BitmapGraphics.FillRectangle(
        Brushes.White, 0, 0, s.Schets.bitmap.Width, s.Schets.bitmap.Height);
    s.Invalidate();
    path.Reset();
  }
}

public class GumTool : ISchetsTool {
  public override string ToString() { return "gum"; }
  public void Letter(SchetsControl s, char c) {}

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
  public GraphicsPath path { get; set; }
  public bool isFilled { get; set; }
  public bool isPenTool { get; set; }
  public Color kleur { get; set; }
  public string? text { get; set; }

  public SchetsElement(GraphicsPath path, Color kleur, string? text = null,
                       bool isFilled = false, bool isPenTool = false) {
    this.path = path;
    this.isFilled = isFilled;
    this.kleur = kleur;
    this.text = text;
    this.isPenTool = isPenTool;
  }

  public bool CheckInBounds(Point p) {
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
    if (isFilled) {
      gr.FillPath(new SolidBrush(kleur), path);
    } else {
      gr.DrawPath(new Pen(kleur, 3), path);
    }
  }
}