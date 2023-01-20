using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;


public class Schets
{
    private Bitmap bitmap;
    public List<SchetsElement> sketchElements = new List<SchetsElement>();
    public bool changes { get; set; }
    public Schets()
    {
        bitmap = new Bitmap(1, 1);
    }
    public Graphics BitmapGraphics
    {
        get { return Graphics.FromImage(bitmap); }
    }
    public void VeranderAfmeting(Size sz)
    {
        if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
        {
            Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                     , Math.Max(sz.Height, bitmap.Size.Height)
                                     );
            Graphics gr = Graphics.FromImage(nieuw);
            gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
            gr.DrawImage(bitmap, 0, 0);
            bitmap = nieuw;
        }
    }
    public void Teken(Graphics gr)
    {
        gr.DrawImage(bitmap, 0, 0);
        foreach(SchetsElement element in sketchElements)
        {
            element.Draw(gr);
        }
    }

    public void SaveBitmap(Object o, EventArgs e)
    {
        using (SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = @"PNG|*.png|JPEG|*.jpeg|BMP|*.bmp" })
        {   
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {   changes = false;
                bitmap.Save(saveFileDialog.FileName);
                Debug.WriteLine(changes);
            }
        }
    }
    
    public void ImportBitmap(Bitmap bp)
    {
        bitmap = bp;
    }

    public void AddSketchElement(SchetsElement element)
    {
        sketchElements.Add(element);
        SchetsElement first = sketchElements[0];
        Debug.WriteLine(first.GetColor().Name);
    }

    public void RemoveSketchElement(SchetsElement element)
    {
        sketchElements.Remove(element);
    }
    
    public void Schoon()
    {
        Graphics gr = Graphics.FromImage(bitmap);
        gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
    }
    public void Roteer()
    {
        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
    }

}