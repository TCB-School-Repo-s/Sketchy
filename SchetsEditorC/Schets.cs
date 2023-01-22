using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text.Json;
using System.IO;

public class Schets
{
    private Bitmap bitmap;
    public List<SchetsElement> sketchElements = new List<SchetsElement>();
    public bool sketchChanged { get; set; }
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
            element.DrawElement(gr);
        }
    }

    public void SaveBitmap(Object o, EventArgs e)
    {
        using (SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = @"PNG|*.png|JPEG|*.jpeg|BMP|*.bmp" })
        {   
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.sketchChanged = false;
                bitmap.Save(saveFileDialog.FileName);
                Debug.WriteLine(this.sketchChanged);
            }
        }
    }

    public void SaveProject(Object o, EventArgs e)
    {
        var project = JsonSerializer.Serialize<List<SchetsElement>>(sketchElements);
        using (SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = @"Sketchy|*.sketchy" })
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using StreamWriter file = new(saveFileDialog.FileName);
                file.WriteLineAsync(project);
            }
        }
    }

    public void OpenProject(Object o, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = @"Sketchy|*.sketchy" })
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using StreamReader file = new(openFileDialog.FileName);
                var project = file.ReadToEnd();
                sketchElements = JsonSerializer.Deserialize<List<SchetsElement>>(project);
                Debug.WriteLine(sketchElements[0].kleur);
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