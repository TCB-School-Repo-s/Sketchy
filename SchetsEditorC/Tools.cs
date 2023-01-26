using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json.Serialization;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c);
}

public abstract class StartpuntTool : ISchetsTool
{
    protected Point startpunt;
    protected Brush kwast;

    public virtual void MuisVast(SchetsControl s, Point p)
    {   
        startpunt = p;
    }
    public virtual void MuisLos(SchetsControl s, Point p)
    {   
        kwast = new SolidBrush(s.PenKleur);
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c);
}

public class TekstTool : StartpuntTool
{
    public override string ToString() { return "tekst"; }

    public override void MuisDrag(SchetsControl s, Point p) { }

    public override void Letter(SchetsControl s, char c)
    {
        if (c >= 32)
        {
            Graphics gr = s.MaakBitmapGraphics();
            Font font = new Font("Tahoma", 20);
            string tekst = c.ToString();
            SizeF sz = 
            gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
            gr.DrawString   (tekst, font, kwast, 
                                            this.startpunt, StringFormat.GenericTypographic);
            // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
            startpunt.X += (int)sz.Width;
            s.Invalidate();
        }
    }
}

public abstract class TweepuntTool : StartpuntTool
{
    public static Rectangle Punten2Rechthoek(Point p1, Point p2)
    {  
        return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y)), new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y)));
    }
    public static Pen MaakPen(Brush b, int dikte)
    {   
        Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }
    public override void MuisVast(SchetsControl s, Point p)
    {   
        base.MuisVast(s, p);
        kwast = Brushes.Gray;
    }
    public override void MuisDrag(SchetsControl s, Point p)
    {   
        s.Refresh();
        this.Bezig(s.CreateGraphics(), this.startpunt, p, s);
    }
    public override void MuisLos(SchetsControl s, Point p)
    {   
        base.MuisLos(s, p);
        this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p, s);
        s.Invalidate();
    }
    public override void Letter(SchetsControl s, char c)
    {
    }
    public abstract void Bezig(Graphics g, Point p1, Point p2, SchetsControl s);

    public abstract void Compleet(Graphics g, Point p1, Point p2, SchetsControl s);
}

public class RechthoekTool : TweepuntTool
{
    public override string ToString() { return "kader"; }

    public override void Bezig(Graphics g, Point p1, Point p2, SchetsControl s)
    {   
        g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        s.Schets.AddSketchElement(new SchetsElement(ElementType.EmptyRectangle, p1, p2, s.PenKleur));
    }
}
    
public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {   
        g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        s.Schets.AddSketchElement(new SchetsElement(ElementType.FilledRectangle, p1, p2, s.PenKleur));
    }

}

public class CirkelTool : TweepuntTool
{
    public override string ToString() { return "cirkel"; }

    public override void Bezig(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        s.Schets.AddSketchElement(new SchetsElement(ElementType.EmptyEllipse, p1, p2, s.PenKleur));
    }
}

public class VolCirkelTool : CirkelTool
{
    public override string ToString() { return "bol"; }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        s.Schets.AddSketchElement(new SchetsElement(ElementType.FilledEllipse, p1, p2, s.PenKleur));
    }
}

public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2, SchetsControl s)
    {  
        g.DrawLine(MaakPen(this.kwast,3), p1, p2);
    }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        s.Schets.AddSketchElement(new SchetsElement(ElementType.Line, p1, p2, s.PenKleur));
    }
}

public class PenTool : LijnTool
{
    public override string ToString() { return "pen"; }

    public override void MuisDrag(SchetsControl s, Point p)
    {   
        this.MuisLos(s, p);
        this.MuisVast(s, p);
    }
}
    
public class GumTool : PenTool
{
    public override string ToString() { return "gum"; }

    public override void Bezig(Graphics g, Point p1, Point p2, SchetsControl s)
    {  
        
    }
}

public enum ElementType
{
    FilledEllipse,
    EmptyEllipse,
    FilledRectangle,
    EmptyRectangle,
    Text,
    Line,
}


public class SchetsElement
{
    public ElementType type { get; set; }
    public Point beginPunt { get; set; }
    public Point eindPunt { get; set; }
    public Color kleur { get; set; }
    public string? text { get; set; }


    [JsonConstructor]
    public SchetsElement(ElementType type, Point beginPunt, Point eindPunt, Color kleur, string? text = null)
    {
        this.type = type;
        this.beginPunt = beginPunt;
        this.eindPunt = eindPunt;
        this.kleur = kleur;
        this.text = text;
    }

    public void DrawElement(Graphics gr)
    {
        switch (type)
        {
            case ElementType.EmptyEllipse:
                gr.DrawEllipse(new Pen(kleur, 3), TweepuntTool.Punten2Rechthoek(beginPunt, eindPunt));
                break;
            case ElementType.FilledEllipse:
                gr.FillEllipse(new SolidBrush(kleur), TweepuntTool.Punten2Rechthoek(beginPunt, eindPunt));
                break;
            case ElementType.EmptyRectangle:
                gr.DrawRectangle(new Pen(kleur, 3), TweepuntTool.Punten2Rechthoek(beginPunt, eindPunt));
                break;
            case ElementType.FilledRectangle:
                gr.FillRectangle(new SolidBrush(kleur), TweepuntTool.Punten2Rechthoek(beginPunt, eindPunt));
                break;
            case ElementType.Line:
                gr.DrawLine(new Pen(kleur, 3), beginPunt, eindPunt);
                break;
        }
            
    }

    
    
}