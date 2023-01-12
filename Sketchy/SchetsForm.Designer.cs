namespace Sketchy
{
    partial class SchetsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.schetsControl1 = new Sketchy.SchetsControl();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // schetsControl1
            // 
            this.schetsControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.schetsControl1.Location = new System.Drawing.Point(123, 20);
            this.schetsControl1.Name = "schetsControl1";
            this.schetsControl1.Size = new System.Drawing.Size(665, 389);
            this.schetsControl1.TabIndex = 0;
            this.schetsControl1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.schetsControl1_KeyPress);
            this.schetsControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.schetsControl1_MouseDown);
            this.schetsControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.schetsControl1_MouseMove);
            this.schetsControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.schetsControl1_MouseUp);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 29);
            this.button1.TabIndex = 1;
            this.button1.Tag = "PenTool";
            this.button1.Text = "Pen";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 55);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 29);
            this.button2.TabIndex = 2;
            this.button2.Text = "Lijn";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 90);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(94, 29);
            this.button3.TabIndex = 3;
            this.button3.Text = "Kader";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 125);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(94, 29);
            this.button4.TabIndex = 4;
            this.button4.Text = "vierkant";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(12, 160);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(94, 29);
            this.button5.TabIndex = 5;
            this.button5.Text = "tekst";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(12, 195);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(94, 29);
            this.button6.TabIndex = 6;
            this.button6.Text = "gum";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(123, 415);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(94, 29);
            this.button7.TabIndex = 7;
            this.button7.Text = "clear";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(223, 415);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(94, 29);
            this.button8.TabIndex = 8;
            this.button8.Text = "rotate";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // SchetsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.schetsControl1);
            this.Name = "SchetsForm";
            this.Text = "SchetsForm";
            this.Load += new System.EventHandler(this.SchetsForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private SchetsControl schetsControl1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
    }
}