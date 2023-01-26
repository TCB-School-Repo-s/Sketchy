using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

public class Schets {
  public Bitmap bitmap { get; private set; }
  public LinkedList<SchetsElement> sketchElements =
      new LinkedList<SchetsElement>();
  ColorDialog colorDialog = new ColorDialog();
  public bool sketchChanged { get; set; }
  public Schets() { bitmap = new Bitmap(1, 1); }
  public Graphics BitmapGraphics {
    get { return Graphics.FromImage(bitmap); }
  }
  public void VeranderAfmeting(Size sz) {
    if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height) {
      Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width),
                                Math.Max(sz.Height, bitmap.Size.Height));
      Graphics gr = Graphics.FromImage(nieuw);
      gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
      gr.DrawImage(bitmap, 0, 0);
      bitmap = nieuw;
    }
  }
  public void Teken(Graphics gr) {
    foreach (SchetsElement element in sketchElements) {
      element.DrawElement(this.BitmapGraphics);
    }
    gr.DrawImage(bitmap, 0, 0);
  }

  public void OpenColorDialog(SchetsControl control) {
    if (colorDialog.ShowDialog() == DialogResult.OK) {
      control.PenKleur = colorDialog.Color;
    }
  }

  public void SaveBitmap(Object o, EventArgs e) {
    using (SaveFileDialog saveFileDialog = new SaveFileDialog() {
      Filter = @"PNG|*.png|JPEG|*.jpeg|BMP|*.bmp"
    }) {
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        this.sketchChanged = false;
        bitmap.Save(saveFileDialog.FileName);
        Debug.WriteLine(this.sketchChanged);
      }
    }
  }

  public async void SaveProject(Object o, EventArgs e) {
    SaveFileObject FileObject =
        new SaveFileObject(colorDialog.CustomColors, sketchElements);
    string FinalFileData = JsonConvert.SerializeObject(FileObject);
    using (SaveFileDialog saveFileDialog =
               new SaveFileDialog() { Filter = @"Sketchy|*.sketchy" }) {
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        using StreamWriter fileToSave = new(saveFileDialog.FileName);
        await fileToSave.WriteLineAsync(FinalFileData);
      }
    }
  }

  public void OpenProject() {
    using (OpenFileDialog openFileDialog =
               new OpenFileDialog() { Filter = @"Sketchy|*.sketchy" }) {
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        using StreamReader file = new(openFileDialog.FileName);
        var project = file.ReadToEnd();
        SaveFileObject saveFile =
            JsonConvert.DeserializeObject<SaveFileObject>(project);
        sketchElements = saveFile.FileData;
        colorDialog.CustomColors = saveFile.CustomColor;
        Debug.WriteLine(sketchElements.First.Value.kleur);
      }
    }
  }

  public void ImportBitmap(Bitmap bp) { bitmap = bp; }

  public void AddSketchElement(SchetsElement element) {
    sketchElements.AddLast(element);
  }

  public void RemoveSketchElement(SchetsElement element) {
    sketchElements.Remove(element);
  }

  public void Schoon() {
    sketchElements.Clear();
    Graphics gr = Graphics.FromImage(bitmap);
    gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
  }
  public void Roteer() { bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone); }
}

public class SaveFileObject {
  public int[] CustomColor { get; set; }
  public LinkedList<SchetsElement> FileData { get; set; }

  public SaveFileObject(int[] colors, LinkedList<SchetsElement> data) {
    this.CustomColor = colors;
    this.FileData = data;
  }
}